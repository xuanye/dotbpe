// Copyright (c) Xuanye Wong. All rights reserved.
// Licensed under MIT license

using DotBPE.Gateway.Internal;
using DotBPE.Gateway.Tests.TestObjects;
using DotBPE.Rpc;
using DotBPE.Rpc.Client;
using DotBPE.Rpc.Exceptions;
using DotBPE.Rpc.Server;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Primitives;
using Moq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace DotBPE.Gateway.Tests
{
    public class HttpApiCallHandlerTests
    {
        [Fact]
        public async Task HandleCallAsync_MatchingRouteValue_SetOnRequestMessage()
        {
            // Arrange
            Test1Req request = null;
            Task<RpcResult<Test1Rsp>> invoker(ITestService s, Test1Req req)
            {
                request = req;

                var result = new RpcResult<Test1Rsp>()
                {
                    Data = new Test1Rsp { Message = $"Hello {req.Name}" }
                };
                return Task.FromResult(result);
            }

            var httpApiCallHandler = CreateCallHandler<ITestService, Test1Req, Test1Rsp>(invoker);
            var httpContext = CreateHttpContext();
            httpContext.Request.RouteValues["Name"] = "TestName!";
            httpContext.Request.RouteValues["StringValue"] = "StringValue!";


            // Act
            await httpApiCallHandler.HandleCallAsync(httpContext);
            // Assert
            Assert.NotNull(request);
            Assert.Equal("TestName!", request!.Name);
            Assert.Equal("StringValue!", request!.StringValue);

            httpContext.Response.Body.Seek(0, SeekOrigin.Begin);
            using var responseJson = JsonDocument.Parse(httpContext.Response.Body);
            Assert.Equal("Hello TestName!", responseJson.RootElement.GetProperty("message").GetString());
        }

        [Fact]
        public async Task HandleCallAsync_MatchingRouteValueWithJsonName_SetOnRequestMessage()
        {
            // Arrange
            Test1Req request = null;
            Task<RpcResult<Test1Rsp>> invoker(ITestService s, Test1Req req)
            {
                request = req;

                var result = new RpcResult<Test1Rsp>()
                {
                    Data = new Test1Rsp { Message = $"Hello {req.Name}" }
                };
                return Task.FromResult(result);
            }

            var httpApiCallHandler = CreateCallHandler<ITestService, Test1Req, Test1Rsp>(invoker);
            var httpContext = CreateHttpContext();
            httpContext.Request.RouteValues["name"] = "TestName!";
            httpContext.Request.RouteValues["stringValue"] = "StringValue!";


            // Act
            await httpApiCallHandler.HandleCallAsync(httpContext);
            // Assert
            Assert.NotNull(request);
            Assert.Equal("TestName!", request!.Name);
            Assert.Equal("StringValue!", request!.StringValue);

            httpContext.Response.Body.Seek(0, SeekOrigin.Begin);
            using var responseJson = JsonDocument.Parse(httpContext.Response.Body);
            Assert.Equal("Hello TestName!", responseJson.RootElement.GetProperty("message").GetString());
        }


        [Fact]
        public async Task HandleCallAsync_MatchingQueryStringValues_SetOnRequestMessage()
        {

            // Arrange
            Test1Req request = null;
            Task<RpcResult<Test1Rsp>> invoker(ITestService s, Test1Req req)
            {
                request = req;

                var result = new RpcResult<Test1Rsp>()
                {
                    Data = new Test1Rsp { Message = $"Hello {req.Name}" }
                };
                return Task.FromResult(result);
            }

            var httpApiCallHandler = CreateCallHandler<ITestService, Test1Req, Test1Rsp>(invoker);
            var httpContext = CreateHttpContext();
            httpContext.Request.Query = new QueryCollection(new Dictionary<string, StringValues>
            {
                ["name"] = "TestName!",
                ["stringValue"] = "StringValue!"
            });

            // Act
            await httpApiCallHandler.HandleCallAsync(httpContext);
            // Assert
            Assert.NotNull(request);
            Assert.Equal("TestName!", request!.Name);
            Assert.Equal("StringValue!", request!.StringValue);

            httpContext.Response.Body.Seek(0, SeekOrigin.Begin);
            using var responseJson = JsonDocument.Parse(httpContext.Response.Body);
            Assert.Equal("Hello TestName!", responseJson.RootElement.GetProperty("message").GetString());

        }
        [Theory]
        [InlineData("post", "application/x-www-form-urlencoded")]
        [InlineData("put", "application/x-www-form-urlencoded")]
        [InlineData("patch", "application/x-www-form-urlencoded")]
        [InlineData("delete", "application/x-www-form-urlencoded")]
        [InlineData("post", "multipart/form-data")]
        [InlineData("put", "multipart/form-data")]
        [InlineData("patch", "multipart/form-data")]
        [InlineData("delete", "multipart/form-data")]
        public async Task HandleCallAsync_MatchingFormValuesWithJsonName_SetOnRequestMessage(string method, string contentType)
        {
            // Arrange
            Test1Req request = null;
            Task<RpcResult<Test1Rsp>> invoker(ITestService s, Test1Req req)
            {
                request = req;

                var result = new RpcResult<Test1Rsp>()
                {
                    Data = new Test1Rsp { Message = $"Hello {req.Name}" }
                };
                return Task.FromResult(result);
            }

            var httpApiCallHandler = CreateCallHandler<ITestService, Test1Req, Test1Rsp>(invoker);
            var httpContext = CreateHttpContext();

            httpContext.Request.Method = method;
            httpContext.Request.ContentType = contentType;
            httpContext.Request.Form = new FormCollection(new Dictionary<string, StringValues>()
            {
                ["name"] = "TestName!",
                ["stringValue"] = "StringValue!"
            });

            // Act
            await httpApiCallHandler.HandleCallAsync(httpContext);

            // Assert
            Assert.NotNull(request);
            Assert.Equal("TestName!", request!.Name);
            Assert.Equal("StringValue!", request!.StringValue);

            httpContext.Response.Body.Seek(0, SeekOrigin.Begin);
            using var responseJson = JsonDocument.Parse(httpContext.Response.Body);
            Assert.Equal("Hello TestName!", responseJson.RootElement.GetProperty("message").GetString());

        }

        [Fact]
        public async Task HandleCallAsync_SuccessfulResponse_WithoutDefaultValuesInResponseJson()
        {
            // Arrange
            Test1Req request = null;
            Task<RpcResult<Test1Rsp>> invoker(ITestService s, Test1Req req)
            {
                request = req;

                var result = new RpcResult<Test1Rsp>()
                {
                    Data = new Test1Rsp()
                };
                return Task.FromResult(result);
            }

            var httpApiCallHandler = CreateCallHandler<ITestService, Test1Req, Test1Rsp>(invoker);
            var httpContext = CreateHttpContext();

            httpContext.Request.Query = new QueryCollection(new Dictionary<string, StringValues>
            {
                ["name"] = "TestName!"
            });

            // Act
            await httpApiCallHandler.HandleCallAsync(httpContext);

            // Assert
            Assert.NotNull(request);
            Assert.Equal("TestName!", request!.Name);

            httpContext.Response.Body.Seek(0, SeekOrigin.Begin);
            using var reader = new StreamReader(httpContext.Response.Body);
            var responseJson = reader.ReadToEnd();

            Assert.Equal("{\"code\":0,\"message\":\"\",\"data\":{}}", responseJson);
        }


        [Theory]
        [InlineData("{malformed_json}", "Request JSON payload is not correctly formatted.")]
        [InlineData("{\"name\": 1234}", "Request JSON payload is not correctly formatted.")]
        public async Task HandleCallAsync_MalformedRequestBody_BadRequestReturned(string json, string expectedError)
        {
            // Arrange        
            Test1Req request = null;
            Task<RpcResult<Test1Rsp>> invoker(ITestService s, Test1Req req)
            {
                request = req;

                var result = new RpcResult<Test1Rsp>()
                {
                    Data = new Test1Rsp()
                };
                return Task.FromResult(result);
            }

            var httpApiCallHandler = CreateCallHandler<ITestService, Test1Req, Test1Rsp>(invoker);
            var httpContext = CreateHttpContext();
            httpContext.Request.Method = "post";
            httpContext.Request.Body = new MemoryStream(Encoding.UTF8.GetBytes(json));
            httpContext.Request.ContentType = "application/json";
            // Act
            await httpApiCallHandler.HandleCallAsync(httpContext);

            // Assert
            Assert.Equal((int)HttpStatusCode.BadRequest, httpContext.Response.StatusCode);

            httpContext.Response.Body.Seek(0, SeekOrigin.Begin);
            using var responseJson = JsonDocument.Parse(httpContext.Response.Body);
            Assert.Equal(expectedError, responseJson.RootElement.GetProperty("message").GetString());
            Assert.Equal((int)HttpStatusCode.BadRequest, responseJson.RootElement.GetProperty("code").GetInt32());
        }


        [Theory]
        [InlineData(null)]
        [InlineData("text/html")]
        public async Task HandleCallAsync_BadContentType_BadRequestReturned(string contentType)
        {
            // Arrange        
            Test1Req request = null;
            Task<RpcResult<Test1Rsp>> invoker(ITestService s, Test1Req req)
            {
                request = req;

                var result = new RpcResult<Test1Rsp>()
                {
                    Data = new Test1Rsp()
                };
                return Task.FromResult(result);
            }

            var httpApiCallHandler = CreateCallHandler<ITestService, Test1Req, Test1Rsp>(invoker);
            var httpContext = CreateHttpContext();
            httpContext.Request.Method = "post";
            httpContext.Request.Body = new MemoryStream(Encoding.UTF8.GetBytes("{}"));
            httpContext.Request.ContentType = contentType;
            // Act
            await httpApiCallHandler.HandleCallAsync(httpContext);

            // Assert
            Assert.Equal((int)HttpStatusCode.BadRequest, httpContext.Response.StatusCode);

            var expectedError = "Request content-type is invalid.";
            httpContext.Response.Body.Seek(0, SeekOrigin.Begin);
            using var responseJson = JsonDocument.Parse(httpContext.Response.Body);
            Assert.Equal(expectedError, responseJson.RootElement.GetProperty("message").GetString());
            Assert.Equal((int)HttpStatusCode.BadRequest, responseJson.RootElement.GetProperty("code").GetInt32());
        }


        [Fact]
        public async Task HandleCallAsync_RpcExceptionReturned_StatusReturned()
        {
            // Arrange  
            static Task<RpcResult<Test1Rsp>> invoker(ITestService s, Test1Req req)
            {
                return Task.FromException<RpcResult<Test1Rsp>>(new RpcException(StatusCodes.Status401Unauthorized, "Message!"));
            }

            var httpApiCallHandler = CreateCallHandler<ITestService, Test1Req, Test1Rsp>(invoker);
            var httpContext = CreateHttpContext();

            // Act
            await httpApiCallHandler.HandleCallAsync(httpContext);

            // Assert
            Assert.Equal(StatusCodes.Status401Unauthorized, httpContext.Response.StatusCode);

            httpContext.Response.Body.Seek(0, SeekOrigin.Begin);
            using var responseJson = JsonDocument.Parse(httpContext.Response.Body);
            Assert.Equal("Message!", responseJson.RootElement.GetProperty("message").GetString());
            Assert.Equal(StatusCodes.Status401Unauthorized, responseJson.RootElement.GetProperty("code").GetInt32());
        }

        [Fact]
        public async Task HandleCallAsync_RpcExceptionThrown_StatusReturned()
        {
            // Arrange  
            static Task<RpcResult<Test1Rsp>> invoker(ITestService s, Test1Req req)
            {
                throw new RpcException(StatusCodes.Status401Unauthorized, "Message!");
            }

            var httpApiCallHandler = CreateCallHandler<ITestService, Test1Req, Test1Rsp>(invoker);
            var httpContext = CreateHttpContext();

            // Act
            await httpApiCallHandler.HandleCallAsync(httpContext);

            // Assert
            Assert.Equal(StatusCodes.Status401Unauthorized, httpContext.Response.StatusCode);

            httpContext.Response.Body.Seek(0, SeekOrigin.Begin);
            using var responseJson = JsonDocument.Parse(httpContext.Response.Body);
            Assert.Equal("Message!", responseJson.RootElement.GetProperty("message").GetString());
            Assert.Equal(StatusCodes.Status401Unauthorized, responseJson.RootElement.GetProperty("code").GetInt32());
        }


        [Fact]
        public async Task HandleCallAsync_StatusSet_StatusReturned()
        {
            // Arrange  
            static Task<RpcResult<Test1Rsp>> invoker(ITestService s, Test1Req req)
            {
                var reslutl = new RpcResult<Test1Rsp>() { Code = StatusCodes.Status401Unauthorized, Data = new Test1Rsp() { Message = "Message!" } };

                return Task.FromResult(reslutl);
            }

            var httpApiCallHandler = CreateCallHandler<ITestService, Test1Req, Test1Rsp>(invoker);
            var httpContext = CreateHttpContext();

            // Act
            await httpApiCallHandler.HandleCallAsync(httpContext);

            // Assert
            Assert.Equal(StatusCodes.Status200OK, httpContext.Response.StatusCode);

            httpContext.Response.Body.Seek(0, SeekOrigin.Begin);
            using var responseJson = JsonDocument.Parse(httpContext.Response.Body);
            Assert.Equal("Message!", responseJson.RootElement.GetProperty("message").GetString());
            Assert.Equal(StatusCodes.Status401Unauthorized, responseJson.RootElement.GetProperty("code").GetInt32());
        }


        [Fact]
        public async Task HandleCallAsync_ExceptionThrown_StatusReturned()
        {
            // Arrange  
            static Task<RpcResult<Test1Rsp>> invoker(ITestService s, Test1Req req)
            {
                throw new InvalidOperationException("Exception!");
            }

            var httpApiCallHandler = CreateCallHandler<ITestService, Test1Req, Test1Rsp>(invoker);
            var httpContext = CreateHttpContext();

            // Act
            await httpApiCallHandler.HandleCallAsync(httpContext);

            // Assert
            Assert.Equal(StatusCodes.Status500InternalServerError, httpContext.Response.StatusCode);

            httpContext.Response.Body.Seek(0, SeekOrigin.Begin);
            using var responseJson = JsonDocument.Parse(httpContext.Response.Body);
            Assert.Equal("Exception!", responseJson.RootElement.GetProperty("message").GetString());
            Assert.Equal(StatusCodes.Status500InternalServerError, responseJson.RootElement.GetProperty("code").GetInt32());
        }

        [Fact]
        public async Task HandleCallAsync_DataTypes_SetOnRequestMessage()
        {
            // Arrange
            Test1Req request = null;
            Task<RpcResult<Test1Rsp>> invoker(ITestService s, Test1Req req)
            {
                request = req;

                var result = new RpcResult<Test1Rsp>()
                {
                    Data = new Test1Rsp()
                };
                return Task.FromResult(result);
            }

            var httpApiCallHandler = CreateCallHandler<ITestService, Test1Req, Test1Rsp>(invoker);
            var httpContext = CreateHttpContext();


            httpContext.Request.Query = new QueryCollection(new Dictionary<string, StringValues>
            {
                ["intValue"] = "1",
                ["longValue"] = "2",
                ["stringValue"] = "STRING!",
                ["floatValue"] = "11.1",
                ["doubleValue"] = "12.1",
                ["boolValue"] = "true",
                ["emumMessage"] = "FOO"
            });

            // Act
            await httpApiCallHandler.HandleCallAsync(httpContext);

            // Assert
            Assert.NotNull(request);
            Assert.Equal(1, request.IntValue);
            Assert.Equal(2, request.LongValue);

            Assert.Equal("STRING!", request.StringValue);
            Assert.Equal(11.1, request.FloatValue, 3);
            Assert.Equal(12.1, request.DoubleValue, 3);
            Assert.True(request.BoolValue);
            Assert.Equal(EmumMessage.FOO, request.EmumMessage);
        }



        [Fact]
        public async Task HandleCallAsync_DataTypesEnumNumber_SetOnRequestMessage()
        {
            // Arrange
            Test1Req request = null;
            Task<RpcResult<Test1Rsp>> invoker(ITestService s, Test1Req req)
            {
                request = req;

                var result = new RpcResult<Test1Rsp>()
                {
                    Data = new Test1Rsp()
                };
                return Task.FromResult(result);
            }

            var httpApiCallHandler = CreateCallHandler<ITestService, Test1Req, Test1Rsp>(invoker);
            var httpContext = CreateHttpContext();


            httpContext.Request.Query = new QueryCollection(new Dictionary<string, StringValues>
            {
                ["intValue"] = "1",
                ["longValue"] = "2",
                ["stringValue"] = "STRING!",
                ["floatValue"] = "11.1",
                ["doubleValue"] = "12.1",
                ["boolValue"] = "true",
                ["emumMessage"] = "1"
            });

            // Act
            await httpApiCallHandler.HandleCallAsync(httpContext);

            // Assert
            Assert.NotNull(request);
            Assert.Equal(1, request.IntValue);
            Assert.Equal(2, request.LongValue);

            Assert.Equal("STRING!", request.StringValue);
            Assert.Equal(11.1, request.FloatValue, 3);
            Assert.Equal(12.1, request.DoubleValue, 3);
            Assert.True(request.BoolValue);
            Assert.Equal(EmumMessage.FOO, request.EmumMessage);
        }


        [Fact]
        public async Task HandleCallAsync_ParseRequest_UseHttpRequestParsePlugin()
        {
            // arrange
            Test1Req request = null;
            Task<RpcResult<Test1Rsp>> invoker(ITestService s, Test1Req req)
            {
                request = req;

                var result = new RpcResult<Test1Rsp>()
                {
                    Data = new Test1Rsp()
                };
                return Task.FromResult(result);
            }

            var parsePlugin = new Mock<IHttpRequestParsePlugin>();
            parsePlugin.Setup(x => x.ParseAsync(It.IsAny<HttpRequest>())).Returns(
                () =>
                Task.FromResult(
                    (
                        (object)new Test1Req()
                        {
                            Name = "TestName2!",
                            SubMessage = new SubMessage()
                            {
                                SubField1 = "TestSubfield2!"
                            }
                        }
                        , StatusCodes.Status200OK,
                        "")
                    )
            );

            var httpApiOptions = new HttpApiOptions()
            {
                PluginName = typeof(IHttpRequestParsePlugin).AssemblyQualifiedName
            };

            var httpApiCallHandler = CreateCallHandler<ITestService, Test1Req, Test1Rsp>(invoker, httpApiOptions);

            var httpContext = CreateHttpContext((services) => services.AddScoped((_) => parsePlugin.Object));
            httpContext.Request.Query = new QueryCollection(new Dictionary<string, StringValues>
            {
                ["name"] = "TestName!"
            });

            // act      
            await httpApiCallHandler.HandleCallAsync(httpContext);


            // assert
            Assert.NotNull(request);
            Assert.Equal("TestName2!", request.Name);
            Assert.NotNull(request.SubMessage);
            Assert.Equal("TestSubfield2!", request.SubMessage.SubField1);

        }


        [Fact]
        public async Task HandleCallAsync_ReturnUnsuccessfulStatus_UseHttpRequestParsePlugin()
        {
            // arrange
            Test1Req request = null;
            Task<RpcResult<Test1Rsp>> invoker(ITestService s, Test1Req req)
            {
                request = req;

                var result = new RpcResult<Test1Rsp>()
                {
                    Data = new Test1Rsp()
                };
                return Task.FromResult(result);
            }
            var parsePlugin = new Mock<IHttpRequestParsePlugin>();
            parsePlugin.Setup(x => x.ParseAsync(It.IsAny<HttpRequest>())).Returns(
                () =>
                Task.FromResult(
                    (
                        (object)null
                        , StatusCodes.Status500InternalServerError,
                        "Internal Error.")
                    )
            );
            var httpApiOptions = new HttpApiOptions()
            {
                PluginName = typeof(IHttpRequestParsePlugin).AssemblyQualifiedName
            };

            var httpApiCallHandler = CreateCallHandler<ITestService, Test1Req, Test1Rsp>(invoker, httpApiOptions);

            var httpContext = CreateHttpContext((services) => services.AddScoped<IHttpRequestParsePlugin>((_) => parsePlugin.Object));
            httpContext.Request.Query = new QueryCollection(new Dictionary<string, StringValues>
            {
                ["name"] = "TestName!"
            });

            // act
            await httpApiCallHandler.HandleCallAsync(httpContext);

            // assert
            Assert.Equal(500, httpContext.Response.StatusCode);

            httpContext.Response.Body.Seek(0, SeekOrigin.Begin);
            using var responseJson = JsonDocument.Parse(httpContext.Response.Body);
            var message = responseJson.RootElement.GetProperty("message").GetString();
            var code = responseJson.RootElement.GetProperty("code").GetInt32();

            Assert.Equal("Internal Error.", message);
            Assert.Equal(StatusCodes.Status500InternalServerError, code);
        }

        [Fact]
        public async Task HandleCallAsync_ParseRequest_UseHttpRequestParsePostPlugin()
        {
            // arrange
            Test1Req request = null;
            Task<RpcResult<Test1Rsp>> invoker(ITestService s, Test1Req req)
            {
                request = req;

                var result = new RpcResult<Test1Rsp>()
                {
                    Data = new Test1Rsp()
                };
                return Task.FromResult(result);
            }

            var parsePlugin = new Mock<IHttpRequestParsePostPlugin>();
            parsePlugin.Setup(x => x.ParseAsync(It.IsAny<HttpRequest>(), It.IsAny<object>())).Returns(
                (HttpRequest _, object message) =>
                {
                    if (message is Test1Req reqMessage)
                    {
                        reqMessage.Name += " With UseHttpRequestParsePostPlugin";
                        reqMessage.SubMessage = new SubMessage()
                        {
                            SubField1 = "TestSubfield! With UseHttpRequestParsePostPlugin"
                        };

                    }

                    return Task.FromResult((StatusCodes.Status200OK, ""));
                }
             );

            var httpApiOptions = new HttpApiOptions()
            {
                PluginName = typeof(IHttpRequestParsePostPlugin).AssemblyQualifiedName
            };

            var httpApiCallHandler = CreateCallHandler<ITestService, Test1Req, Test1Rsp>(invoker, httpApiOptions);

            var httpContext = CreateHttpContext((services) => services.AddScoped((_) => parsePlugin.Object));
            httpContext.Request.Query = new QueryCollection(new Dictionary<string, StringValues>
            {
                ["name"] = "TestName!"
            });

            // act
            await httpApiCallHandler.HandleCallAsync(httpContext);

            // assert
            Assert.NotNull(request);
            Assert.Equal("TestName! With UseHttpRequestParsePostPlugin", request.Name);

            Assert.NotNull(request.SubMessage);

            Assert.Equal("TestSubfield! With UseHttpRequestParsePostPlugin", request.SubMessage.SubField1);

        }


        [Fact]
        public async Task HandleCallAsync_ReturnUnsuccessfulStatus_UseHttpRequestParsePostPlugin()
        {
            // arrange
            Test1Req request = null;
            Task<RpcResult<Test1Rsp>> invoker(ITestService s, Test1Req req)
            {
                request = req;

                var result = new RpcResult<Test1Rsp>()
                {
                    Data = new Test1Rsp()
                };
                return Task.FromResult(result);
            }

            var parsePlugin = new Mock<IHttpRequestParsePostPlugin>();
            parsePlugin.Setup(x => x.ParseAsync(It.IsAny<HttpRequest>(), It.IsAny<object>())).Returns(
                () =>
                Task.FromResult(
                        (
                            StatusCodes.Status500InternalServerError,
                            "Internal Error."
                        )
                    )
            );
            var httpApiOptions = new HttpApiOptions()
            {
                PluginName = typeof(IHttpRequestParsePostPlugin).AssemblyQualifiedName
            };


            var httpApiCallHandler = CreateCallHandler<ITestService, Test1Req, Test1Rsp>(invoker, httpApiOptions);

            var httpContext = CreateHttpContext((services) => services.AddScoped((_) => parsePlugin.Object));
            httpContext.Request.Query = new QueryCollection(new Dictionary<string, StringValues>
            {
                ["name"] = "TestName!",
                ["subMessage.subField"] = "TestSubfield!"
            });

            // act
            await httpApiCallHandler.HandleCallAsync(httpContext);

            // assert
            Assert.Equal(500, httpContext.Response.StatusCode);

            httpContext.Response.Body.Seek(0, SeekOrigin.Begin);
            using var responseJson = JsonDocument.Parse(httpContext.Response.Body);
            var message = responseJson.RootElement.GetProperty("message").GetString();
            var code = responseJson.RootElement.GetProperty("code").GetInt32();


            Assert.Equal("Internal Error.", message);
            Assert.Equal(StatusCodes.Status500InternalServerError, code);
        }


        [Fact]
        public async Task HandleCallAsync_RedactResponseMessage_UseHttpOutputProcessPlugin()
        {
            // arrange          
            static Task<RpcResult<Test1Rsp>> invoker(ITestService s, Test1Req req)
            {
                var result = new RpcResult<Test1Rsp>()
                {
                    Data = new Test1Rsp() { Message = "Test Message!" }
                };
                return Task.FromResult(result);
            }

            var parsePlugin = new Mock<IHttpOutputProcessPlugin>();
            parsePlugin.Setup(x => x.ProcessAsync(It.IsAny<HttpRequest>(), It.IsAny<HttpResponse>(), It.IsAny<Encoding>(), It.IsAny<RpcResult>())).Returns(
                (HttpRequest _, HttpResponse __, Encoding ___, RpcResult result) =>
                {
                    if (result is RpcResult<Test1Rsp> replyMessage && replyMessage.Data != null)
                    {
                        replyMessage.Data.Message += " Use HttpOutputProcessPlugin";
                    }

                    return Task.FromResult(false);
                }
             );

            var httpApiOptions = new HttpApiOptions()
            {
                PluginName = typeof(IHttpOutputProcessPlugin).AssemblyQualifiedName
            };


            var httpApiCallHandler = CreateCallHandler<ITestService, Test1Req, Test1Rsp>(invoker, httpApiOptions);
            var httpContext = CreateHttpContext((services) => services.AddScoped<IHttpOutputProcessPlugin>((_) => parsePlugin.Object));


            // act
            await httpApiCallHandler.HandleCallAsync(httpContext);

            // assert
            Assert.Equal(200, httpContext.Response.StatusCode);

            httpContext.Response.Body.Seek(0, SeekOrigin.Begin);
            using var responseJson = JsonDocument.Parse(httpContext.Response.Body);
            var message = responseJson.RootElement.GetProperty("message").GetString();

            Assert.Equal("Test Message! Use HttpOutputProcessPlugin", message);
        }


        [Fact]
        public async Task HandleCallAsync_ReplaceDefaultResponse_UseHttpOutputProcessPlugin()
        {
            // arrange          
            static Task<RpcResult<Test1Rsp>> invoker(ITestService s, Test1Req req)
            {
                var result = new RpcResult<Test1Rsp>()
                {
                    Data = new Test1Rsp() { Message = "Test Message!" }
                };
                return Task.FromResult(result);
            }

            var parsePlugin = new Mock<IHttpOutputProcessPlugin>();
            parsePlugin.Setup(x => x.ProcessAsync(It.IsAny<HttpRequest>(), It.IsAny<HttpResponse>(), It.IsAny<Encoding>(), It.IsAny<RpcResult>())).Returns(
                async (HttpRequest _, HttpResponse res, Encoding ___, RpcResult result) =>
                {
                    if (result is RpcResult<Test1Rsp> replyMessage && replyMessage.Data != null)
                    {
                        replyMessage.Data.Message += " Use HttpOutputProcessPlugin";
                        res.StatusCode = StatusCodes.Status200OK;
                        res.ContentType = "text/plain";
                        await res.WriteAsync(replyMessage.Data.Message);
                        return true;
                    }

                    return false;
                }
             );

            var httpApiOptions = new HttpApiOptions()
            {
                PluginName = typeof(IHttpOutputProcessPlugin).AssemblyQualifiedName
            };


            var httpApiCallHandler = CreateCallHandler<ITestService, Test1Req, Test1Rsp>(invoker, httpApiOptions);
            var httpContext = CreateHttpContext((services) => services.AddScoped<IHttpOutputProcessPlugin>((_) => parsePlugin.Object));


            // act
            await httpApiCallHandler.HandleCallAsync(httpContext);

            // assert
            Assert.Equal(200, httpContext.Response.StatusCode);

            httpContext.Response.Body.Seek(0, SeekOrigin.Begin);

            using var reader = new StreamReader(httpContext.Response.Body);
            var responseBody = reader.ReadToEnd();

            Assert.Equal("Test Message! Use HttpOutputProcessPlugin", responseBody);
        }


        [Fact]
        public async Task HandleCallAsync_PostProcessErrorResponse_UseHttpApiErrorProcess()
        {
            // arrange          
            static Task<RpcResult<Test1Rsp>> invoker(ITestService s, Test1Req req)
            {
                throw new InvalidOperationException("Exception!");
            }

            var process = new Mock<IHttpApiErrorProcess>();

            process.Setup(x => x.ProcessAsync(It.IsAny<HttpResponse>(), It.IsAny<Encoding>(), It.IsAny<Error>())).Returns(
                (HttpResponse _, Encoding __, Error error) =>
                {
                    error.Message = "Internal Error.";
                    return Task.FromResult(false);
                }
                );


            var httpApiCallHandler = CreateCallHandler<ITestService, Test1Req, Test1Rsp>(invoker);
            var httpContext = CreateHttpContext((services) => services.AddScoped((_) => process.Object));


            // act
            await httpApiCallHandler.HandleCallAsync(httpContext);

            // assert
            Assert.Equal(500, httpContext.Response.StatusCode);

            httpContext.Response.Body.Seek(0, SeekOrigin.Begin);
            using var responseJson = JsonDocument.Parse(httpContext.Response.Body);

            var message = responseJson.RootElement.GetProperty("message").GetString();
            var code = responseJson.RootElement.GetProperty("code").GetInt32();




            Assert.Equal("Internal Error.", message);
            Assert.Equal(StatusCodes.Status500InternalServerError, code);//Internal Error default code
        }

        [Fact]
        public async Task HandleCallAsync_HandlerErrorResponse_UseHttpApiErrorProcess()
        {
            // arrange          
            static Task<RpcResult<Test1Rsp>> invoker(ITestService s, Test1Req req)
            {
                throw new InvalidOperationException("Exception!");
            }

            var process = new Mock<IHttpApiErrorProcess>();

            process.Setup(x => x.ProcessAsync(It.IsAny<HttpResponse>(), It.IsAny<Encoding>(), It.IsAny<Error>())).Returns(
                async (HttpResponse res, Encoding __, Error error) =>
                {
                    error.Message = "Exception was thrown by handler.";
                    res.StatusCode = StatusCodes.Status500InternalServerError;
                    res.ContentType = "text/plain";
                    await res.WriteAsync(error.Message);
                    return true;
                }
                );

            var httpApiCallHandler = CreateCallHandler<ITestService, Test1Req, Test1Rsp>(invoker);
            var httpContext = CreateHttpContext((services) => services.AddScoped((_) => process.Object));


            // act
            await httpApiCallHandler.HandleCallAsync(httpContext);

            // assert
            Assert.Equal(500, httpContext.Response.StatusCode);

            httpContext.Response.Body.Seek(0, SeekOrigin.Begin);
            using var reader = new StreamReader(httpContext.Response.Body);
            var responseBody = reader.ReadToEnd();

            Assert.Equal("Exception was thrown by handler.", responseBody);
        }



        [Fact]
        public async Task HandleCallAsync_ReplaceDefaultResponse_UseHttpApiOutputProcess()
        {
            // arrange
            static Task<RpcResult<Test1Rsp>> invoker(ITestService s, Test1Req req)
            {
                var result = new RpcResult<Test1Rsp>()
                {
                    Data = new Test1Rsp() { Message = "Test Message!" }
                };
                return Task.FromResult(result);
            }


            var process = new Mock<IHttpApiOutputProcess>();
            process.Setup(x => x.ProcessAsync(It.IsAny<HttpRequest>(), It.IsAny<HttpResponse>(), It.IsAny<Encoding>(), It.IsAny<RpcResult>())).Returns(
                async (HttpRequest _, HttpResponse res, Encoding ___, RpcResult result) =>
                {
                    if (result is RpcResult<Test1Rsp> replyMessage && replyMessage.Data != null)
                    {

                        replyMessage.Data.Message += " Use UseHttpApiOutputProcess";
                        res.StatusCode = StatusCodes.Status200OK;
                        res.ContentType = "text/plain";
                        await res.WriteAsync(replyMessage.Data.Message);
                        return true;
                    }

                    return false;

                }
             );

            var httpApiCallHandler = CreateCallHandler<ITestService, Test1Req, Test1Rsp>(invoker);
            var httpContext = CreateHttpContext((services) => services.AddScoped((_) => process.Object));


            // act
            await httpApiCallHandler.HandleCallAsync(httpContext);

            // assert
            Assert.Equal(200, httpContext.Response.StatusCode);

            httpContext.Response.Body.Seek(0, SeekOrigin.Begin);

            using var reader = new StreamReader(httpContext.Response.Body);
            var responseBody = reader.ReadToEnd();

            Assert.Equal("Test Message! Use UseHttpApiOutputProcess", responseBody);

        }

        private static HttpApiCallHandler<TService, TRequest, TResponse> CreateCallHandler<TService, TRequest, TResponse>(ServiceMethod<TService, TRequest, TResponse> invoker
            , HttpApiOptions httpApiOptions = null)
            where TService : class
            where TRequest : class
            where TResponse : class
        {
            var gatewayOption = RpcGatewayOption.Default;
            var clientProxy = new Mock<IClientProxy>();
            var jsonParser = new DefaultJsonParser();
            var methodInvoker = new ApiMethodInvoker<TService, TRequest, TResponse>(invoker, null, 0, clientProxy.Object);
            var callHandler = new HttpApiCallHandler<TService, TRequest, TResponse>(gatewayOption, methodInvoker, jsonParser, httpApiOptions ?? new HttpApiOptions(), NullLoggerFactory.Instance);

            return callHandler;
        }
        private static DefaultHttpContext CreateHttpContext(Action<ServiceCollection> additionalServices = null, CancellationToken cancellationToken = default)
        {
            var serviceCollection = new ServiceCollection();

            //serviceCollection.BindService(typeof(TestService));

            additionalServices?.Invoke(serviceCollection);

            var serviceProvider = serviceCollection.BuildServiceProvider();
            var httpContext = new DefaultHttpContext();
            httpContext.Request.Host = new HostString("localhost");
            httpContext.RequestServices = serviceProvider;
            httpContext.Response.Body = new MemoryStream();
            httpContext.Connection.RemoteIpAddress = IPAddress.Parse("127.0.0.1");
            httpContext.Features.Set<IHttpRequestLifetimeFeature>(new HttpRequestLifetimeFeature(cancellationToken));
            return httpContext;
        }
        private class HttpRequestLifetimeFeature : IHttpRequestLifetimeFeature
        {
            public HttpRequestLifetimeFeature(CancellationToken cancellationToken)
            {
                RequestAborted = cancellationToken;
            }

            public CancellationToken RequestAborted { get; set; }

            public void Abort()
            {
            }
        }

    }
}
