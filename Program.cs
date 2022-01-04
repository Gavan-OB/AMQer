using Azure;
using Azure.Storage.Queues;
using Azure.Storage.Queues.Models;
using System;
using System.Threading.Tasks;

namespace AMQer
{
    class Program
    {
        static string connectionString = Environment.GetEnvironmentVariable("AZURE_STORAGE_CONNECTION_STRING");

        static void Main(string[] args)
        {
            if (args[0] == "--help" || args[0] == "-h" )
            {
                DisplayHelp();
            }

            string method = args[0];
            QueueClient queue = new QueueClient(connectionString, args[1]);
            string message = "null";
            string pop = "null";

            if (args.Length > 2)
            {
                message = args[2];
            }

            if (args.Length > 3)
            {
                pop = args[3];
            }

            switch (method)
            {
                case "read":
                    read(queue);
                    break;

                case "write":
                    write(message, queue);
                    break;

                case "delete":
                    delete(message, pop, queue);
                    break;

                case "clear":
                    clear(queue);
                    break;

                case "count":
                    count(queue);
                    break;
            }

            static void read(QueueClient queue)
            {
                QueueProperties properties = queue.GetProperties();

                if (properties.ApproximateMessagesCount > 0)
                {
                    TimeSpan vis = new TimeSpan(0, 2, 30);
                    QueueMessage[] retrievedMessage =  queue.ReceiveMessages(1, vis);
                    string rmsg = retrievedMessage[0].Body.ToString();
                    string rmsgid = retrievedMessage[0].MessageId;
                    string rmsgpop = retrievedMessage[0].PopReceipt;
                    Console.WriteLine(rmsg);
                    Console.WriteLine(rmsgid);
                    Console.WriteLine(rmsgpop);
                }
                else
                {
                    Console.WriteLine("Empty");
                }

            }

            static void write(string message, QueueClient queue)
            {
                try {
                        queue.SendMessage(message);
                        Console.WriteLine("Added: " + message);
                }
                catch(Exception ex)
                {
                    Console.WriteLine(ex);
                }

                QueueProperties properties = queue.GetProperties();
                Console.WriteLine(properties.ApproximateMessagesCount);
            }

            static void delete(string messageid, string messagepop, QueueClient queue)
            {
                queue.DeleteMessage(messageid, messagepop);
                Console.WriteLine("Removed");
            }

            static void clear(QueueClient queue)
            {
                queue.ClearMessages();
                Console.WriteLine("Cleared");
            }

            static void count(QueueClient queue)
            {
                QueueProperties properties = queue.GetProperties();
                Console.WriteLine(properties.ApproximateMessagesCount);
            }

            static void DisplayHelp()
            {
                Console.WriteLine("READ: AMQer.exe read queue");
                Console.WriteLine("Returns Message as string, MessageID and POP Receipt, hides message for 150 Seconds");
                Console.WriteLine("");
                Console.WriteLine("WRITE: AMQer.exe write queue message");
                Console.WriteLine("Returns Message added and approx number of items in queue ");
                Console.WriteLine("");
                Console.WriteLine("DELETE Message: AMQer.exe delete queue messageid popreceipt");
                Console.WriteLine("Deletes relevant message and returns confirmation, visability does not matter");
                Console.WriteLine("");
                Console.WriteLine("CLEAR Queue: AMQer.exe clear queue");
                Console.WriteLine("Clears the whole queue and returns confirmation");
                Console.WriteLine("");
                Console.WriteLine("COUNT: AMQer.exe count queue");
                Console.WriteLine("Returns approx count of messages in the queue");
                Console.WriteLine("");
            }
        }
    }
}
