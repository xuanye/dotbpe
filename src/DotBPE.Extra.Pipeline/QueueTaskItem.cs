using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using DotBPE.Baseline.Extensions;

namespace DotBPE.Extra
{
    [DisplayAttribute(Name = "relic-queue-task")]
    [DataContract]
    public class QueueTaskItem
    {
        public QueueTaskItem()
        {

        }
        public QueueTaskItem(int serviceId, ushort messageId, string executeJson, string routeKey = null, long taskId = 0, string checkCode = null)
        {
            this.TaskId = taskId;
            this.ServiceId = serviceId;
            this.MessageId = messageId;
            this.ExecuteJson = executeJson;
            this.ExecuteTime = DateTime.Now.ToUnixTimeSeconds();
            this.CheckCode = checkCode ?? Guid.NewGuid().ToString("N");
            this.FailCount = 0;
            this.RouteKey = routeKey;
        }
        public QueueTaskItem(IDelayTask from)
        {
            this.CheckCode = from.CheckCode ?? Guid.NewGuid().ToString("N");
            this.TaskId = from.TaskId;
            this.ServiceId = from.ServiceId;
            this.MessageId = from.MessageId;
            this.ExecuteJson = from.ExecuteJson;
            this.ExecuteTime = from.ExecuteTime.ToUnixTimeSeconds();
            this.FailCount = from.FailCount;
            this.RouteKey = from.RouteKey;
        }

        [DataMember(Name = "taskId", Order = 1)]
        public long TaskId { get; set; }


        /// <summary>
        /// 服务ID
        /// </summary>
        [DataMember(Name = "serviceId", Order = 2)]
        public int ServiceId { get; set; }


        /// <summary>
        /// 消息ID
        /// </summary>
        [DataMember(Name = "messageId", Order = 3)]
        public ushort MessageId { get; set; }

        /// <summary>
        /// 任务执行的数据，JSON格式，尽量简化，如只传递ID
        /// </summary>
        [DataMember(Name = "executeJson", Order = 4)]
        public string ExecuteJson { get; set; }

        /// <summary>
        /// 计划执行时间,时间戳 秒单位
        /// </summary>


        [DataMember(Name = "executeTime", Order = 5)]
        public long ExecuteTime { get; set; }

        [DataMember(Name = "checkCode", Order = 6)]
        public string CheckCode { get; set; }

        /// <summary>
        /// 失败次数
        /// </summary>
        [DataMember(Name = "failCount", Order = 7)]
        public int FailCount { get; set; }

        /// <summary>
        /// 路由key,null或空时不使用该key
        /// </summary>
        [DataMember(Name = "routeKey", Order = 8)]
       [KeyAttribute]
        public string RouteKey { get; set; }

    }
}
