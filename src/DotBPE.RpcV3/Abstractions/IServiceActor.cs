// Copyright (c) Xuanye Wong. All rights reserved.
// Licensed under MIT license

namespace DotBPE.Rpc.Abstractions
{
    public interface IServiceActor
    {
        string Id {
            get;
        }

        string GroupName {
            get;
        }
    }

    public interface IServiceActor<TService> : IServiceActor where TService : class
    {

    }
}
