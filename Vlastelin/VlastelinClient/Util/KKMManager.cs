using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vlastelin.KKM;
using Vlastelin.Data.Model;
using System.Reflection;
using System.Drawing;
using Vlastelin.Common;
using System.Windows;

namespace VlastelinClient.Util
{
    /// <summary>
    /// вспомогательный класс для работы с ККМ
    /// </summary>
    public class KKMManager
    {
        /// <summary>
        /// объект ККМ
        /// </summary>
        public KKM KKM { get; private set; }

        /// <summary>
        /// индикатор, показыающий, рабтает ли ККМ или нет
        /// </summary>
        /// <returns></returns>
        public bool IsKKMEnabled
        {
            get
            {
				return this.KKM != null || !Properties.Settings.Default.FlagUseKKM;
            }
        }

        #region properties

        public string KKMModel 
        { 
            get 
            {
				if (!IsKKMEnabled || !Properties.Settings.Default.FlagUseKKM) return ClientMsg.ErrorNoKKM;
                try
                {
                    return KKM.KKMModel;
                }
                catch (Exception ex)
                {
                    UtilManager.Instance.logger.LogMessage(Vlastelin.Common.LogEventType.Error, ex.ToString());
                    return ClientMsg.ErrorKKM;
                }
            } 
        }

		public KKMMode KKMMode
		{
			get
			{
				if (!IsKKMEnabled || !Properties.Settings.Default.FlagUseKKM) return KKMMode.Test;
				try
				{
					return KKM.Mode;
				}
				catch (Exception ex)
				{
					UtilManager.Instance.logger.LogMessage(Vlastelin.Common.LogEventType.Error, ex.ToString());
					return KKMMode.Test;
				}
			}
		}

        public string SerialNum
        { 
            get 
            {
				if (!IsKKMEnabled || !Properties.Settings.Default.FlagUseKKM) return ClientMsg.ErrorNoKKM;
                try
                {
                    return KKM.SerialNum;
                }
                catch (Exception ex)
                {
                    UtilManager.Instance.logger.LogMessage(Vlastelin.Common.LogEventType.Error, ex.ToString());
                    return ClientMsg.ErrorKKM;
                }
            } 
        }

        public string RegistrationNum
        { 
            get 
            {
				if (!IsKKMEnabled || !Properties.Settings.Default.FlagUseKKM) return ClientMsg.ErrorNoKKM;
                try
                {
                    return KKM.RegistrationNum;
                }
                catch (Exception ex)
                {
                    UtilManager.Instance.logger.LogMessage(Vlastelin.Common.LogEventType.Error, ex.ToString());
                    return ClientMsg.ErrorKKM;
                }
            } 
        }

        public long ControlCounterValue
        { 
            get 
            {
				if (!IsKKMEnabled || !Properties.Settings.Default.FlagUseKKM) return -1;
                try
                {
                    return KKM.ControlCounterValue;
                }
                catch (Exception ex)
                {
                    UtilManager.Instance.logger.LogMessage(Vlastelin.Common.LogEventType.Error, ex.ToString());
					return -1;
                }
            } 
        }

        public decimal IncrementalControlCounterValue
        { 
            get 
            {
				if (!IsKKMEnabled || !Properties.Settings.Default.FlagUseKKM) return -1;
                try
                {
                    return KKM.IncrementalControlCounterValue;
                }
                catch (Exception ex)
                {
                    UtilManager.Instance.logger.LogMessage(Vlastelin.Common.LogEventType.Error, ex.ToString());
					return -1;
                }
            } 
        }
        
        public decimal AccMoneyCounterValue
        { 
            get 
            {
				if (!IsKKMEnabled || !Properties.Settings.Default.FlagUseKKM) return -1;
                try
                {
                    return KKM.IncrementalControlCounterValue;
                }
                catch (Exception ex)
                {
                    UtilManager.Instance.logger.LogMessage(Vlastelin.Common.LogEventType.Error, ex.ToString());
					return -1;
                }
            } 
        }        

        public decimal TotalRevenue
        { 
            get 
            {
				if (!IsKKMEnabled || !Properties.Settings.Default.FlagUseKKM) return -1;
                try
                {
                    return KKM.TotalRevenue;
                }
                catch (Exception ex)
                {
                    UtilManager.Instance.logger.LogMessage(Vlastelin.Common.LogEventType.Error, ex.ToString());
					return -1;
                }
            } 
        }          

        public decimal TotalReturn
        { 
            get 
            {
				if (!IsKKMEnabled || !Properties.Settings.Default.FlagUseKKM) return -1;
                try
                {
                    return KKM.TotalReturn;
                }
                catch (Exception ex)
                {
                    UtilManager.Instance.logger.LogMessage(Vlastelin.Common.LogEventType.Error, ex.ToString());
					return -1;
                }
            } 
        }         
      
