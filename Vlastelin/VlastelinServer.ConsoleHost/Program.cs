using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;

using Vlastelin.Common;
using System.ServiceModel.Dispatcher;

namespace VlastelinServer.ConsoleHost
{
    class Program
    {
        static Logger logger = new Logger();

        static void Main(string[] args)
        {
            ServiceHost host = null; // только для того, чтобы избежать ошибки о неинициализированной переменной
            try
            {
                // мы можем так сделать, т.к. сервис у нас Singleton
                host = new ServiceHost(new VlastelinSrv());

                host.Faulted += new EventHandler(host_Faulted);
                host.UnknownMessageReceived += new EventHandler<UnknownMessageReceivedEventArgs>(host_UnknownMessageReceived);

                host.Open();
                logger.LogMessage(LogEventType.Info, "Service started");
                Console.WriteLine("Service started. Press any key to stop.");
                Console.ReadKey();
            }
            catch (Exception ex)
            {
                logger.LogMessage(LogEventType.Error, string.Format("Exception is thrown:{0}", ex.Message));
                Console.WriteLine(ex.Message);
            }
            finally
            {
                if (host!=null && host.State == CommunicationState.Opened)
                    host.Close();
                logger.LogMessage(LogEventType.Info, "ServiceHost closed");
            }
            Console.WriteLine("Service stopped. Press any key to exit...");
            Console.ReadKey();
        }

        static void host_UnknownMessageReceived(object sender, UnknownMessageReceivedEventArgs e)
        {
            logger.LogMessage(LogEventType.Warning, string.Format("Unknown message: {0}", e.Message.ToString()));
        }

        static void host_Faulted(object sender, EventArgs e)
        {
            logger.LogMessage(LogEventType.Error, "Host faulted!");
        }
    }
}
