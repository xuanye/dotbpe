namespace DotBPE.Gateway
{
    public class DefaultHttpMetricFactory:IHttpMetricFactory
    {
        public IHttpMetric Create()
        {
            return new NullHttpMetric();
        }
    }
}