        public KKMMode Mode
        { 
            get 
            {
				if (!IsKKMEnabled || !Properties.Settings.Default.FlagUseKKM) return KKMMode.Test;
                try
                {
                    return KKM.Mode;
                }
                catch (Exception ex)
                {
                    UtilManager.Instance.logger.LogMessage(Vlastelin.Common.LogEventType.Error, ex.ToString());
                    return KKMMode.Test;
                }
            } 
        }  
        
        public string ModeString
        { 
            get 
            {
				if (!IsKKMEnabled || !Properties.Settings.Default.FlagUseKKM) return ClientMsg.ErrorNoKKM;
                try
                {
                    return KKM.ModeString;
                }
                catch (Exception ex)
                {
                    UtilManager.Instance.logger.LogMessage(Vlastelin.Common.LogEventType.Error, ex.ToString());
                    return ClientMsg.ErrorKKM;
                }
            } 
        }

        public int LastSellReturnCheckNum
        {
            get
            {
                if (!IsKKMEnabled || !Properties.Settings.Default.FlagUseKKM) return -1;
                try
                {
                    return KKM.LastSellReturnCheckNum;
                }
                catch (Exception ex)
                {
                    UtilManager.Instance.logger.LogMessage(Vlastelin.Common.LogEventType.Error, ex.ToString());
                    return -1;
                }
            }
        }
		public int LastSellCheckNum
        { 
            get 
            {
				if (!IsKKMEnabled || !Properties.Settings.Default.FlagUseKKM) return -1;
                try
                {
					return KKM.LastSellCheckNum;
                }
                catch (Exception ex)
                {
                    UtilManager.Instance.logger.LogMessage(Vlastelin.Common.LogEventType.Error, ex.ToString());
                    return -1;
                }
            } 
        }
		public DateTime InternalTime
		{
			get
			{
				if (!IsKKMEnabled || !Properties.Settings.Default.FlagUseKKM) return DateTime.MinValue;
				try
				{
					return KKM.InternalTime;
				}
				catch (Exception ex)
				{
					UtilManager.Instance.logger.LogMessage(Vlastelin.Common.LogEventType.Error, ex.ToString());
					return DateTime.MinValue;
				}
			}

			set
			{
				if (IsKKMEnabled && Properties.Settings.Default.FlagUseKKM)
				{
					try
					{
						this.KKM.InternalTime = value;
					}
					catch (Exception ex)
					{
						UtilManager.Instance.logger.LogMessage(Vlastelin.Common.LogEventType.Error, ex.ToString());
					}
				}
			}
		}  

        #endregion

        /// <summary>
        /// создание объекта ККМ
        /// </summary>
        public void CreateKKMObject()
        {
			// если не используем ККМ, то выходим
			if (!Properties.Settings.Default.FlagUseKKM)
			{
				UtilManager.Instance.StateManager.KKMState = KKMStates.NotUsed;
				return;
			}
			
			try
            {
                // создаем объект ККМ
                this.KKM = new KKM();

				String path = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"\Images\vlastelinLogo.bmp";
                System.Drawing.Bitmap bitmap = new System.Drawing.Bitmap(path);
                this.KKM.LogoForCheck = bitmap;
				UtilManager.Instance.StateManager.KKMState = KKMStates.Enabled;
				if (this.Mode == KKMMode.OpenedSession_after24h)
				{
					UtilManager.Instance.StateManager.KKMState = KKMStates.UnClosedSession;
				}
            }
            catch (Exception ex)
            {
                // при возникновении ошибки логгируем текст исключения
                UtilManager.Instance.logger.LogMessage(Vlastelin.Common.LogEventType.Error, ex.ToString());
				this.KKM = null;
				UtilManager.Instance.StateManager.KKMState = KKMStates.Disabled;
            }
        }

		/// <summary>
		/// проверка времени ККМ
		/// </summary>
		/// <returns></returns>
		public bool IsKKMTimeCorrect()
		{
			return Math.Abs((DateTime.Now - this.InternalTime).TotalSeconds) < Properties.Settings.Default.KKMTimeMaxDiscrepancy.TotalSeconds || UtilManager.Instance.StateManager.KKMState == KKMStates.NotUsed;
		}

		/// <summary>
		/// проверка на то, что не закрыта смена ККМ
		/// </summary>
		/// <returns></returns>
		public bool IsKKMSessionClosed()
		{
			return UtilManager.Instance.StateManager.KKMState == KKMStates.UnClosedSession && UtilManager.Instance.StateManager.KKMState != KKMStates.NotUsed;
		}

