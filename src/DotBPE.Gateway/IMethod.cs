using System;
using System.Collections.Generic;
using System.Text;

namespace DotBPE.Gateway
{
    public interface IMethod
    {
        //
        // Summary:
        //     Gets the name of the service to which this method belongs.
        string ServiceName
        {
            get;
        }

        //
        // Summary:
        //     Gets the unqualified name of the method.
        string Name
        {
            get;
        }

        //
        // Summary:
        //     Gets the fully qualified name of the method. On the server side, methods are
        //     dispatched based on this name.
        string FullName
        {
            get;
        }
    }
}
