namespace Survey.Service
{
    public class Constants
    {
        public const string MAIN_DB_KEY = "master";
    }

    public class ErrorCodes
    {
        //Parameter Validation fails
        public const int PARAMS_VALIDATION_FAIL = -1001001; //参数校验不通过

        public const int BIZ_RULE_FAIL = -1001999; //一般业务规则错误

        //Authorization Required
        public const int AUTHORIZATION_REQUIRED = -1001401; //需要登录后访问

        public const int DATA_NOT_FOUND = -1001404; //数据不存在，或者没有权限查看

        //invalid operation
        public const int INVALID_OPERATION = -1001501; //无效操作

        //Internal error;
        public const int INTERNAL_ERROR = -1001500; //内部错误
    }
}
