// Copyright (c) Xuanye Wong. All rights reserved.
// Licensed under MIT license

using Microsoft.AspNetCore.Builder;
using System;
using System.Collections.Generic;

namespace DotBPE.Gateway.Internal
{
    internal class ApiEndpointConventionBuilder : IEndpointConventionBuilder
    {
        private readonly IEnumerable<IEndpointConventionBuilder> _endpointConventionBuilders;

        public ApiEndpointConventionBuilder(IEnumerable<IEndpointConventionBuilder> endpointConventionBuilders)
        {
            _endpointConventionBuilders = endpointConventionBuilders;
        }

        public void Add(Action<EndpointBuilder> convention)
        {
            foreach (var endpointConventionBuilder in _endpointConventionBuilders)
            {
                endpointConventionBuilder.Add(convention);
            }
        }
    }
}
