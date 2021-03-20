using System;
using System.Collections.Generic;
using System.Text;

namespace DotBPE.BestPractice
{

    /// <summary>
    /// 一般业务异常
    /// </summary>
    public class BizException : Exception
    {
        public BizException()
        {

        }
        public BizException(int errorCode, string errorMessage) : base(errorMessage)
        {
            this.ErrorCode = errorCode;
        }

        public BizException(string errorMessage) : base(errorMessage)
        {

        }

        public BizException(string errorMessage, Exception inner) : base(errorMessage, inner)
        {

        }


        public BizException(int errorCode, string errorMessage, Exception inner) : base(errorMessage, inner)
        {
            this.ErrorCode = errorCode;
        }

        public int ErrorCode { get; }

    }
}
