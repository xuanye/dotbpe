using System;
using System.Diagnostics;
using Peach.Messaging;

namespace Peach.Diagnostics
{
    public static class DiagnosticListenerExtensions
    {
        public const string DiagnosticListenerName = "PeachDiagnosticListener";
        public const string DiagnosticServiceReieve = "Peach.Service.Receive";
        public const string DiagnosticServiceReieveCompleted = "Peach.Service.ReieveCompleted";
        public const string DiagnosticServiceException = "Peach.Service.Exception";  
        public const string DiagnosticClientReceive = "Peach.Client.Receive";
        public const string DiagnosticClientReceiveCompleted = "Peach.Client.ReceiveCompleted";
        public const string DiagnosticClientException = "Peach.Client.Exception";

        public static void ServiceReceive<TMessage>(this DiagnosticListener listener, TMessage ReceiveMessage) where TMessage : IMessage
        {
            if (listener.IsEnabled(DiagnosticServiceReieve))
            {
                listener.Write(DiagnosticServiceReieve, new
                {
                    Message = ReceiveMessage
                });
            }
        }

        public static void ServiceReceiveCompleted<TMessage>(this DiagnosticListener listener, TMessage ReceiveMessage) where TMessage : IMessage
        {
            if (listener.IsEnabled(DiagnosticServiceReieveCompleted))
            {
                listener.Write(DiagnosticServiceReieveCompleted, new
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