        /// <summary>
        /// печать чека на ККМ
        /// </summary>
        /// <param name="ticket">чек</param>
        public void KKM_SellSeat(Seat seat, double sum)
        {
			// если не используем ККМ, то выходим
			if (!Properties.Settings.Default.FlagUseKKM)
			{
				return;
			}
			
			if (!IsKKMEnabled)
            {
				throw new KKMException("ККМ не подключен");
            }
			try
			{
				this.KKM.SellTicket(seat, sum);
			}
			catch (Exception ex)
			{
				UtilManager.Instance.logger.LogMessage(Vlastelin.Common.LogEventType.Error, ex.ToString());
				throw new KKMException(ex.Message);
			}
        }

		/// <summary>
		/// продажа
		/// </summary>
		public void KKM_SellItem(SalesKind item, double sum)
		{
			// если не используем ККМ, то выходим
			if (!Properties.Settings.Default.FlagUseKKM)
			{
				return;
			}

			if (!IsKKMEnabled)
			{
				throw new KKMException("ККМ не подключен");
			}
			try
			{
				this.KKM.SellItem(item, sum);
			}
			catch (Exception ex)
			{
				UtilManager.Instance.logger.LogMessage(Vlastelin.Common.LogEventType.Error, ex.ToString());
				throw new KKMException(ex.Message);
			}
		}

        /// <summary>
        /// закрытие сессии
        /// </summary>
        public void KKM_CloseSession()
        {
			// если не используем ККМ, то выходим
			if (!Properties.Settings.Default.FlagUseKKM)
			{
				return;
			}
			
			if (!IsKKMEnabled)
			{
				throw new KKMException("ККМ не подключен");
			}
			try
			{
				this.KKM.CloseSession();
			}
			catch (Exception ex)
			{
				UtilManager.Instance.logger.LogMessage(Vlastelin.Common.LogEventType.Error, ex.ToString());
				throw new KKMException(ex.Message);
			}
        }

		/// <summary>
		/// закрытие сессии
		/// </summary>
		public void KKM_SessionReport()
		{
			// если не используем ККМ, то выходим
			if (!Properties.Settings.Default.FlagUseKKM)
			{
				return;
			}
			
			if (!IsKKMEnabled)
			{
				throw new KKMException("ККМ не подключен");
			}
			try
			{
				this.KKM.SessionReport();
			}
			catch (Exception ex)
			{
				UtilManager.Instance.logger.LogMessage(Vlastelin.Common.LogEventType.Error, ex.ToString());
				throw new KKMException(ex.Message);
			}
		}

		/// <summary>
		/// возврат билета
		/// </summary>
		public void KKM_ReturnTicket(Seat seat, double sum, double commission)
		{
			// если не используем ККМ, то выходим
			if (!Properties.Settings.Default.FlagUseKKM)
			{
				return;
			}
			
			if (!IsKKMEnabled)
			{
				throw new KKMException("ККМ не подключен");
			}
			try
			{
				this.KKM.ReturnTicket(seat, sum, commission);
			}
			catch (Exception ex)
			{
				UtilManager.Instance.logger.LogMessage(Vlastelin.Common.LogEventType.Error, ex.ToString());
				throw new KKMException(ex.Message);
			}
		}

		/// <summary>
		/// внесение денег в кассу
		/// </summary>
		public void KKM_CashIncome(double sum)
		{
			// если не используем ККМ, то выходим
			if (!Properties.Settings.Default.FlagUseKKM)
			{
				return;
			}

			if (!IsKKMEnabled)
			{
				throw new KKMException("ККМ не подключен");
			}
			try
			{
				this.KKM.CashIncome(sum);
			}
			catch (Exception ex)
			{
				UtilManager.Instance.logger.LogMessage(Vlastelin.Common.LogEventType.Error, ex.ToString());
				throw new KKMException(ex.Message);
			}
		}

		/// <summary>
		/// изъятие денег из кассы
		/// </summary>
		public void KKM_CashOutcome(double sum)
		{
			// если не используем ККМ, то выходим
			if (!Properties.Settings.Default.FlagUseKKM)
			{
				return;
			}
			
			if (!IsKKMEnabled)
			{
				throw new KKMException("ККМ не подключен");
			}
			try
			{
				this.KKM.CashOutcome(sum);
			}
			catch (Exception ex)
			{
				UtilManager.Instance.logger.LogMessage(Vlastelin.Common.LogEventType.Error, ex.ToString());
				throw new KKMException(ex.Message);
			}
		}

        /// <summary>
        /// сброс состояния ККМ
        /// </summary>
        public void KKM_Reset()
        {
            // если не используем ККМ, то выходим
            if (!Properties.Settings.Default.FlagUseKKM)
            {
                return;
            }

            if (!IsKKMEnabled)
            {
                throw new KKMException("ККМ не подключен");
            }
            try
            {
                this.KKM.Reset();
            }
            catch (Exception ex)
            {
                UtilManager.Instance.logger.LogMessage(Vlastelin.Common.LogEventType.Error, ex.ToString());
                throw new KKMException(ex.Message);
            }
        }
    }
}
