using System;
using System.Diagnostics;
using Tomato.Rpc.Protocol;
using Peach;

namespace Tomato.Rpc.Diagnostics
{
    public static class TomatoDiagnosticListenerExtensions
    {
        public static readonly DiagnosticListener Listener = new DiagnosticListener(TomatoDiagnosticListenerName);

        public const string TomatoDiagnosticListenerName = "TomatoDiagnosticListener";
        public const string TomatoDiagnosticServiceActorReceiveRequest = "Tomato.Rpc.ServiceActor.ReceiveRequest";
        public const string TomatoDiagnosticServiceActorSendResponse = "Tomato.Rpc.ServiceActor.SendResponse";
        public const string TomatoDiagnosticServiceActorException = "Tomato.Rpc.ServiceActor.Exception";
        public const string TomatoDiagnosticClientSendRequest = "Tomato.Rpc.Client.SendRequest";
        public const string TomatoDiagnosticClientReceiveResponse = "Tomato.Rpc.Client.ReceiveResponse";
        public const string TomatoDiagnosticClientException = "Tomato.Rpc.Client.Exception";

        public static void ServiceActorReceiveRequest(this DiagnosticListener listener,
            ISocketContext<AmpMessage> context, AmpMessage request)
        {
            if (listener.IsEnabled(TomatoDiagnosticServiceActorReceiveRequest))
            {
                listener.Write(TomatoDiagnosticServiceActorReceiveRequest, new
                {
                    Context = context,
                    Request = request
                });
            }
        }

        public static void ServiceActorSendResponse(this DiagnosticListener listener,
            ISocketContext<AmpMessage> context, AmpMessage request,AmpMessage response)
        {
            if (listener.IsEnabled(TomatoDiagnosticServiceActorSendResponse))
            {
                listener.Write(TomatoDiagnosticServiceActorSendResponse, new
                {
                    Context = context,
                    Request = request,
                    Response = response
                });
            }
        }

        public static void ServiceActorException(this DiagnosticListener listener,ISocketContext<AmpMessage> context
            ,AmpMessage request,Exception exception)
        {
            if (listener.IsEnabled(TomatoDiagnosticServiceActorException))
            {
                listener.Write(TomatoDiagnosticServiceActorException, new
                {
                    Context = context,
                    Request = request,
                    Exception = exception
                });
            }
        }



        public static void ClientSendRequest(this DiagnosticListener listener, AmpMessage request)
        {
            if (listener.IsEnabled(TomatoDiagnosticClientSendRequest))
            {
                listener.Write(TomatoDiagnosticClientSendRequest, new
                {
                    Request = request
                });
            }
        }

        public static void ClientReceiveResponse(this DiagnosticListener listener, AmpMessage request,AmpMessage response)
        {
            if (listener.IsEnabled(TomatoDiagnosticClientReceiveResponse))
            {
                listener.Write(TomatoDiagnosticClientReceiveResponse, new
                {
                    Request = request,
                    Response = response
                });
            }
        }

        public static void ClientException(this DiagnosticListener listener,AmpMessage request,Exception exception)
        {
            if (listener.IsEnabled(TomatoDiagnosticClientException))
            {
                listener.Write(TomatoDiagnosticClientException, new
                {
                    Request = request,
                    Exception = exception
                });
            }
        }


    }
}
