using System;
using RabbitMQ.Client;

namespace RabbitMQProducerBasic
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
            Console.WriteLine($"Producer (mode:{mode} , routekey:{routeKey})");

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
            if (isExchangeMode) //exchange mode
            {
                channelName = "demoExchange";
                channel.ExchangeDeclare(channelName, ExchangeType.Direct);
            }
            else//queue mode
            {
                channelName = "demoChannel";

                channel.QueueDeclare(
                    queue: channelName,
                    durable: false,
                    exclusive: false,
                    autoDelete: false,
                    arguments: null
                    );
            }
            string message = string.Empty;
            do
            {
                Console.WriteLine("Input message:");
                message = Console.ReadLine();
                byte[] body = System.Text.Encoding.UTF8.GetBytes(message);
                if (isExchangeMode)
                {
                    channel.BasicPublish(channelName, routeKey , null, body);
                }
                else
                {
                    channel.BasicPublish("", channelName, null, body);
                }
                
                Console.WriteLine($"Message sent successfully: {message}");
            } while (message.Trim().ToLower() != "exit");

            con.Close();
            channel.Close();

        }
    }
}
