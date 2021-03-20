using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DotBPE.Extra
{
    public interface IDBStorage
    {
        Task<List<IDelayTask>> Pulling(DateTime pullTime);
        Task<long> AddDelayTask(IDelayTask delayTask);
        Task Failed(long taskId,int errorCount,TimeSpan nextRetryTimeSpan);

        Task Success(long taskId);
        Task<int> CancelTask(string checkCode);
    }
}
