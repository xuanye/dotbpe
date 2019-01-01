using System;
using System.Diagnostics;
using Peach.Messaging;

namespace Peach.Diagnostics
{
    public static class DiagnosticListenerExtensions
    {

        public const string DiagnosticListenerName = "PeachDiagnosticListener";
        public const string DiagnosticServiceReceive = "Peach.Service.Receive";
        public const string DiagnosticServiceReceiveCompleted = "Peach.Service.ReceiveCompleted";
        public const string DiagnosticServiceException = "Peach.Service.Exception";
        public const string DiagnosticClientReceive = "Peach.Client.Receive";
        public const string DiagnosticClientReceiveCompleted = "Peach.Client.ReceiveCompleted";
        public const string DiagnosticClientException = "Peach.Client.Exception";

        public static void ServiceReceive<TMessage>(this DiagnosticListener listener, TMessage ReceiveMessage) where TMessage : IMessage
        {
            if (listener.IsEnabled(DiagnosticServiceReceive))
            {
                listener.Write(DiagnosticServiceReceive, new
                {
                    Message = ReceiveMessage
                });
            }
        }

        public static void ServiceReceiveCompleted<TMessage>(this DiagnosticListener listener, TMessage ReceiveMessage) where TMessage : IMessage
        {
            if (listener.IsEnabled(DiagnosticServiceReceiveCompleted))
            {
                listener.Write(DiagnosticServiceReceiveCompleted, new
                {
                    Request = ReceiveMessage
                });
            }
        }

        public static void ServiceException(this DiagnosticListener listener,  Exception exception)
        {
            if (listener.IsEnabled(DiagnosticServiceException))
            {
                listener.Write(DiagnosticServiceException, new
                {
                    Exception = exception
                });
            }
        }


        public static void ClientReceive<TMessage>(this DiagnosticListener listener, TMessage ReceiveMessage) where TMessage : IMessage
        {
            if (listener.IsEnabled(DiagnosticClientReceive))
            {
                listener.Write(DiagnosticClientReceive, new
                {
                    Message = ReceiveMessage
                });
            }
        }
        public static void ClientReceiveComplete<TMessage>(this DiagnosticListener listener, TMessage ReceiveMessage) where TMessage : IMessage
        {
            if (listener.IsEnabled(DiagnosticClientReceive))
            {
                listener.Write(DiagnosticClientReceive, new
                {
                    Message = ReceiveMessage
                });
            }
        }
        public static void ClientException(this DiagnosticListener listener,Exception exception)
        {
            if (listener.IsEnabled(DiagnosticClientException))
            {
                listener.Write(DiagnosticClientException, new
                {
                    Exception = exception
                });
            }
        }
    }

}
