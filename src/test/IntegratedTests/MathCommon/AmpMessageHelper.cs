using DotBPE.Protocol.Amp;
using Google.Protobuf;
using System;
using System.Collections.Generic;
using System.Text;

namespace MathCommon
{

    public class AmpMessageHelper
    {
        static readonly JsonFormatter Formatter = new JsonFormatter(new JsonFormatter.Settings(false).WithFormatEnumsAsIntegers(true));

        public static string Stringify(AmpMessage message)
        {
            string json = "";

            if(message == null)
            {
                return null;
            }

            if (message.Code == 0)
            {                              
                var rspTemp = ProtobufObjectFactory.GetResponseTemplate(message.ServiceId, message.MessageId);
                if (rspTemp == null)
                {
                    return null;
                }

                if (message.Data != null)
                {
                    rspTemp.MergeFrom(message.Data);
                }                
                json = Formatter.Format(rspTemp);                         
            }           
            return json;
        }
    }
}
