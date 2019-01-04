using System;
using System.Diagnostics;
using DotBPE.Rpc.Protocol;
using Peach;

namespace DotBPE.Rpc.Diagnostics
{
    public static class DotBPEDiagnosticListenerExtensions
    {
        public static readonly DiagnosticListener Listener = new DiagnosticListener(DotBPEDiagnosticListenerName);

        public const string DotBPEDiagnosticListenerName = "DotBPEDiagnosticListener";
        public const string DotBPEDiagnosticServiceActorReceiveRequest = "DotBPE.Rpc.ServiceActor.ReceiveRequest";
        public const string DotBPEDiagnosticServiceActorSendResponse = "DotBPE.Rpc.ServiceActor.SendResponse";
        public const string DotBPEDiagnosticServiceActorException = "DotBPE.Rpc.ServiceActor.Exception";
        public const string DotBPEDiagnosticClientSendRequest = "DotBPE.Rpc.Client.SendRequest";
        public const string DotBPEDiagnosticClientReceiveResponse = "DotBPE.Rpc.Client.ReceiveResponse";
        public const string DotBPEDiagnosticClientException = "DotBPE.Rpc.Client.Exception";

        public static void ServiceActorReceiveRequest(this DiagnosticListener listener,
            ISocketContext<AmpMessage> context, AmpMessage request)
        {
            if (listener.IsEnabled(DotBPEDiagnosticServiceActorReceiveRequest))
            {
                listener.Write(DotBPEDiagnosticServiceActorReceiveRequest, new
                {
                    Context = context,
                    Request = request
                });
            }
        }

        public static void ServiceActorSendResponse(this DiagnosticListener listener,
            ISocketContext<AmpMessage> context, AmpMessage request,AmpMessage response)
        {
            if (listener.IsEnabled(DotBPEDiagnosticServiceActorSendResponse))
            {
                listener.Write(DotBPEDiagnosticServiceActorSendResponse, new
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
            if (listener.IsEnabled(DotBPEDiagnosticServiceActorException))
            {
                listener.Write(DotBPEDiagnosticServiceActorException, new
                {
                    Context = context,
                    Request = request,
                    Exception = exception
                });
            }
        }



        public static void ClientSendRequest(this DiagnosticListener listener, AmpMessage request)
        {
            if (listener.IsEnabled(DotBPEDiagnosticClientSendRequest))
            {
                listener.Write(DotBPEDiagnosticClientSendRequest, new
                {
                    Request = request
                });
            }
        }

        public static void ClientReceiveResponse(this DiagnosticListener listener, AmpMessage request,AmpMessage response)
        {
            if (listener.IsEnabled(DotBPEDiagnosticClientReceiveResponse))
            {
                listener.Write(DotBPEDiagnosticClientReceiveResponse, new
                {
                    Request = request,
                    Response = response
                });
            }
        }

        public static void ClientException(this DiagnosticListener listener,AmpMessage request,Exception exception)
        {
            if (listener.IsEnabled(DotBPEDiagnosticClientException))
            {
                listener.Write(DotBPEDiagnosticClientException, new
                {
                    Request = request,
                    Exception = exception
                });
            }
        }


    }
}
