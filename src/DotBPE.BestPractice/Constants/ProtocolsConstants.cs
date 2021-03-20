using System;
using System.Collections.Generic;
using System.Text;

namespace DotBPE.BestPractice
{
    public static class ProtocolsConstants
    {
        public const int RETURN_MESSAGE_NUM = 1;

        public static HashSet<string> FieldMaskList = new HashSet<string>()
        {
            "password",
            "idcard"
        };
    }
}
