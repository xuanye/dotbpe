using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.IO;
using DotNetty.Buffers;
using DotNetty.Transport.Channels;
using DotNetty.Codecs;

namespace DotBPE.Codes.Avenue
{
    public class AvenueDecoder : ByteToMessageDecoder
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
       
        protected override void Decode(IChannelHandlerContext context, IByteBuffer input, List<object> output)
        {
            Contract.Requires(context != null);
            Contract.Requires(input != null);
            Contract.Requires(output != null);

            int length = input.ReadableBytes;

            if (length < Constants.STANDARD_HEADLEN) //不足一个标准头长度
            {
                return;
            }
            input.MarkReaderIndex();
            //开始读取头
            byte flag = input.ReadByte();
            if (flag != Constants.TYPE_REQUEST && flag != Constants.TYPE_RESPONSE)
            {
                throw new CodecException("package_type_error");
            }

            byte headLen = input.ReadByte();
            if(headLen<Constants.STANDARD_HEADLEN || headLen > length)
            {
                throw new CodecException("package_headlen_error, headLen=" + headLen 
                    + ",length=" + length);
            }
            
            byte version = input.ReadByte();
            if (version != Constants.VERSION_1)
            {
                throw new CodecException("package_version_error");
            }

            input.ReadByte();

            //包长
            int packetLen = input.ReadInt();

            //如果可读长度小于包长
            if (length < packetLen)
            {
                //在没有读完整个包之前，每次收到内容都从头开始读取
                input.ResetReaderIndex();
                return;
            }
            int serviceId = input.ReadInt();
            if (serviceId < 0)
            {
                throw new CodecException("package_serviceid_error");
            }
            int msgId = input.ReadInt();
            if (msgId != 0)
            {
                if (serviceId == 0)
                {
                    throw new CodecException("package_msgid_error");
                }
            }

            int sequence = input.ReadInt();

            //下4位为 optional
            //context
            input.ReadByte();
            byte mustReach = input.ReadByte();
            byte format = input.ReadByte();
            byte encoding = input.ReadByte();
            if (mustReach != Constants.MUSTREACH_NO && mustReach != Constants.MUSTREACH_YES)
            {
                throw new CodecException("package_mustreach_error");
            }

            if (format != Constants.FORMAT_TLV && format != Constants.FORMAT_JSON)
            {
                throw new CodecException("package_format_error");
            }

            if (encoding != Constants.ENCODING_GBK && encoding != Constants.ENCODING_UTF8)
            {
                throw new CodecException("package_encoding_error");
            }

            // 优先级实际没有用
            int priority = input.ReadInt();
            //signature
            input.ReadBytes(16);

            if (serviceId == 0 && msgId == 0 && length != Constants.STANDARD_HEADLEN)
            {
                throw new CodecException("package_ping_size_error");                
            }
            IByteBuffer xhead = null;
            if (headLen > Constants.STANDARD_HEADLEN)
            {
                xhead = context.Allocator.Buffer(headLen - Constants.STANDARD_HEADLEN);
                input.ReadBytes(xhead);
                xhead.SetReaderIndex(0);
            }          
            //开始读取Body
            IByteBuffer body = context.Allocator.Buffer(packetLen - headLen);
            input.ReadBytes(body);
            body.SetReaderIndex(0);
        
            var avenueData = new AvenueData(
                flag,
                version,
                serviceId,
                msgId,
                sequence,
                mustReach,
                encoding,
                format,
                xhead, body
            );
            output.Add(avenueData);
        }
    }
}
