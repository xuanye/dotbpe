using System;
using System.Diagnostics;

namespace DotBPE.Rpc.Utils
{
    /// <summary>
    /// Utility methods to run microbenchmarks.
    /// </summary>
    public static class BenchmarkUtil
    {
        /// <summary>
        /// Runs a simple benchmark preceded by warmup phase.
        /// </summary>
        public static void RunBenchmark(int warmupIterations, int benchmarkIterations, Action action)
        {
            var logger = Environment.Logger;

            logger.Info("Warmup iterations: {0}", warmupIterations);
            for (int i = 0; i < warmupIterations; i++)
            {
                action();
            }

            logger.Info("Benchmark iterations: {0}", benchmarkIterations);
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            for (int i = 0; i < benchmarkIterations; i++)
            {
                action();
            }
            stopwatch.Stop();
            logger.Info("Elapsed time: {0}ms", stopwatch.ElapsedMilliseconds);
            logger.Info("Ops per second: {0}", (int)((double)benchmarkIterations * 1000 / stopwatch.ElapsedMilliseconds));
        }
    }
}
