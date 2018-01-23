using DotBPE.Rpc;
using Survey.Core;
using Survey.Service.InnerImpl.Domain;
using System;
using System.Threading.Tasks;

namespace Survey.Service.InnerImpl
{
    public class UserInnerService : UserInnerServiceBase
    {
        private readonly Repository.UserRepository _userRepo;

        public UserInnerService(Repository.UserRepository userRepo)
        {
            this._userRepo = userRepo;
        }

        //修改用户信息
        public override async Task<RpcResult<EditUserRsp>> EditAsync(EditUserReq request)
        {
            var res = new RpcResult<EditUserRsp>();
            res.Data = new EditUserRsp();

            var isvalid = ValidateEditInfo(request, out string error);
            if (!isvalid)
            {
                res.Code = ErrorCodes.PARAMS_VALIDATION_FAIL;
                res.Data.ReturnMessage = error;
                return res;
            }

            if (request.Account != request.Identity && request.CheckRole)
            {
                res.Code = ErrorCodes.INVALID_OPERATION;
                res.Data.ReturnMessage = "非法操作";
                return res;
            }

            var user = await this._userRepo.GetUser(request.Account);
            if (user == null)
            {
                res.Code = ErrorCodes.DATA_NOT_FOUND;
                res.Data.ReturnMessage = "用户不存在";
                return res;
            }

            user.FullUpdate = false;
            if (!string.IsNullOrEmpty(request.OldPassword)) // 修改密码
            {
                string md5pwd = CryptographyManager.Md5Encrypt(request.Account + request.OldPassword);
                if (md5pwd != user.Password)
                {
                    res.Code = ErrorCodes.INVALID_OPERATION;
                    res.Data.ReturnMessage = "旧密码错误";
                    return res;
                }
                user.Password = CryptographyManager.Md5Encrypt(request.Account + request.NewPassword);
            }
            user.FullName = request.FullName;
            user.UpdateTime = DateTime.Now;

            await this._userRepo.UpdateAsync(user);

            return res;
        }

        public override async Task<RpcResult<GetUserRsp>> GetUserInfoAsync(GetUserReq request)
        {
            var res = new RpcResult<GetUserRsp>();
            res.Data = new GetUserRsp();

            if (string.IsNullOrEmpty(request.UserId))
            {
                res.Code = ErrorCodes.PARAMS_VALIDATION_FAIL;
                res.Data.ReturnMessage = "账号不能为空";
                return res;
            }
            var user = await this._userRepo.GetUser(request.UserId);

            if (user == null)
            {
                res.Code = ErrorCodes.DATA_NOT_FOUND;
                res.Data.ReturnMessage = "用户不存在";
                return res;
            }

            res.Data.Account = user.Account;
            res.Data.FullName = user.FullName;
            res.Data.IsAdmin = user.IsAdmin;
            return res;
        }

        /// <summary>
        /// 登录请求
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public override async Task<RpcResult<LoginRsp>> LoginAsync(LoginReq request)
        {
            var res = new RpcResult<LoginRsp>();
            res.Data = new LoginRsp();

            if (string.IsNullOrEmpty(request.Account))
            {
                res.Code = ErrorCodes.PARAMS_VALIDATION_FAIL;
                res.Data.ReturnMessage = "登录账号不能为空";
                return res;
            }

            if (string.IsNullOrEmpty(request.Password))
            {
                res.Code = ErrorCodes.PARAMS_VALIDATION_FAIL;
                res.Data.ReturnMessage = "登录密码不能为空";
                return res;
            }

            var user = await this._userRepo.GetUser(request.Account);

            if (user == null)
            {
                res.Code = ErrorCodes.DATA_NOT_FOUND;
                res.Data.ReturnMessage = "用户不存在";
                return res;
            }

            var md5pwd = CryptographyManager.Md5Encrypt(request.Account + request.Password);
            if (md5pwd == user.Password)
            {
                res.Data.Account = user.Account;
                res.Data.FullName = user.FullName;
                res.Data.IsAdmin = user.IsAdmin;
            }
            else
            {
                res.Code = ErrorCodes.BIZ_RULE_FAIL;
                res.Data.ReturnMessage = "用户名或者密码错误";
            }

            return res;
        }

        /// <summary>
        /// 注册用户
        /// </summary>
        /// <param name="request">注册用户的请求</param>
        /// <returns></returns>
        public override async Task<RpcResult<RegisterRsp>> RegisterAsync(RegisterReq request)
        {
            var res = new RpcResult<RegisterRsp>();
            var rsp = new RegisterRsp();

            bool isvalid = ValidateRegisterInfo(request, out string errorMsg);
            if (!isvalid)
            {
                res.Code = ErrorCodes.PARAMS_VALIDATION_FAIL;
                res.Data.ReturnMessage = errorMsg;
                return res;
            }

            var user = new UserInfo();
            user.Account = request.Account;
            user.FullName = request.FullName;
            user.Password = CryptographyManager.Md5Encrypt(user.Account + request.Password);
            user.IsAdmin = false;
            user.UpdateTime = DateTime.Now;
            user.CreateTime = DateTime.Now;
            var userId = await this._userRepo.InsertAsync(user);

            rsp.UserId = (int)userId;

            return res;
        }

        private bool ValidateEditInfo(EditUserReq req, out string error)
        {
            error = "";
            if (string.IsNullOrEmpty(req.Account))
            {
                error = "账号不能为空";
                return false;
            }
            if (string.IsNullOrEmpty(req.FullName))
            {
                error = "姓名不能为空";
                return false;
            }
            if (!Validator.IsStringLength(req.FullName, 1, 16))
            {
                error = "姓名太长了";
                return false;
            }

            if (!string.IsNullOrEmpty(req.OldPassword) && string.IsNullOrEmpty(req.NewPassword))
            {
                error = "新密码不能为空";
                return false;
            }

            if (string.IsNullOrEmpty(req.OldPassword) && !string.IsNullOrEmpty(req.NewPassword))
            {
                error = "请输入旧密码";
                return false;
            }
            return true;
        }

        private bool ValidateRegisterInfo(RegisterReq req, out string error)
        {
            error = "";

            if (string.IsNullOrEmpty(req.Account))
            {
                error = "账号不能为空";
                return false;
            }

            if (string.IsNullOrEmpty(req.Password))
            {
                error = "密码不能为空";
                return false;
            }
            if (string.IsNullOrEmpty(req.FullName))
            {
                error = "用户名称不能为空";
                return false;
            }

            if (!Validator.IsStringLength(req.FullName, 1, 16))
            {
                error = "用户名称太长了";
                return false;
            }

            if (!Validator.IsValidAccount(req.Account))
            {
                error = "账号格式不合法:字母开头，允许5-16字节，允许字母数字下划线";
                return false;
            }
            if (!Validator.IsValidPassword(req.Password))
            {
                error = "密码必须在6-18位之间";
                return false;
            }

            return true;
        }
    }
}
