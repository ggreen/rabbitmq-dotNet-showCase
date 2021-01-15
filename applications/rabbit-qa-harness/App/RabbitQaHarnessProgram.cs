using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Imani.Solutions.Core.API.Util;

namespace rabbit_qa_harness.App
{
    public class Program
    {
        public static void Main(string [] args)
        {
            var config = new ConfigSettings();
            var producerCount =  config.GetPropertyInteger("PRODUCERS",1);
            var consumerCount =  config.GetPropertyInteger("CONSUMERS",1);

            List<Task> tasks = new List<Task>();
            for (int i=0; i< producerCount; i++) {
                Task t = Task.Run( () => 
                    { 
                        var producer = new ProducerHarness(); 
                        producer.Run();
                    } );
                tasks.Add(t);
            }

            for (int i=0; i< consumerCount; i++) {
                Task t = Task.Run( () => 
                    { 
                        var consumer = new ConsumerHarness() {
                            IsNackRequeued = config.GetPropertyBoolean("CONSUMER_IS_NASK_REQUEUED",false)
                        }; 
                        consumer.Run();
                    } );
                tasks.Add(t);
            }
            
            Task.WaitAll(tasks.ToArray());
            foreach (Task t in tasks)
                Console.WriteLine("Task {0} Status: {1}", t.Id, t.Status);
            
        }
    }
}