using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Imani.Solutions.Core.API.Util;
using RabbitMQ.Client;

namespace delay_exchnage_harness.App
{
    public class Program
    {
        private static readonly bool mandatory = true;

        public static void Main(string[] args)
        {
            var config = new ConfigSettings();
            string exchange = config.GetProperty("exchange");

            string exchangeType = "x-delayed-message";
            IDictionary<string, object> arguments = new Dictionary<string,object>();
            arguments["x-delayed-type"] = "direct";

            var factory = new ConnectionFactory(){
                HostName = config.GetProperty("RABBIT_HOST","localhost"),
                Port = config.GetPropertyInteger("RABBIT_PORT",5672)
            };
            String message = config.GetProperty("message");

            using(var connection = factory.CreateConnection())
            {
                using(var channel = connection.CreateModel())
                {
                    channel.ExchangeDeclare(exchange,exchangeType,true,false,arguments);

                    string routingKey = "";
                    
                    IBasicProperties msgProperties = channel.CreateBasicProperties();
                    msgProperties.Headers = new Dictionary<string,object>();
                    msgProperties.Headers.Add("x-delay",config.GetPropertyInteger("DELAY_MS"));

                    byte[] body = new UTF8Encoding().GetBytes(message);


                    int msgCount = config.GetPropertyInteger("MSG_CNT");

                    for (int i = 0; i < msgCount; i++)
                    {
                        channel.BasicPublish(exchange,routingKey,mandatory,msgProperties,body);   
                        Thread.Sleep(TimeSpan.FromMilliseconds(config.GetPropertyInteger("DELAY_MS",5)));    
                    }
                 
                }
            }
            
            
        }

    }
}