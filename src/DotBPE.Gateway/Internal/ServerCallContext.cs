using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace DotBPE.Gateway.Internal
{
    public abstract class ServerCallContext
    {
        private Dictionary<object, object> userState;

        //
        // Summary:
        //     Name of method called in this RPC.
        public string Method => MethodCore;

        //
        // Summary:
        //     Name of host called in this RPC.
        public string Host => HostCore;

        //
        // Summary:
        //     Address of the remote endpoint in URI format.
        public string Peer => PeerCore;

        //
        // Summary:
        //     Deadline for this RPC. The call will be automatically cancelled once the deadline
        //     is exceeded.
        public DateTime Deadline => DeadlineCore;

     
        //
        // Summary:
        //     Cancellation token signals when call is cancelled. It is also triggered when
        //     the deadline is exceeeded or there was some other error (e.g. network problem).
        public CancellationToken CancellationToken => CancellationTokenCore;


    
        // Summary:
        //     Gets a dictionary that can be used by the various interceptors and handlers of
        //     this call to store arbitrary state.
        public IDictionary<object, object> UserState => UserStateCore;

        //
        // Summary:
        //     Provides implementation of a non-virtual public member.
        protected abstract string MethodCore
        {
            get;
        }

        //
        // Summary:
        //     Provides implementation of a non-virtual public member.
        protected abstract string HostCore
        {
            get;
        }

        //
        // Summary:
        //     Provides implementation of a non-virtual public member.
        protected abstract string PeerCore
        {
            get;
        }

        //
        // Summary:
        //     Provides implementation of a non-virtual public member.
        protected abstract DateTime DeadlineCore
        {
            get;
        }

      
        //
        // Summary:
        //     Provides implementation of a non-virtual public member.
        protected abstract CancellationToken CancellationTokenCore
        {
            get;
        }

     

        //
        // Summary:
        //     Provides implementation of a non-virtual public member.
        protected virtual IDictionary<object, object> UserStateCore
        {
            get
            {
                if (userState == null)
                {
                    userState = new Dictionary<object, object>();
                }

                return userState;
            }
        }

        //
        // Summary:
        //     Creates a new instance of ServerCallContext.
        protected ServerCallContext()
        {
        }


     

    }
}
