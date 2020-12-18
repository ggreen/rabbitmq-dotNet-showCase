using System;
using System.Text;
using System.Threading;
using Imani.Solutions.Core.API.Util;
using rabbit_api.API;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace rabbit_demo_consumer
{
    class PasswordEncryptionProgram
    {
        static void Main(string[] args)
        {
            if(args.Length == 0)
            {
                Console.WriteLine("password arg required");
                return;
            }
            ConfigSettings config = new ConfigSettings();
            Console.WriteLine(config.EncryptPassword(args[0].ToCharArray()));

        }
    }
}
