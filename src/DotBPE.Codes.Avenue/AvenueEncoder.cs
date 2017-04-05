using DotNetty.Codecs;
using System;
using System.Collections.Generic;
using System.Text;
using DotNetty.Buffers;
using DotNetty.Transport.Channels;
using System.Diagnostics.Contracts;

namespace DotBPE.Codes.Avenue
{
    public class AvenueEncoder : MessageToByteEncoder<AvenueData>
    {
        /**
        *
        *   0........ 8........16........24........32
        *   1  |--0xA1---|headLen-|--version-|----M---| //协议标记,头长,版本号,路由数
        *   2  |---------------packetLen--------------| //包长 
        *   3  |---------------serviceId--------------| //服务id
        *   4  |----------------msgId-----------------| //请求id
        *   5  |---------------sequence---------------| //序列号
        *   6  |---------------optional---------------| //可选标记位
        *   7  |---------------priority---------------| //优先级
        *   8  |-------------signature----------------|
        *   9  |-------------signature----------------|
        *   10 |-------------signature----------------|
        *   11 |-------------signature----------------| //16字节签名
        *
        *   headLen 填44 + 扩展包头长度         *
        *   version 固定填1, 
        *   M 固定填0x0F
        *   packageLen 填实际包长    *
        *   serviceid   服务编号
        *   msgid 消息编号    
        *   serviceid,msgid 都为0表示心跳，这时无body    
        *   sequence为序列号    
        *   optional    标志位：    
        *       context 固定填0 
        *       mustReach 必达标志位 填0表示一般的包，填1表示必达包
        *       format 填 0 表示 tlv, 填 1 表示 json, 目前仅用到0
        *       encoding 填 0 表示 gbk, 填 1 表示 utf-8    *
        *   priority： 0 一般 1 高 实际未用到    *
        *  signature 全部填0    
        *  body: 格式和编码取决于format和encoding
        */
        protected override void Encode(IChannelHandlerContext context, AvenueData message, IByteBuffer output)
        {
            Contract.Requires(context != null);
            Contract.Requires(message != null);
            Contract.Requires(output != null);

            IByteBuffer buffer = context.Allocator.Buffer(message.PackageLength);


            buffer.WriteByte(message.Flag);
            buffer.WriteByte(message.HeadLength);
            buffer.WriteByte(message.Version);
            buffer.WriteByte(0x0F);

            buffer.WriteInt(message.PackageLength);
            buffer.WriteInt(message.ServiceId);
            buffer.WriteInt(message.MsgId);
            buffer.WriteInt(message.Sequence);

            buffer.WriteByte(0);
            buffer.WriteByte(message.MustReach);
            buffer.WriteByte(message.Format);
            buffer.WriteByte(message.Encoding);

            buffer.WriteInt(0);

            buffer.WriteBytes(Constants.EMPTY_SIGNATURE);

            if (message.XHead != null)
            {
                buffer.WriteBytes(message.XHead);
            }

            if(message.Body != null)
            {
                buffer.WriteBytes(message.Body);
            }
            output.EnsureWritable(message.PackageLength);
            output.WriteBytes(buffer);
        }
    }
}
