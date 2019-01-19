namespace PiggyMetrics.Common
{
    public class ServiceDiscoveryOption
    {
        public static ServiceDiscoveryOption Default = new ServiceDiscoveryOption();

        public int Interval { get; set; } = 50 ; //休息50毫秒
    }
}
