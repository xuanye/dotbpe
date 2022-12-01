// Copyright (c) Xuanye Wong. All rights reserved.
// Licensed under MIT license

using DotBPE.Baseline.Extensions;
using DotBPE.Gateway.Internal;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Options;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace DotBPE.Gateway.Swagger
{
    public class DefaultSwaggerProvider : ISwaggerProvider
    {
        private static SwaggerInfo _swaggerInfoInstance;

        private readonly IApiDescriptionGroupCollectionProvider _apiDescriptionGroup;
        private readonly SwaggerOptions _config;
        private XmlCommentResolver _resolver;

        public DefaultSwaggerProvider(IApiDescriptionGroupCollectionProvider apiDescriptionGroup, IOptions<SwaggerOptions> optionsAccessor)
        {
            _apiDescriptionGroup = apiDescriptionGroup;
            _config = optionsAccessor?.Value ?? new SwaggerOptions();
        }

        public SwaggerInfo GetSwaggerInfo()
        {
            if (_swaggerInfoInstance != null)
            {
                return _swaggerInfoInstance;
            }
            InitialSwaggerInfo();
            return _swaggerInfoInstance;
        }

        private void InitialSwaggerInfo()
        {
            _swaggerInfoInstance = new SwaggerInfo
            {
                Host = _config.Host,
                Info = _config.ApiInfo ?? new SwaggerApiInfo(),
                BasePath = _config.BasePath ?? "/",
                Paths = new Dictionary<string, Dictionary<string, SwaggerMethod>>(),
                Definitions = new Dictionary<string, SwaggerDefinition>(),
                Tags = new List<SwaggerTag>()
            };

            var applicableApiDescriptions = _apiDescriptionGroup.ApiDescriptionGroups.Items
              .SelectMany(group => group.Items);

            var firstApi = applicableApiDescriptions.FirstOrDefault();
            if (firstApi == null)
            {
                return;
            }
            var rpcHttpMetadata = firstApi.GetProperty<HttpApiMetadata>();
            if (rpcHttpMetadata == null)
            {
                return;
            }
            var firstMethod = rpcHttpMetadata.HanderMethod;

            if (_resolver == null)
            {
                var xmlFile = $"{firstMethod.DeclaringType.Assembly.GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                _resolver = new XmlCommentResolver(new List<string>() { xmlPath });
            }
            GenerateApiPaths(applicableApiDescriptions);
        }

        private void GenerateApiPaths(IEnumerable<ApiDescription> applicableApiDescriptions)
        {
            foreach (var apiDescription in applicableApiDescriptions)
            {
                GenerateApiPath(apiDescription);
            }
        }

        private void GenerateApiPath(ApiDescription apiDescription)
        {
            var swagger = _swaggerInfoInstance;

            var rpcHttpMetadata = apiDescription.GetProperty<HttpApiMetadata>();

            var tagName = rpcHttpMetadata.HanderServiceType.Name;

            if (!_swaggerInfoInstance.Tags.Exists(x => x.Name == tagName))
            {
                _swaggerInfoInstance.Tags.Add(new SwaggerTag
                {
                    Name = tagName,
                    Description = _resolver.GetTypeComment(rpcHttpMetadata.HanderServiceType)
                });
            }

            var acceptVerb = apiDescription.HttpMethod.ToLower();
            var pathMethod = DefaultSwaggerProvider.CreateSwaggerMethod(acceptVerb);

            pathMethod.Tags = new List<string> { tagName };

            pathMethod.Version = !string.IsNullOrEmpty(rpcHttpMetadata.HttpApiOptions.Version) ? rpcHttpMetadata.HttpApiOptions.Version : "1.0";
            pathMethod.Summary = $"{tagName}.{rpcHttpMetadata.HanderMethod.Name}";


            apiDescription.SupportedRequestFormats.ForEach(requestFormat => pathMethod.Consumes.Add(requestFormat.MediaType));


            pathMethod.OperationId = rpcHttpMetadata.HanderMethod.Name;

            pathMethod.Description = _resolver.GetMethodComment(rpcHttpMetadata.HanderMethod);

            //parameters
            apiDescription.ParameterDescriptions.ForEach(paramter =>
            {

                var name = paramter.ModelMetadata.Name;
                if (_config.IngoreFields.Contains(name))
                {
                    return;
                }
                var p = paramter.ModelMetadata.ContainerType.GetProperty(name, BindingFlags.Public | BindingFlags.Instance);

                if (p == null)
                {
                    return;
                }
                var apiParameter = new SwaggerApiParameters
                {
                    Name = paramter.Name,
                    Required = paramter.IsRequired,
                    DefaultValue = paramter.DefaultValue?.ToString(),
                    Description = _resolver.GetMemberInfoComment(p)
                };

                if (p.PropertyType == typeof(string) || p.PropertyType.IsValueType)
                {
                    var (typeName, format) = GetSwaggerType(p.PropertyType);
                    apiParameter.Type = typeName;
                    apiParameter.Format = format;
                    apiParameter.In = DefaultSwaggerProvider.ConvertBindSource(paramter.Source);
                }
                else
                {
                    apiParameter.In = "body";
                    apiParameter.Schema = GetSwaggerItemSchema(p.PropertyType);
                }
                pathMethod.Parameters.Add(apiParameter);
            });

            //response          
            apiDescription.SupportedResponseTypes.ForEach(apiResponse =>
            {
                apiResponse.ApiResponseFormats.ForEach(fomart =>
                {
                    pathMethod.Produces.Add(fomart.MediaType);

                });

                var responseType = apiResponse.Type;
                var swaggerApiResponse = new SwaggerApiResponse
                {
                    Description = _resolver.GetTypeComment(responseType),
                    Schema = GetSwaggerItemSchema(responseType)
                };
                pathMethod.Responses.Add(apiResponse.StatusCode.ToString(), swaggerApiResponse);
            });

            if (!swagger.Paths.ContainsKey(apiDescription.RelativePath))
            {
                swagger.Paths.Add(apiDescription.RelativePath, new Dictionary<string, SwaggerMethod> { { acceptVerb, pathMethod } });
            }
            else
            {
                var pathItem = swagger.Paths[apiDescription.RelativePath];
                if (!pathItem.ContainsKey(acceptVerb))
                {
                    pathItem.Add(acceptVerb, pathMethod);
                }
            }
        }

        private SwaggerItemSchema GetSwaggerItemSchema(Type type)
        {

            if (type.IsArray && type.HasElementType)
            {
                var arrayItem = new SwaggerArrayItemSchema();
                arrayItem.Items.Add(new SwaggerSingleItemSchema
                {
                    Ref = "#/definitions/" + type.GetElementType().Name
                });
                CreateSwaggerDefinition(type.GetElementType().Name, type.GetElementType());
                return arrayItem;
            }

            if (type == typeof(string) || type.IsValueType) //string or valueType
            {
                var (typeName, _) = GetSwaggerType(type);


                var valueItem = new SwaggerSingleItemSchema { Ref = typeName };

                CreateSwaggerDefinition(type.Name, type);

                return valueItem;
            }

            if (
               IsCollectionType(type)
            )
            {
                var arrayItem = new SwaggerArrayItemSchema();

                if (type.IsGenericType)
                {
                    arrayItem.Items.Add(new SwaggerSingleItemSchema
                    {
                        Ref = "#/definitions/" + type.GenericTypeArguments[0].Name
                    });
                    CreateSwaggerDefinition(type.GenericTypeArguments[0].Name, type.GenericTypeArguments[0]);

                }
                else
                {
                    arrayItem.Items.Add(new SwaggerSingleItemSchema
                    {
                        Ref = "#/definitions/Object"
                    });
                }
                return arrayItem;
            }

            var singleItem = new SwaggerSingleItemSchema { Ref = "#/definitions/" + type.Name };

            CreateSwaggerDefinition(type.Name, type);

            return singleItem;
        }

        private bool IsCollectionType(Type type)
        {
            return type.GetInterface(nameof(ICollection)) != null;
        }

        private void CreateSwaggerDefinition(string name, Type definitionType)
        {
            if (_swaggerInfoInstance.Definitions.ContainsKey(name))
            {
                return;
            }
            if (definitionType == typeof(string) || definitionType.IsValueType) //string or valueType
            {
                return;
            }

            if ("Type".Equals(name))
            {
                return;
            }

            SwaggerDefinition definition = new SwaggerDefinition
            {
                Type = "object",
                Properties = new Dictionary<string, SwaggerPropertyDefinition>()
            };


            _swaggerInfoInstance.Definitions.Add(name, definition);

            var properties = definitionType.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly);

            properties.ForEach(p =>
            {
                SwaggerPropertyDefinition pd = new SwaggerPropertyDefinition
                {
                    Description = _resolver.GetMemberInfoComment(p)
                };

                if (p.PropertyType == typeof(string) || p.PropertyType.IsValueType)
                {
                    var (type, format) = GetSwaggerType(p.PropertyType);
                    pd.Type = type;
                    pd.Format = format;
                }
                else if (p.PropertyType.IsArray && p.PropertyType.HasElementType)
                {
                    pd.Type = "array";
                    pd.Items = GetSwaggerItemSchema(p.PropertyType.GetElementType());
                }
                else if (
                   IsCollectionType(p.PropertyType)
                    )
                {

                    if (p.PropertyType.IsGenericType)
                    {
                        pd.Type = "array";
                        pd.Items = GetSwaggerItemSchema(p.PropertyType.GenericTypeArguments[0]);
                    }
                    else
                    {
                        pd.Type = "array";
                    }
                }
                else
                {
                    //Console.WriteLine(p.PropertyType.Name);
                    pd.Ref = "#/definitions/" + p.PropertyType.Name;
                    CreateSwaggerDefinition(p.PropertyType.Name, p.PropertyType);
                }

                definition.Properties.Add(p.Name.ToCamelCase(), pd);
            });


        }


        private (string, string) GetSwaggerType(Type type)
        {

            if (type == typeof(string))
            {
                return ("string", null);
            }

            if (type == typeof(int) || type == typeof(uint) || type.IsEnum)
            {
                return ("integer", "int32");
            }
            if (type == typeof(long) || type == typeof(ulong))
            {
                return ("integer", "int64");
            }
            if (type == typeof(short) || type == typeof(ushort))
            {
                return ("integer", "int16");
            }

            if (type == typeof(bool))
            {
                return ("boolean", null);
            }

            if (type == typeof(float) || type == typeof(decimal) || type == typeof(double))
            {
                return ("number", null);
            }

            if (type.IsGenericType) //NullAble valueType
            {
                return GetSwaggerType(type.GetGenericArguments()[0]);
            }

            return ("string", null);

        }

        private static string ConvertBindSource(BindingSource source)
        {
            if (source == BindingSource.Path)
            {
                return "path";
            }
            else if (source == BindingSource.Form)
            {
                return "formData";
            }

            return "query";
        }

        private static SwaggerMethod CreateSwaggerMethod(string verb)
        {
            switch (verb)
            {
                case "get":
                    return new SwaggerGetMethod();
                case "post":
                    return new SwaggerPostMethod();
                case "delete":
                    return new SwaggerDeleteMethod();
                case "put":
                    return new SwaggerPutMethod();
                case "patch":
                    return new SwaggerPatchMethod();
                default:
                    return new SwaggerGetMethod();

            }
        }

    }
}
