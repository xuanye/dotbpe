using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;

namespace DotBPE.ProtobufPlugin
{
    
    public static class DotBPEOptions
    {
        [SuppressMessage("ReSharper", "InconsistentNaming")]
        public const int SERVICE_ID = 51001;
        [SuppressMessage("ReSharper", "InconsistentNaming")]
        public const int DISABLE_GENERIC_SERVICES_CLIENT = 51003;
        [SuppressMessage("ReSharper", "InconsistentNaming")]
        public const int DISABLE_GENERIC_SERVICES_SERVER = 51004;
        [SuppressMessage("ReSharper", "InconsistentNaming")]
        public const int MESSAGE_ID = 51002;
        [SuppressMessage("ReSharper", "InconsistentNaming")]
        public const int GENERIC_MARKDOWN_DOC = 51005;
    }
}
