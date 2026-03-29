using System;
using Microsoft.Extensions.Hosting;

namespace DotBPE.Extensions
{
    public static class HostBuilderExtensions
    {
        public static IHostBuilder UseJsonNetSerializer(this IHostBuilder @this)
        {
            return @this.ConfigureServices(services => services.AddJsonNetSerializer());
        }

    }
}

namespace DotBPE.Extra
{
    /// <summary>
    /// HostBuilderExtensions is obsolete. Use DotBPE.Extensions.HostBuilderExtensions instead.
    /// </summary>
    [Obsolete("Use DotBPE.Extensions.HostBuilderExtensions instead")]
    public static class HostBuilderExtensions
    {
        /// <summary>
        /// UseJsonNetSerializer is obsolete. Use DotBPE.Extensions.HostBuilderExtensions.UseJsonNetSerializer instead.
        /// </summary>
        [Obsolete("Use DotBPE.Extensions.HostBuilderExtensions.UseJsonNetSerializer instead")]
        public static IHostBuilder UseJsonNetSerializer(this IHostBuilder @this)
        {
            return DotBPE.Extensions.HostBuilderExtensions.UseJsonNetSerializer(@this);
        }
    }
}
