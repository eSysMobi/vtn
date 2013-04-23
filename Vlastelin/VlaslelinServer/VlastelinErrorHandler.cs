using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel.Dispatcher;

using Vlastelin.Common;
using System.ServiceModel;
using System.ServiceModel.Channels;

namespace VlastelinServer
{
    public class VlastelinErrorHandler
        :IErrorHandler
    {
        public Logger logger=new Logger();

        public bool HandleError(Exception error)
        {
            logger.LogMessage(LogEventType.Info,string.Format("HandleError: {0}",error.Message));
            // тут можно анализировать исключения по типу и выставлять возвращаемое значение.
            // пока у нашего сервиса InstanceMode = Single то пофигу что возвращать
            // иначе true значит, что сервис в порядке и не надо рвать сессию и убивать экземпляр сервиса,
            // false - наоборот
            return true;
        }

        public void ProvideFault(
            Exception error, 
            System.ServiceModel.Channels.MessageVersion version, 
            ref System.ServiceModel.Channels.Message fault)
        {
            // метод для перехвата всех исключений (кроме совсем уж критических) и отправки клиенту FaultException
            logger.LogMessage(LogEventType.Info, 
                string.Format("Exception of type {0} was thrown with message: {1}. Top of calling stack:\n{2}\n.",
                error.GetType().Name,
                error.Message, 
                error.StackTrace.Substring(0,error.StackTrace.IndexOf('\n')))
                );
            
            FaultException<VlastelinFault> fe
                = new FaultException<VlastelinFault>(new VlastelinFault(false, error.Message),new FaultReason(error.Message));

            MessageFault flt = fe.CreateMessageFault() ;
            
            fault = System.ServiceModel.Channels.Message.CreateMessage(
              version,
              flt,
              null // при включенной безопасности не могу заставить шифроваться эту часть заголовка. Поэтому передаю null
            );
        }
    }

}
