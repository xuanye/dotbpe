using Google.Protobuf;
using Microsoft.Extensions.Caching.Distributed;
using Survey.Core;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Survey.Core
{
    public interface ILoginService
    {
        Task SetSessionAsync(SessionUser user);

        Task<SessionUser> GetSessionUserAsync(string sessionId);

        Task RefreshSessionAsync(string sessionId);

        Task RemoveSessionAsync(string sessionId);
    }


    public class LoginService : ILoginService
    {
        private static readonly string PREX = "BPESESS:";
        private readonly IDistributedCache _cache;
        public LoginService(IDistributedCache cache)
        {
            _cache = cache;
        }

        public Task RefreshSessionAsync(string sessionId)
        {
            return _cache.RefreshAsync(PREX + sessionId);
        }

        public async Task<SessionUser> GetSessionUserAsync(string sessionId)
        {
            var buf = await this._cache.GetAsync(PREX + sessionId);
            if(buf !=null && buf.Length > 0)
            {
                return SessionUser.Parser.ParseFrom(buf);
            }

            return null;

        }

        public Task SetSessionAsync(SessionUser user)
        {
            var option = new DistributedCacheEntryOptions()
            {
               SlidingExpiration  = TimeSpan.FromMinutes(20)
            };
            return _cache.SetAsync(PREX + user.BpeSessionId, user.ToByteArray(), option);
        }

        public Task RemoveSessionAsync(string sessionId)
        {
            return _cache.RemoveAsync(PREX + sessionId);
        }
    }
}
