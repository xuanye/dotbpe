// Copyright (c) Xuanye Wong. All rights reserved.
// Licensed under MIT license

using Peach.Messaging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DotBPE.Rpc.Abstractions
{

    /// <summary>
    /// 服务定位器，实现者需要根据message中的内容，准确的找到具体的IServiceActor
    /// </summary>  
    public interface IServiceActorLocator
    {
        /// <summary>
        /// Locating a ServiceActor by Id
        /// </summary>
        /// <param name="actorId">service actor id</param>
        /// <returns>IServiceActor</returns>
        IServiceActor LocateServiceActor(string actorId);

    }
}
