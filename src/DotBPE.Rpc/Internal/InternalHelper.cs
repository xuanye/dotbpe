namespace DotBPE.Rpc.Internal
{
    public static class InternalHelper
    {
        public static string FormatServiceIdentity(string serviceGroupName, ushort serviceId, ushort messageId)
        {
            return $"{serviceGroupName}.{serviceId}.{messageId}";
        }

        public static string FormatServicePath(ushort serviceId, ushort messageId)
        {
            return $"{serviceId}.{messageId}";
        }
    }
}
