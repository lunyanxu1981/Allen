using System;
using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace RabbitMQConsumerBasic
{
    class Program
    {
        static void Main(string[] args)
        {
            bool isExchangeMode = false;
            if (args.Length > 0 && args[0].Equals("x"))
            {
                isExchangeMode = true;
            }
            string routeKey = "defaultKey";
            if (args.Length > 1 && !string.IsNullOrWhiteSpace(args[1]))
            {
                routeKey = args[1];
            }
            string mode = isExchangeMode ? "Exchange" : "Queue";
            Console.WriteLine($"Consumer (mode:{mode} , routekey:{routeKey})");


            IConnectionFactory factory = new ConnectionFactory
            {
                HostName = "10.74.20.76",
                Port = 5672,
                UserName = "users",
                Password = "admin"
            };

            IConnection con = factory.CreateConnection();
            IModel channel = con.CreateModel();
            string channelName = string.Empty;
            string queueName = string.Empty;
            if (isExchangeMode == false)
            {
                channelName = "demoChannel";
                channel.QueueDeclare(
                    queue: channelName,
                    durable: false,
                    exclusive: false,
                    autoDelete: false,
                    arguments: null
                );
                //channel.BasicQos(0, 1, false);
            }
            else
            {
                channelName = "demoExchange";
                channel.ExchangeDeclare(channelName, ExchangeType.Direct);
                queueName = DateTime.Now.Second.ToString();
                channel.QueueDeclare(
                    queue: queueName,
                    durable: false,
                    exclusive: false,
                    autoDelete: false,
                    arguments: null
                );
                channel.QueueBind(queueName, channelName, routeKey, null);

                Console.WriteLine($"Queue name:{queueName}");
            }

            var consumer = new EventingBasicConsumer(channel);

            consumer.Received += (sender, ea) =>
            {
                byte[] message = ea.Body;
                Console.WriteLine($"Received message:{Encoding.UTF8.GetString(message)}");
                channel.BasicAck(ea.DeliveryTag, true);
            };

            /* 
             * with channel.BasicAck(ea.DeliveryTag, true);  
             *   => channel.BasicConsume(channelName, false, consumer);
             * without channel.BasicAck(ea.DeliveryTag, true); 
             *   => channel.BasicConsume(channelName, true, consumer);
             */
            if (isExchangeMode == false)
            {
                channel.BasicConsume(channelName, false, consumer);
                channel.Dispose();
                channel.Close();
            }
            else //Exchange mode
            {
                channel.BasicConsume(queueName, false, consumer);
            }
            
            Console.ReadKey();
        }

    }
}
