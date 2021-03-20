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
                foreach (var f in result.Errors)
                {
                    errorMsg += f.ErrorMessage;
                    errorMsg += "\n";
                }

            return errorMsg;
        }

        /// <summary>
        ///     验证消息请求,
        /// </summary>
        /// <typeparam name="TRes">响应的类型</typeparam>
        /// <typeparam name="TValidator">验证器的类型</typeparam>
        /// <param name="result">返回结果</param>
        /// <param name="req">请求参数</param>
        /// <param name="validateAuthenticated">是否校验登录状态</param>
        /// <returns>返回校验后的消息，如果result.Code 则校验通过，否则直接返回错误信息</returns>
        public static RpcResult<TRes> Validate<TRes, TValidator>(this IMessage req, bool validateAuthenticated = false)
            where TRes : IMessage, new() where TValidator : IValidator, new()
        {
            var result = new RpcResult<TRes> { Data = new TRes() };

            if (validateAuthenticated && !req.IsAuthenticated())
            {
                result.Code = BizErrorCodes.REQUIRED_AUTH_CODE;
                result.Data.SetValue(ProtocolsConstants.RETURN_MESSAGE_NUM, "请先登录");
                return result;
            }

            var validator = new TValidator();
            var context = new ValidationContext<IMessage>(req);
            var validationResult = validator.Validate(context);


            if (!validationResult.IsValid)
            {
                result.Code = BizErrorCodes.PARAM_FORMAT_WRONG_CODE;
                result.Data.SetValue(ProtocolsConstants.RETURN_MESSAGE_NUM,
                    validationResult.GetErrorMessage() ?? "意外错误");
            }

            return result;
        }

        public static RpcResult<TRes> Validate<TRes, TReq>(this TReq req, Action<CommonValidator<TReq>> validateAction,
            bool validateAuthenticated = false)
            where TRes : IMessage, new() where TReq : IMessage, new()
        {
            var result = new RpcResult<TRes> { Data = new TRes() };

            if (validateAuthenticated && !req.IsAuthenticated())
            {
                result.Code = BizErrorCodes.REQUIRED_AUTH_CODE;
                result.Data.SetValue(ProtocolsConstants.RETURN_MESSAGE_NUM, "请先登录");
                return result;
            }

            var validator = new CommonValidator<TReq>(validateAction);
            var validationResult = validator.Validate(req);


            if (!validationResult.IsValid)
            {
                result.Code = BizErrorCodes.PARAM_FORMAT_WRONG_CODE;
                result.Data.SetValue(ProtocolsConstants.RETURN_MESSAGE_NUM,
                    validationResult.GetErrorMessage() ?? "意外错误");
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
