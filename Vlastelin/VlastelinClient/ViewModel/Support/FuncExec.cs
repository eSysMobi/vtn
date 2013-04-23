using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vlastelin.Common;
using System.ServiceModel;
//using VlastelinClient.ServiceReference2;
using VlastelinClient.Util;
using VlastelinClient.ServiceReference1;
using System.ServiceModel.Security;
//using VlastelinClient.ServiceReference3;

namespace VlastelinClient.Util
{
    /// <summary>
    /// обертка команд, выполняемых сервисом, необходима для отлова и обработки исключений сервиса и базы данных
    /// </summary>
    public static class FuncExec
    {
        /// <summary>
        /// обрабатывает исключения
        /// </summary>
        /// <param name="ex">исключение</param>
        public static void ProcessException(Exception exception)
        {
            try
            {
                throw exception;
            }
            catch (EmptyRequiredFieldException ex)
            {
                UtilManager.Instance.MessageProvider.ShowInformationWindow(ex.Message);
                UtilManager.Instance.logger.LogMessage(LogEventType.Error, ex.ToString());
            }
            catch (IncorrectDataException ex)
            {
                UtilManager.Instance.MessageProvider.ShowInformationWindow(ex.Message);
                UtilManager.Instance.logger.LogMessage(LogEventType.Error, ex.ToString());
            }
            catch (KKMException ex)
            {
                UtilManager.Instance.MessageProvider.ShowErrorWindow(ex.Message);
                UtilManager.Instance.logger.LogMessage(LogEventType.Error, ex.ToString());
                UtilManager.Instance.StateManager.KKMState = KKMStates.Disabled;
            }
            catch (MessageSecurityException ex)
            {
                UtilManager.Instance.StateManager.ServerState = ServerStates.Disconnected;
                UtilManager.Instance.MessageProvider.ShowWarningnWindow("Ошибка входа в систему. Проверьте правильность ввода логина или пароля");
                UtilManager.Instance.logger.LogMessage(LogEventType.Error, ex.ToString());
				UtilManager.Instance.Client = new VlastelinSrvClient();
            }
			catch (FaultException ex)
            {
                UtilManager.Instance.MessageProvider.ShowErrorWindow(ex.Reason.ToString());
                UtilManager.Instance.logger.LogMessage(LogEventType.Error, ex.ToString());
                UtilManager.Instance.StateManager.ServerState = ServerStates.Faulted;
            }
            catch (CommunicationException ex)
            {
                UtilManager.Instance.StateManager.ServerState = ServerStates.Disconnected;
                UtilManager.Instance.MessageProvider.ShowErrorWindow(ClientMsg.ErrorService);
                UtilManager.Instance.logger.LogMessage(LogEventType.Error, ex.ToString());
            }
            catch (Exception ex)
            {
                UtilManager.Instance.MessageProvider.ShowErrorWindow(ClientMsg.ErrorUnknown);
                UtilManager.Instance.logger.LogMessage(LogEventType.Error, ex.ToString());
                UtilManager.Instance.StateManager.ServerState = ServerStates.Unknown;
                UtilManager.Instance.StateManager.KKMState = KKMStates.Unknown;
            }
        }

        /// <summary>
        /// выполняет функцию по работе с сервисом
        /// </summary>
        /// <param name="action">функция</param>
        /// <param name="UtilManager.Instance">сервисные данные</param>
        public static void Execute(Action action)
        {
			try
			{
                action();
            }
			catch (Exception ex)
			{
                ProcessException(ex);
			}
        }
    }
}
