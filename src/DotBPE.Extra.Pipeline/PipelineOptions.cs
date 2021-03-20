using DotBPE.Rpc;
using System.Runtime.CompilerServices;

namespace DotBPE.Extra
{
    public class PipelineOptions
    {
        private int[] DefaultRetryStrategy = new int[] { 10, 60, 2 * 60, 5 * 60, 10 * 60, 20 * 60, 30 * 60 };
        public PipelineOptions()
        {
            this.RetryMaxCount = 5;
            this.RetryExceptionCode = new int[] { RpcErrorCodes.CODE_INTERNAL_ERROR };
            this.RetryStrategy = DefaultRetryStrategy;
        }
        /// <summary>
        /// DB拉取时间间隔，单位秒
        /// </summary>
        public int DbPullInterval { get; set; } = 60;

        /// <summary>
        /// Redis拉取时间间隔,单位毫秒
        /// </summary>
        public int RedisPullIntervalMS { get; set; } = 500;

        public string RedisCacheKey { get; set; } = "DotBPE:DelayTask";


        internal string ZKeyCachekey => this.RedisCacheKey + ":ZKey";
        internal string ZItemCachekey => this.RedisCacheKey + ":ZItem";

        public string RedisPullLockingKey { get; set; } = "DotBPE:QUEUE:LKREDISPULL";


        public string DBPullLockingKey { get; set; } = "DotBPE:QUEUE:LKDBPULL";


        /// <summary>
        /// 失败重试最大次数 0不重试，默认：5
        /// </summary>
        public int RetryMaxCount { get; set; }

        private int[] _retryExceptionCode;
        /// <summary>
        /// 重试异常code 默认 -10242500
        /// </summary>
        public int[] RetryExceptionCode
        {
            get
            {
                if (_retryExceptionCode == null) _retryExceptionCode = new int[0];
                return _retryExceptionCode;
            }
            set { _retryExceptionCode = value; }
        }

        private int[] _retryStrategy;
        /// <summary>
        /// 失败重试延迟策略 单位：秒 （第二次以后要大于等于60秒）  默认失败次数对应值延迟时间[ 10, 60, 2 * 60, 5 * 60, 10 * 60, 20 * 60, 30 * 60 ];
        /// </summary>
        public int[] RetryStrategy
        {
            get
            {
                if (_retryStrategy == null || _retryStrategy.Length == 0) _retryStrategy = DefaultRetryStrategy;
                return _retryStrategy;
            }
            set { _retryStrategy = value; }
        }

    }
}
