using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using DotBPE.Baseline.Extensions;
using DotBPE.Gateway.Swagger.Generator;
using Newtonsoft.Json;

namespace DotBPE.Gateway.Swagger
{
    public class SwaggerApiInfoProvider:ISwaggerApiInfoProvider
    {
        private readonly IHttpServiceScanner _httpScanner;

        private string _swaggerJson = string.Empty;

        private XmlCommentResolver _resolver;

        public SwaggerApiInfoProvider(IHttpServiceScanner httpScanner)
        {
            this._httpScanner = httpScanner;
        }

        public void ScanApiInfo(Action<SwaggerConfig> configAction =null)
        {
            SwaggerConfig config = new SwaggerConfig();

            configAction?.Invoke(config);

            _resolver = new XmlCommentResolver(config.XmlComments);
            //所有路由信息
            var routeOptions = this._httpScanner.GetRuntimeRouteOptions();
            // config 包含配置和XML的注释信息

            SwaggerInfo swagger = new SwaggerInfo
            {
                Host = config.Host,
                Info = config.ApiInfo??new SwaggerApiInfo(),
                BasePath = config.BasePath,
                Paths = new Dictionary<string, Dictionary<string, SwaggerMethod>>(),
                Definitions = new Dictionary<string, SwaggerDefinition>()
            };

            ProcessPaths(swagger.Paths, routeOptions, swagger.Definitions);

            var settings = new JsonSerializerSettings
            {
                DefaultValueHandling = DefaultValueHandling.Ignore
            };

            this._swaggerJson = JsonConvert.SerializeObject(swagger,Formatting.Indented, settings);

        }

        private void ProcessPaths(Dictionary<string, Dictionary<string, SwaggerMethod>> swaggerPaths,
            HttpRouteOptions routeOptions,Dictionary<string, SwaggerDefinition> definitions)
        {
            routeOptions.Items.ForEach(item =>
            {
                var path = CreateSwaggerMethod(item.AcceptVerb);
                path.Tags = new List<string> {
                    item.Category??"default",
                    this._resolver.GetTypeComment(item.InvokeMethod.DeclaringType)
                    ,item.InvokeMethod.DeclaringType.Name
                };

                path.Summary = _resolver.GetMethodComment(item.InvokeMethod);

                string verb;
                //path.Summary = item.AcceptVerb.ToString();
                if (item.AcceptVerb == RestfulVerb.Any || item.AcceptVerb == RestfulVerb.Get
                    || item.AcceptVerb != RestfulVerb.UnKnown)
                {

                    //path.Consumes = new List<string> {"application/json"};
                    verb = "get";
                }
                else
                {
                    path.Consumes = new List<string> {"application/x-www-form-urlencoded","application/json"};
                    verb = "post";
                }

                path.Description =  path.Summary;

                path.Parameters = new List<SwaggerApiParameters>();

                ProcessParameters(item.AcceptVerb ,path.Parameters, item.InvokeMethod.GetParameters(),definitions);

                path.Responses = new Dictionary<string, SwaggerApiResponse>();

                ProcessResponses(path.Responses, item.InvokeMethod.ReturnParameter,definitions);

                swaggerPaths.Add(item.Path,new Dictionary<string, SwaggerMethod> {{verb,path}});
            });
        }

        private void ProcessResponses(Dictionary<string, SwaggerApiResponse> pathResponses,
            ParameterInfo invokeMethodReturnParameter,Dictionary<string, SwaggerDefinition> definitions)
        {
            //Task<RpcResult>
            if (!invokeMethodReturnParameter.ParameterType.IsGenericType)
            {
                return;
            }
            var rpcResultType = invokeMethodReturnParameter.ParameterType.GenericTypeArguments[0];
            if (!rpcResultType.IsGenericType) //Task<RpcResult>
            {
                return;
            }
            //Task<RpcResult<T>>
            var innerType = rpcResultType.GenericTypeArguments[0];

            SwaggerApiResponse apiResponse = new SwaggerApiResponse
            {
                Description = this._resolver.GetTypeComment(innerType),
                Schema = GetSwaggerItemSchema(innerType,definitions)
            };

            pathResponses.Add("200",apiResponse);

            CreateSwaggerDefinition(innerType.Name, innerType, definitions);
        }

