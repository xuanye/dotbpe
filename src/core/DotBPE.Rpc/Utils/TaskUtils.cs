
using System;
using System.Threading.Tasks;

namespace DotBPE.Rpc.Utils
{
    /// <summary>
    /// Utility methods for task parallel library.
    /// </summary>
    public static class TaskUtils
    {
        /// <summary>
        /// Framework independent equivalent of <c>Task.CompletedTask</c>.
        /// </summary>
        public static Task CompletedTask
        {
            get
            {
#if NETCOREAPP1_1
                return Task.CompletedTask;
#else
                return Task.FromResult<object>(null);  // for .NET45, emulate the functionality
#endif
            }
        }
    }
}
