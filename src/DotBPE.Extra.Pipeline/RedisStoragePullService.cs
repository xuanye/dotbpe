using Microsoft.Extensions.Hosting;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Foundatio.Messaging;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using StackExchange.Redis;

namespace DotBPE.Extra
{
    public class RedisStoragePullService : BackgroundService
    {
        private readonly IRedisStorage _redisStorage;
        private readonly IDBStorage _dbStorage;
        private readonly IMessageBus _queueBus;
        private readonly ILogger<RedisStoragePullService> _logger;
        private readonly PipelineOptions _options;
        private readonly IDatabase _redis;

        public RedisStoragePullService(IOptions<PipelineOptions> optionsAccessor, IRedisStorage redisStorage
            , IDBStorage dbStorage, IMessageBus queueBus,
            ConnectionMultiplexer multiplexer,
            ILogger<RedisStoragePullService> logger)
        {


            _redisStorage = redisStorage;
            _dbStorage = dbStorage;
            _queueBus = queueBus;
            _logger = logger;
            _options = optionsAccessor?.Value ?? new PipelineOptions();

            _redis = multiplexer.GetDatabase();
        }


        protected override  Task ExecuteAsync(CancellationToken stoppingToken)
        {
           return  Task.Run(async() =>
            {


                this._logger.LogInformation("Start Pulling Redis...");
                while (!stoppingToken.IsCancellationRequested)
                {
                    bool locked = await _redis.LockTakeAsync(_options.RedisPullLockingKey, 1, TimeSpan.FromSeconds(2));
                    if (!locked)
                    {
                        await Task.Delay(TimeSpan.FromSeconds(_options.RedisPullIntervalMS), stoppingToken).ConfigureAwait(false);
                        continue;
                    }

                    try
                    {
                        await PullingTask(stoppingToken).ConfigureAwait(false);
                    }
                    catch (TaskCanceledException)
                    {
                        _logger.LogInformation("Pulling Redis Task  Cancel");
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Pulling Redis Error...");
                        await Task.Delay(TimeSpan.FromSeconds(this._options.RedisPullIntervalMS), stoppingToken).ConfigureAwait(false);
                    }
                }
                this._logger.LogInformation("End Pulling Redis...");
            });
        }


        private async Task PullingTask(CancellationToken stoppingToken)
        {
            var list = await this._redisStorage.Pulling(DateTime.Now);

            if (list?.Count > 0)
            {
                _logger.LogDebug("pulling {0} delay tasks from redis", list.Count);
                foreach (var item in list)
                {
                    await _queueBus.PublishAsync(item);
                    await _dbStorage.Success(item.TaskId);
                    await _redisStorage.Success(item.CheckCode);
                }
            }
            else
            {
                this._logger.LogDebug("pulling 0 delay tasks from redis");
            }
            await _redis.LockReleaseAsync(_options.RedisPullLockingKey, 1); //释放锁

            await Task.Delay(TimeSpan.FromMilliseconds(_options.RedisPullIntervalMS), stoppingToken);
        }
    }
}
