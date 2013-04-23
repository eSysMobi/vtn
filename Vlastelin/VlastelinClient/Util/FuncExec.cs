using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vlastelin.Common;
using System.ServiceModel;
using VlastelinClient.ServiceReference1;

namespace VlastelinClient.Util
{
    /// <summary>
    /// обертка команд, выполняемых сервисом, необходима для отлова и обработки исключений сервиса и базы данных
    /// </summary>
    public static class FuncExec
    {
        /// <summary>
        /// выполняет функцию по работе с сервисом
        /// </summary>
        /// <param name="action">функция</param>
        /// <param name="param">передаваемый параметр</param>
        /// <param name="util">сервисные данные</param>
        public static void Execute(Action<object> action, object param)
        {
            //try
            //{
                action(param);
            //}
            //catch (DataBaseException ex)
            //{
            //    util.MessageProvider.ShowErrorWindow(String.Format("{0} : {1}", ClientMsg.ErrorDataBaseSave, ex.Message));
            //}
            //catch (FaultException ex)
            //{
            //    util.MessageProvider.ShowErrorWindow(ClientMsg.ErrorService);
            //}
            //catch (CommunicationException ex)
            //{
            //    util.Client = new VlastelinSrvClient();
            //    util.MessageProvider.ShowErrorWindow(ClientMsg.ErrorService);
            //}
            //catch (Exception ex)
            //{
            //    util.MessageProvider.ShowErrorWindow(ClientMsg.ErrorUnknown);
            //}
        }

        /// <summary>
        /// выполняет функцию по работе с сервисом (принимает функции с возвращаемым результатом)
        /// </summary>
        /// <param name="action">функция</param>
        /// <param name="param">передаваемый параметр</param>
        /// <param name="util">сервисные данные</param>
        public static void ExecuteFunc(Func<object, object> action, object param)
        {
            //try
            //{
                object result = action(param);
            //}
            //catch (DataBaseException ex)
            //{
            //    util.MessageProvider.ShowErrorWindow(String.Format("{0} : {1}", ClientMsg.ErrorDataBaseSave, ex.Message));
            //}
            //catch (FaultException ex)
            //{
            //    util.MessageProvider.ShowErrorWindow(ClientMsg.ErrorService);
            //}
            //catch (CommunicationException ex)
            //{
            //    util.Client = new VlastelinSrvClient();
            //    util.MessageProvider.ShowErrorWindow(ClientMsg.ErrorService);
            //}
            //catch (FormatException ex)
            //{
            //    util.MessageProvider.ShowErrorWindow(ex.Message);
            //}
            //catch (Exception ex)
            //{
            //    util.MessageProvider.ShowErrorWindow(ClientMsg.ErrorUnknown);
            //}
        }

        /// <summary>
        /// выполняет функцию по работе с сервисом
        /// </summary>
        /// <param name="action">функция</param>
        /// <param name="util">сервисные данные</param>
        public static void Execute(Action action)
        {
            //try
            //{
                action();
            //}
            //catch (DataBaseException ex)
            //{
            //    util.MessageProvider.ShowErrorWindow(String.Format("{0} : {1}", ClientMsg.ErrorDataBaseSave, ex.Message));
            //}
            //catch (FaultException ex)
            //{
            //    util.MessageProvider.ShowErrorWindow(ClientMsg.ErrorService);
            //}
            //catch (CommunicationException ex)
            //{
            //    util.Client = new VlastelinSrvClient();
            //    util.MessageProvider.ShowErrorWindow(ClientMsg.ErrorService);
            //}
            //catch (Exception ex)
            //{
            //    util.MessageProvider.ShowErrorWindow(ClientMsg.ErrorUnknown);
            //}
        }
    }
}
