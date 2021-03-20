using Microsoft.Extensions.Hosting;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using StackExchange.Redis;

namespace DotBPE.Extra
{
    public class DBStoragePullService : BackgroundService
    {


        private readonly IDBStorage _dbStorage;
        private readonly IRedisStorage _redisStorage;
        private readonly ILogger<DBStoragePullService> _logger;
        private readonly PipelineOptions _options;
        private readonly IDatabase _redis;
        public DBStoragePullService(IOptions<PipelineOptions> optionsAccessor
            , IDBStorage dbStorage, IRedisStorage redisStorage,
            ConnectionMultiplexer multiplexer,
            ILogger<DBStoragePullService> logger)
        {

            this._dbStorage = dbStorage;
            this._redisStorage = redisStorage;
            this._logger = logger;
            this._options = optionsAccessor?.Value ?? new PipelineOptions();

            this._redis = multiplexer.GetDatabase();

        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            return Task.Run(async () =>
            {
                this._logger.LogInformation("Start Pulling DB...");
                while (!stoppingToken.IsCancellationRequested)
                {
                    bool locked = await this._redis.LockTakeAsync(this._options.DBPullLockingKey, 1, TimeSpan.FromSeconds(20));
                    if (!locked)
                    {
                        await Task.Delay(TimeSpan.FromSeconds(this._options.DbPullInterval), stoppingToken);
                        continue;
                    }

                    var list = await this._dbStorage.Pulling(DateTime.Now.AddSeconds(this._options.DbPullInterval));

                    if (list != null && list.Any())
                    {
                        this._logger.LogDebug("pulling {0} delay tasks from db", list.Count);
                        await this._redisStorage.Push(list);
                    }
                    else
                    {
                        this._logger.LogDebug("pulling 0 delay tasks from db");
                    }

                    await this._redis.LockReleaseAsync(this._options.DBPullLockingKey, 1); //释放锁

                    await Task.Delay(TimeSpan.FromSeconds(this._options.DbPullInterval), stoppingToken);
                }
                this._logger.LogInformation("End Pulling DB...");
            });
        }
    }
}
