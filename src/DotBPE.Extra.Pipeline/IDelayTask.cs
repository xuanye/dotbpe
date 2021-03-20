using System;

namespace DotBPE.Extra
{

    public interface IDelayTask
    {
        /// <summary>
        /// 任务ID
        /// </summary>
        long TaskId { get; set; }

        /// <summary>
        /// 任务代码，用于标识一类任务
        /// </summary>
        string TaskCode { get; }

        /// <summary>
        /// 检查码，用于修改/取消延时任务
        /// </summary>
        string CheckCode { get; }


        /// <summary>
        /// 任务描述
        /// </summary>
        string Description { get; }

        /// <summary>
        /// 服务ID
        /// </summary>
        int ServiceId { get; }


        /// <summary>
        /// 消息ID
        /// </summary>
        ushort MessageId { get; }

        /// <summary>
        /// 任务执行的数据，JSON格式，尽量简化，如只传递ID
        /// </summary>
        string ExecuteJson { get; }

        /// <summary>
        /// 计划执行时间
        /// </summary>
        DateTime ExecuteTime { get; }

        /// <summary>
        /// 任务状态 1= 有效 0=失效 2=已执行 9=执行失败
        /// </summary>
        byte Status { get; set; }

        /// <summary>
        /// 失败次数
        /// </summary>
        int FailCount { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        DateTime CreateTime { get; }

        /// <summary>
        /// 最后更新时间
        /// </summary>
        DateTime UpdateTime { get; }

        string RouteKey { get; }
    }

    public class DelayTask : IDelayTask
    {
        public long TaskId { get; set; }
        public string TaskCode { get; set; }
        public string CheckCode { get; set; }
        public string Description { get; set; }
        public int ServiceId { get; set; }
        public ushort MessageId { get; set; }
        public string ExecuteJson { get; set; }
        public DateTime ExecuteTime { get; set; }
        public byte Status { get; set; }

        /// <summary>
        /// 失败次数
        /// </summary>
        public int FailCount { get; set; }

        public DateTime CreateTime { get; set; }
        public DateTime UpdateTime { get; set; }

        public string RouteKey { get; set; }
    }

}
