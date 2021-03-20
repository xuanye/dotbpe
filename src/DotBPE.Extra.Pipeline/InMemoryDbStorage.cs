using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DotBPE.Baseline.Extensions;

namespace DotBPE.Extra
{
    /// <summary>
    /// 测试用勿在生产环境使用
    /// </summary>
    public class InMemoryDbStorage : IDBStorage
    {
        private readonly List<DelayTask> _taskList = new List<DelayTask>();

        private readonly object _lockObj = new object();


        public Task<List<IDelayTask>> Pulling(DateTime pullTime)
        {
            var retLst = new List<IDelayTask>();

            lock (this._lockObj)
            {
                foreach (var item in _taskList)
                {
                    if (item.ExecuteTime <= pullTime && (item.Status==1 || item.Status==9) && item.FailCount < 5)
                    {
                        item.Status = 8;
                        retLst.Add(item);
                    }
                }
            }

            return Task.FromResult(retLst);
        }

        public Task<long> AddDelayTask(IDelayTask delayTask)
        {
            long oldTaskId = 0;
            if (!(delayTask is DelayTask tItem))
            {
                return Task.FromResult(oldTaskId);
            }

            if (!string.IsNullOrEmpty(delayTask.CheckCode))
            {
                lock (this._lockObj)
                {
                    var cTask = this._taskList.Find(x => x.CheckCode == delayTask.CheckCode);
                    if (cTask != null)
                    {
                        tItem.UpdateTime = DateTime.Now;
                        oldTaskId = cTask.TaskId;
                        this._taskList.Remove(cTask);
                    }
                }
            }

            tItem.TaskId = oldTaskId > 0 ? oldTaskId : DateTime.Now.ToUnixTimeSeconds();

            lock (this._lockObj)
            {
                this._taskList.Add(tItem);
            }

            return Task.FromResult(tItem.TaskId);
        }

        public Task Failed(long taskId, int errorCount, TimeSpan nextRetryTimeSpan)
        {
            lock (this._lockObj)
            {
                var cTask = this._taskList.Find(x => x.TaskId == taskId);
                cTask.Status = 9;
                cTask.FailCount += errorCount;
                cTask.ExecuteTime = DateTime.Now.Add(nextRetryTimeSpan);
            }



            return Task.CompletedTask;
        }

        public Task Success(long taskId)
        {
            lock (this._lockObj)
            {
                var cTask = this._taskList.Find(x => x.TaskId == taskId);
                //this._taskList.Remove(cTask);
                cTask.Status = 2;
            }

            return Task.CompletedTask;
        }

        public Task<int> CancelTask(string checkCode)
        {
            lock (this._lockObj)
            {
                var cTask = this._taskList.Find(x => x.CheckCode == checkCode);
                this._taskList.Remove(cTask);
            }

            return Task.FromResult(1);
        }
    }
}
