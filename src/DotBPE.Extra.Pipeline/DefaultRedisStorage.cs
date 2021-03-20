using DotBPE.Baseline.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace DotBPE.Extra
{
    public class DefaultRedisStorage : IRedisStorage
    {
        private readonly ILogger<DefaultRedisStorage> _logger;
        private readonly PipelineOptions _options;
        private readonly IDatabase _redis;

        public DefaultRedisStorage(IOptions<PipelineOptions> optionsAccessor,
            ConnectionMultiplexer multiplexer,
            ILogger<DefaultRedisStorage> logger)
        {
            _logger = logger;
            _options = optionsAccessor?.Value ?? new PipelineOptions();
            _redis = multiplexer.GetDatabase();
        }

        public async Task Push(List<IDelayTask> list)
        {
            if (list == null || !list.Any())
                return;

            SortedSetEntry[] setList = new SortedSetEntry[list.Count];
            KeyValuePair<RedisKey, RedisValue>[] kvpList = new KeyValuePair<RedisKey, RedisValue>[list.Count];
            for (var i = 0; i < list.Count; i++)
            {
                var item = new QueueTaskItem(list[i]);

                var data = JsonSerializer.Serialize(item);

                _logger.LogDebug("PUSH序列化数据：{0}", data);

                var checkCode = item.CheckCode;

                var score = list[i].ExecuteTime.ToUnixTimeSeconds();
                setList[i] = new SortedSetEntry(checkCode, score);

                var itemKey = _options.ZItemCachekey + ":" + checkCode;
                kvpList[i] = new KeyValuePair<RedisKey, RedisValue>(itemKey, data);
            }
            var trans = _redis.CreateTransaction();

            var t1 = trans.SortedSetAddAsync(_options.ZKeyCachekey, setList);
            var t2 = trans.StringSetAsync(kvpList);

            await trans.ExecuteAsync();
            await t1;
            await t2;
        }

        public async Task<List<QueueTaskItem>> Pulling(DateTime pullTime)
        {
            List<QueueTaskItem> list = new List<QueueTaskItem>();
            long timestamp = pullTime.ToUnixTimeSeconds();

            _logger.LogTrace("拉取Redis时间戳:{0}", timestamp);
            var trans = _redis.CreateTransaction();

            var t1 = trans.SortedSetRangeByScoreAsync(_options.ZKeyCachekey, 0, timestamp);
            var t2 = trans.SortedSetRemoveRangeByScoreAsync(_options.ZKeyCachekey, 0, timestamp);

            var tx = trans.ExecuteAsync();
            bool exec = false;
            try
            {
                exec = await tx;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "call redis error");
            }

            if (exec) //执行成功
            {
                var cacheValues = await t1;

                await t2;

                this._logger.LogTrace("拉取到Redis的数据为:{0}", cacheValues?.Length);
                if (cacheValues == null || cacheValues.Length == 0) return list;

                RedisKey[] keys = new RedisKey[cacheValues.Length];
                for (var i = 0; i < keys.Length; i++)
                {
                    keys[i] = _options.ZItemCachekey + ":" + cacheValues[i];
                }

                var itemValues = await _redis.StringGetAsync(keys);

                foreach (var cv in itemValues)
                {
                    if (cv.HasValue) //有值
                    {
                        _logger.LogDebug("拉取Redis数据", cv);
                        list.Add(JsonSerializer.Deserialize<QueueTaskItem>(cv));
                    }
                }
                //list.AddRange(from cv in itemValues where cv.HasValue select JsonSerializer.Deserialize<QueueTaskItem>(cv));
            }

            return list;
        }

        public async Task<int> CancelTask(string checkCode)
        {
            var trans = _redis.CreateTransaction();

            var t1 = trans.SortedSetRemoveAsync(_options.ZKeyCachekey, checkCode);

            var key = _options.ZItemCachekey + ":" + checkCode;

            var t2 = trans.KeyExpireAsync(key, TimeSpan.FromMilliseconds(10));

            var ret = await trans.ExecuteAsync();
            await t1;
            await t2;

            return ret ? 1 : 0;
        }

        public async Task<int> Success(string checkCode)
        {
            var key = _options.ZItemCachekey + ":" + checkCode;

            bool has = await _redis.KeyExpireAsync(key, TimeSpan.FromMilliseconds(10));

            return has ? 1 : 0;
        }
    }
}
