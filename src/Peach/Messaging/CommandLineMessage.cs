using System;
using System.Collections.Generic;
using System.Text;
using Peach.Infrastructure;

namespace Peach.Messaging
{
    public class CommandLineMessage : IMessage
    {
        /// <summary>
        /// get the current command name.
        /// </summary>
        public string Command { get; }

        /// <summary>
        /// Get Parameters
        /// </summary>
        public string[] Parameters { get; }

        public int Length {
            get
            {
                int length = 0;
                length = Encoding.UTF8.GetByteCount(Command);
                if(Parameters !=null && Parameters.Length > 0)
                {
                    for(var i=0; i < Parameters.Length; i++)
                    {
                        length += Encoding.UTF8.GetByteCount(Parameters[i]);
                    }
                }
                return length;
            }
        }

        /// <summary>
        /// new
        /// </summary>
        /// <param name="seqID"></param>
        /// <param name="cmdName"></param>
        /// <param name="parameters"></param>    
        public CommandLineMessage(string cmdName, params string[] parameters)
        {
            Preconditions.CheckNotNull(cmdName, nameof(cmdName));  
          
            Command = cmdName;
            Parameters = parameters;
        }
             
    }    
}
