using System;
using System.Text;
using System.Threading;
using Imani.Solutions.Core.API.Util;
using rabbit_api.API;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace SecretEncryption
{
    /// <summary>
    /// SecretEncryptionProgram using the Imani encryption to generated an encrypted secret.
    /// author: Gregory Green
    /// </summary>
    public class SecretEncryptionProgram
    {
        static void Main(string[] args)
        {
            if(args.Length == 0)
            {
                Console.WriteLine("password arg required");
                return;
            }
           
            Console.WriteLine(EncryptSecret(args[0]));
        }

        internal static string EncryptSecret(string secret)
        {
            return new ConfigSettings().EncryptSecret(secret);
        }
    }
}
