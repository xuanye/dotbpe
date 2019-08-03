using System.Net;

namespace Tomato.Baseline.Extensions
{
    public static class AddressExtensions
    {

        public static string ToIPV4(this IPAddress address)
        {
            if (address.IsIPv4MappedToIPv6)
            {
                return address.MapToIPv4().ToString();
            }

            return address.ToString();
        }
    }
}
