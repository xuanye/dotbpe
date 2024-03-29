// Copyright (c) Xuanye Wong. All rights reserved.
// Licensed under MIT license

using DotBPE.Baseline.Extensions;
using DotBPE.Gateway.Internal;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using Microsoft.AspNetCore.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace DotBPE.Gateway.Swagger
{
    internal class HttpApiDescriptionProvider : IApiDescriptionProvider
    {
        private readonly EndpointDataSource _endpointDataSource;

        public HttpApiDescriptionProvider(EndpointDataSource endpointDataSource)
        {
            _endpointDataSource = endpointDataSource;
        }

        // Executes after ASP.NET Core
        public int Order => -900;

        public void OnProvidersExecuting(ApiDescriptionProviderContext context)
        {
            var endpoints = _endpointDataSource.Endpoints;

            foreach (var endpoint in endpoints)
            {
                if (endpoint is RouteEndpoint routeEndpoint)
                {
                    var rpcMetadata = endpoint.Metadata.GetMetadata<HttpApiMetadata>();

                    if (rpcMetadata != null)
                    {
                        var apiDescription = CreateApiDescription(routeEndpoint, rpcMetadata);
                        context.Results.Add(apiDescription);
                    }
                }
            }
        }

        private static ApiDescription CreateApiDescription(RouteEndpoint routeEndpoint, HttpApiMetadata rpcMetadata)
        {

            var handlerMethod = rpcMetadata.HanderMethod;
            var pattern = rpcMetadata.HttpApiOptions.Pattern;
            var verb = rpcMetadata.HttpApiOptions.AcceptVerb.ToString().ToUpper();

            var apiDescription = new ApiDescription
            {
                HttpMethod = verb
            };
            apiDescription.SetProperty(rpcMetadata);
            apiDescription.ActionDescriptor = new ActionDescriptor
            {
                DisplayName = handlerMethod.Name,
                RouteValues = new Dictionary<string, string>
                {
                    // Swagger uses this to group endpoints together.
                    // Group methods together using the service name.
                    ["controller"] = rpcMetadata.HanderServiceType.FullName
                }

            };
            apiDescription.RelativePath = pattern;

            if (!verb.Equals("get", System.StringComparison.OrdinalIgnoreCase))
            {
                apiDescription.SupportedRequestFormats.Add(new ApiRequestFormat { MediaType = "application/x-www-form-urlencoded" });
                apiDescription.SupportedRequestFormats.Add(new ApiRequestFormat { MediaType = "application/json" });
            }


            apiDescription.SupportedResponseTypes.Add(new ApiResponseType
            {
                ApiResponseFormats = { new ApiResponseFormat { MediaType = "application/json" } },
                Type = rpcMetadata.OutputType,
                ModelMetadata = new ApiModelMetadata(ModelMetadataIdentity.ForType(rpcMetadata.OutputType)),
                StatusCode = 200
            });

            HashSet<string> cache = new HashSet<string>();
            foreach (var routeParameter in routeEndpoint.RoutePattern.Parameters)
            {
                var name = routeParameter.Name.ToPascalCase();
                if (cache.Contains(name))
                {
                    continue;
                }
              
                var property = rpcMetadata.InputType.GetProperty(name);
                if (property == null)
                {
                    continue;
                }
                var modelMetadataIdentity = ModelMetadataIdentity.ForProperty(
                      property
                     , property.PropertyType
                     , rpcMetadata.InputType
                     );

                apiDescription.ParameterDescriptions.Add(new ApiParameterDescription
                {
                    Name = name.ToCamelCase(),
                    ModelMetadata = new ApiModelMetadata(modelMetadataIdentity),
                    Source = BindingSource.Path,
                    IsRequired = !routeParameter.IsOptional,
                    DefaultValue = routeParameter.Default
                });
                cache.Add(name);
            }


            var properties = rpcMetadata.InputType.GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);

            if (verb.Equals("get", StringComparison.OrdinalIgnoreCase))
            {
                foreach (var field in properties)
                {
                    if (cache.Contains(field.Name))
                    {
                        continue;
                    }
                    var modelMetadataIdentity = ModelMetadataIdentity.ForProperty(
                          field
                        , field.PropertyType
                        , rpcMetadata.InputType
                        );
                    apiDescription.ParameterDescriptions.Add(new ApiParameterDescription
                    {
                        Name = field.Name.ToCamelCase(),
                        ModelMetadata = new ApiModelMetadata(modelMetadataIdentity),
                        Source = BindingSource.Query,
                        IsRequired = false
                    });                   
                }
            }
            else
            {

                foreach (var field in properties)
                {
                    if (cache.Contains(field.Name))
                    {
                        continue;
                    }
                    var modelMetadataIdentity = ModelMetadataIdentity.ForProperty(
                          field
                        , field.PropertyType
                        , rpcMetadata.InputType
                        );
                    apiDescription.ParameterDescriptions.Add(new ApiParameterDescription
                    {
                        Name = field.Name.ToCamelCase(),
                        ModelMetadata = new ApiModelMetadata(modelMetadataIdentity),
                        Source = BindingSource.Form,
                        IsRequired = false
                    });                    
                }
            }

            return apiDescription;
        }

        public void OnProvidersExecuted(ApiDescriptionProviderContext context)
        {
            // no-op
        }

    }
}
