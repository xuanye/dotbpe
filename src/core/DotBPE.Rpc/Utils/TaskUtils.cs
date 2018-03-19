using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace DotBPE.Rpc.Utils {
    /// <summary>
    /// Utility methods for task parallel library.
    /// </summary>
    public static class TaskUtils {
        /// <summary>
        /// Framework independent equivalent of <c>Task.CompletedTask</c>.
        /// </summary>
        public static Task CompletedTask {
            get {
#if DOTNETCORE
                return Task.CompletedTask;
#else
                return Task.FromResult<object> (null); // for .NET45, emulate the functionality
#endif
            }
        }

        public static void SafeSetResult<TResult> (TaskCompletionSource<TResult> promise, TResult result, ILogger logger) {
            if (!promise.TrySetResult (result)) {
                logger.LogWarning ($"Failed to set a promise's result  because it is done already: {promise}");
            }
        }

        /// <summary>
        ///     Marks the specified {@code promise} as failure.  If the {@code promise} is done already, log a message.
        /// </summary>
        public static void SafeSetFailure<TResult> (TaskCompletionSource<TResult> promise, Exception cause, ILogger logger) {
            if (!promise.TrySetException (cause)) {
                logger.LogWarning (cause, $"Failed to mark a promise as failure because it's done already: {promise}");
            }
        }
    }
}