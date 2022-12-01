// Copyright (c) Xuanye Wong. All rights reserved.
// Licensed under MIT license

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DotBPE.Rpc.Client
{
    public interface IClientProxy
    {
        Task<TService> CreateAsync<TService>(ushort specialMessageId = 0) where TService : class;

        TService Create<TService>(ushort specialMessageId = 0) where TService : class;
    }
}
