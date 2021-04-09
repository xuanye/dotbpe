using System;
using System.Collections.Generic;
using System.Text;

namespace DotBPE.Rpc.Protocol
{
    /// <summary>
    /// Amp Protocol
    /// </summary>
    public static class AmpProtocol
    {
        public const int InitialBytesToStrip = 0; //读取时需要跳过的字节数
        public const int LengthAdjustment = -5; //包实际长度的纠正，如果包长包括包头和包体，则要减去Length之前的部分
        public const int LengthFieldLength = 4;//长度字段的字节数 整型为4个字节
        public const int LengthFieldOffset = 1;//长度属性的起始（偏移）位
        public const int MaxFrameLength = int.MaxValue;
        public const int HeartbeatInterval = 15; // 15秒没消息发一个心跳

    }
}
