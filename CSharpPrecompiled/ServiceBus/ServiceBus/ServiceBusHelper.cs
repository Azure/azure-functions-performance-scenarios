using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.ServiceBus.Core;


namespace ServiceBus
{
    public class ServiceBusHelper
    {
        public static async Task<int> CleanUpEntity(string entityName)
        {
            string serviceBusConnection = Environment.GetEnvironmentVariable("ServiceBusConnection", EnvironmentVariableTarget.Process);
            var messageReceiver = new MessageReceiver(serviceBusConnection, entityName, ReceiveMode.ReceiveAndDelete);
            Message message;
            int count = 0;
            do
            {
                message = await messageReceiver.ReceiveAsync(TimeSpan.FromSeconds(3)).ConfigureAwait(false);
                if (message != null)
                {
                    count++;
                }
                else
                {
                    break;
                }
            } while (true);
            await messageReceiver.CloseAsync();
            return count;
        }

        public static async Task AddMessages(string queueName, int count)
        {
            string serviceBusConnection = Environment.GetEnvironmentVariable("ServiceBusConnection", EnvironmentVariableTarget.Process);
            List<Message> messages = new List<Message>();
            QueueClient queueClient = new QueueClient(serviceBusConnection, queueName);
            for (int i = 0; i < count; i++)
            {
                messages.Add(new Message(Encoding.UTF8.GetBytes("t")));
                if (i % 2000 == 0)
                {
                    await queueClient.SendAsync(messages);
                    messages.Clear();
                }
            }
            if (messages.Count > 0)
            {
                await queueClient.SendAsync(messages);
            }
        }
    }
}
