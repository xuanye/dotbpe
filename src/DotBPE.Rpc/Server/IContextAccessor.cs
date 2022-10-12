// Copyright (c) Xuanye Wong. All rights reserved.
// Licensed under MIT license

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace DotBPE.Rpc.Server
{
    public interface IContextAccessor
    {
        ICallContext CallContext { get; set; }
    }

    public class DefaultContextAccessor : IContextAccessor
    {
        private static readonly AsyncLocal<ICallContext> _callContextCurrent = new AsyncLocal<ICallContext>();

        public ICallContext CallContext
        {
            get
            {
                return _callContextCurrent.Value;
            }

            set
            {
                _callContextCurrent.Value = value;
            }
        }
    }
}
