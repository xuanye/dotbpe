using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DotBPE.Extra
{
    public interface IRedisStorage
    {
        Task Push(List<IDelayTask> list);
        Task<List<QueueTaskItem>> Pulling(DateTime pullTime);
        Task<int> CancelTask(string checkCode);

        Task<int> Success(string checkCode);

    }
}
