using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.ServiceBus;

namespace Common.Tools
{       

    public class ServiceBusClient
    {
        private string QueueName = "mwtqueue";
        private string ServiceBusConnectionString = "Endpoint=sb://mostwanted.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=ZSbIAIZiMCVUxwCrW2i3WZ7sUfBnxPsWOobZg/Qvbaw=";
        private IQueueClient queueClient;

        public delegate void MessageDelegate(long sequenceNumber, string message);
        private event MessageDelegate MessageEvent;

        public ServiceBusClient()
        {        
            this.queueClient = new QueueClient(this.ServiceBusConnectionString, this.QueueName);

            var messageHandlerOptions = new MessageHandlerOptions(ExceptionReceivedHandler)
            {
                MaxConcurrentCalls = 1,
                AutoComplete = false
            };            
            queueClient.RegisterMessageHandler(ProcessMessagesAsync, messageHandlerOptions);
        }

        public async Task SendMessagesAsync(string message)
        {
            try
            {
                var queueMessage = new Message(Encoding.UTF8.GetBytes(message));
                await this.queueClient.SendAsync(queueMessage);
            }
            catch (Exception exception)
            {
                Console.WriteLine($"{DateTime.Now} :: Exception: {exception.Message}");
            }
        }

        public void RegisterOnMessages(MessageDelegate messageDelegate)
        {
            this.MessageEvent += messageDelegate;
        }

        private async Task ProcessMessagesAsync(Message message, CancellationToken token)
        {
            Console.WriteLine($"Received message: SequenceNumber:{message.SystemProperties.SequenceNumber} Body:{Encoding.UTF8.GetString(message.Body)}");
            this.MessageEvent(message.SystemProperties.SequenceNumber, Encoding.UTF8.GetString(message.Body));
            await queueClient.CompleteAsync(message.SystemProperties.LockToken);

        }

        private Task ExceptionReceivedHandler(ExceptionReceivedEventArgs exceptionReceivedEventArgs)
        {
            Console.WriteLine($"Message handler encountered an exception {exceptionReceivedEventArgs.Exception}.");
            var context = exceptionReceivedEventArgs.ExceptionReceivedContext;
            Console.WriteLine("Exception context for troubleshooting:");
            Console.WriteLine($"- Endpoint: {context.Endpoint}");
            Console.WriteLine($"- Entity Path: {context.EntityPath}");
            Console.WriteLine($"- Executing Action: {context.Action}");
            return Task.CompletedTask;
        }
        
    }

}

