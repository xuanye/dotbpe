// Copyright (c) Xuanye Wong. All rights reserved.
// Licensed under MIT license

using DotBPE.Rpc;
using FluentValidation;
using FluentValidation.Results;
using Google.Protobuf;
using System;

namespace DotBPE.BestPractice
{
    public static class ValidatorExtensions
    {
        public static string GetErrorMessage(this ValidationResult result)
        {
            var errorMsg = "";
            if (!result.IsValid)
            {
                foreach (var f in result.Errors)
                {
                    errorMsg += f.ErrorMessage;
                    errorMsg += "\n";
                }
            }

            return errorMsg;
        }

        public static RpcResult<TResponse> Validate<TResponse, TValidator>(this IMessage req, bool validateAuthenticated = false)

            where TResponse : class, IMessage, new() where TValidator : IValidator, new()
        {
            var result = new RpcResult<TResponse> { Data = new TResponse() };

            if (validateAuthenticated && !req.IsAuthenticated())
            {
                result.Code = BizErrorCodes.REQUIRED_AUTH_CODE;
                result.Data.SetValue(ProtocolsConstants.RETURN_MESSAGE_NUM, "Authorized access required");
                return result;
            }

            var validator = new TValidator();
            var context = new ValidationContext<IMessage>(req);
            var validationResult = validator.Validate(context);


            if (!validationResult.IsValid)
            {
                result.Code = BizErrorCodes.PARAM_FORMAT_WRONG_CODE;
                result.Data.SetValue(ProtocolsConstants.RETURN_MESSAGE_NUM,
                    validationResult.GetErrorMessage() ?? "Wrong parameter format");
            }

            return result;
        }

        public static RpcResult<TResponse> Validate<TResponse, TRequest>(this TRequest req, Action<CommonValidator<TRequest>> validateAction,
            bool validateAuthenticated = false)
            where TResponse : class, IMessage, new() where TRequest : IMessage, new()
        {
            var result = new RpcResult<TResponse> { Data = new TResponse() };

            if (validateAuthenticated && !req.IsAuthenticated())
            {
                result.Code = BizErrorCodes.REQUIRED_AUTH_CODE;
                result.Data.SetValue(ProtocolsConstants.RETURN_MESSAGE_NUM, "Authorized access required");
                return result;
            }

            var validator = new CommonValidator<TRequest>(validateAction);
            var validationResult = validator.Validate(req);


            if (!validationResult.IsValid)
            {
                result.Code = BizErrorCodes.PARAM_FORMAT_WRONG_CODE;
                result.Data.SetValue(ProtocolsConstants.RETURN_MESSAGE_NUM,
                    validationResult.GetErrorMessage() ?? "Wrong parameter format");
            }

            return result;
        }
    }

    public class NothingValidator : AbstractValidator<IMessage>
    {
    }
    public class CommonValidator<TReq> : AbstractValidator<TReq>
    {
        public CommonValidator(Action<CommonValidator<TReq>> action)
        {
            action(this);
        }
    }
}
