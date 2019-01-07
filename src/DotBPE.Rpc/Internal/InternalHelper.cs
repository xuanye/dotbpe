namespace DotBPE.Rpc.Internal
{
    public static class InternalHelper
    {
        public static string FormatServiceIdentity(string serviceGroupName, int serviceId, ushort messageId)
        {
            return $"{serviceGroupName}.{serviceId}.{messageId}";
        }

        public static string FormatServicePath(int serviceId, ushort messageId)
        {
            return $"{serviceId}.{messageId}";
        }
    }
}