        private void CreateSwaggerDefinition(string name,Type definitionType,Dictionary<string, SwaggerDefinition> definitions)
        {
            if (definitions.ContainsKey(name))
            {
                return;
            }

            SwaggerDefinition definition = new SwaggerDefinition
            {
                Type = "object",
                Properties =new Dictionary<string, SwaggerPropertyDefinition>()
            };


            var properties = definitionType.GetProperties();

            properties.ForEach(p =>
            {
                SwaggerPropertyDefinition pd = new SwaggerPropertyDefinition
                {
                    Description = this._resolver.GetMemberInfoComment(p)
                };

                if (p.PropertyType == typeof(string) || p.PropertyType.IsValueType )
                {
                    pd.Type =  GetSwaggerType(p.PropertyType);
                }
                else if(p.PropertyType.IsArray && p.PropertyType.HasElementType)
                {
                    pd.Type = "array";
                    pd.Items = GetSwaggerItemSchema(p.PropertyType.GetElementType(),definitions);
                }
                else
                {
                    pd.Ref = "#/definitions/" + p.PropertyType.Name;
                    CreateSwaggerDefinition(p.PropertyType.Name, p.PropertyType, definitions);
                }

                definition.Properties.Add(p.Name.ToCamelCase(),pd);
            });

            definitions.Add(name,definition);
        }

        private void ProcessParameters(RestfulVerb verb,List<SwaggerApiParameters> pathParameters, ParameterInfo[] getParameters
            ,Dictionary<string, SwaggerDefinition> definitions)
        {
            var parameter = getParameters[0];
            var properties = parameter.ParameterType.GetProperties();

            properties.ForEach(p =>
            {
                SwaggerApiParameters apiParameter = new SwaggerApiParameters
                {
                    Name = p.Name.ToCamelCase(),
                    Description = this._resolver.GetMemberInfoComment(p)
                };

                if (p.PropertyType == typeof(string) || p.PropertyType.IsValueType )
                {
                    apiParameter.Type = GetSwaggerType(p.PropertyType);
                    apiParameter.In = verb == RestfulVerb.Any || verb == RestfulVerb.Get ? "query" : "formData";
                }
                else
                {
                    apiParameter.In = verb == RestfulVerb.Any || verb == RestfulVerb.Get ? "query" : "body";
                    apiParameter.Schema = GetSwaggerItemSchema(p.PropertyType,definitions);
                }
                pathParameters.Add(apiParameter);
            });

        }

        private SwaggerItemSchema GetSwaggerItemSchema(Type type,Dictionary<string, SwaggerDefinition> definitions)
        {

            if (type.IsArray && type.HasElementType)
            {
                SwaggerArrayItemSchema arrayItem = new SwaggerArrayItemSchema();
                arrayItem.Items.Add(new SwaggerSingleItemSchema
                {
                    Ref = "#/definitions/"+type.GetElementType().Name
                });
                CreateSwaggerDefinition(type.GetElementType().Name,type.GetElementType(),definitions);
                return arrayItem;
            }

            SwaggerSingleItemSchema singleItem = new SwaggerSingleItemSchema {Ref = "#/definitions/" + type.Name};

            CreateSwaggerDefinition(type.Name,type,definitions);

            return singleItem;
        }


        private SwaggerMethod CreateSwaggerMethod(RestfulVerb verb)
        {
            SwaggerMethod m;
            switch (verb)
            {
                case  RestfulVerb.Get:
                    m = new SwaggerGetMethod();
                    break;
                case  RestfulVerb.Post:
                    m = new SwaggerPostMethod();
                    break;
                case  RestfulVerb.Put:
                    m = new SwaggerPutMethod();
                    break;
                case  RestfulVerb.Delete:
                    m = new SwaggerDeleteMethod();
                    break;
                case  RestfulVerb.Patch:
                    m = new SwaggerPatchMethod();
                    break;
                default:
                    m = new SwaggerGetMethod();
                    break;
            }
            return m;
        }
        public string GetSwaggerApiJson()
        {
            return _swaggerJson;
        }

        private string GetSwaggerType(Type type)
        {
            if (type == typeof(string))
            {
                return "string";
            }

            if (type == typeof(int) || type == typeof(long) || type == typeof(short) || type.IsEnum)
            {
                return "integer";
            }
            if (type == typeof(bool))
            {
                return "boolean";
            }

            if (type == typeof(float) || type == typeof(decimal) || type == typeof(long) || type == typeof(short))
            {
                return "number";
            }

             return "string";

        }

    }
}
