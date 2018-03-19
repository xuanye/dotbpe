using System;

namespace DotBPE.Rpc.Utils {
    public static class IdUtils {
        public static string NewId () {
            return Guid.NewGuid ().ToString ("N");
        }
    }
}