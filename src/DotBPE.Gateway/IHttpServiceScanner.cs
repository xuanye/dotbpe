namespace DotBPE.Gateway
{
    public interface IHttpServiceScanner
    {
        HttpRouteOptions Scan(string dllPrefix="*",params string[] categories);

        HttpRouteOptions GetRuntimeRouteOptions();
    }
}
