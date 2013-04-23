using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Threading;
using System.ComponentModel;

namespace VlastelinClient.Util
{
	public class TimerManager
	{
		/// <summary>
		/// таймер для проверки соединения с сервером
		/// </summary>
		public DispatcherTimer TimerMonitorConnection { get; set; }

		/// <summary>
		/// таймер для проверки наличия обновления данных
		/// </summary>
		//public DispatcherTimer TimerMonitorDataUpdates { get; set; }

		/// <summary>
		/// тамйер для проверки наличия обновлений программы
		/// </summary>
		public DispatcherTimer TimerMonitorAppVersion { get; set; }

		/// <summary>
		/// флаг, необходимый для синхронизации проверки обновления данных и соединения с сервером
		/// 1 - проверка соединения выполнена и можно проверять обновление данных, 0 - нет
		/// </summary>
		//public bool IsCheckConnectionSucceed { get; set; }

		/// <summary>
		/// используется для проверки обновленных данных
		/// </summary>
		//public BackgroundWorker BackgroundWorkerMonitorUpdates { get; set; }

		/// <summary>
		/// используется для проверки соединения с сервером
		/// </summary>
		public BackgroundWorker BackgroundWorkerMonitorConnection { get; set; }

		/// <summary>
		/// инициализация таймеров
		/// </summary>
		public void Init()
		{
			// устанавливает значение таймеров из конфига
            this.TimerMonitorConnection = new DispatcherTimer(DispatcherPriority.Normal);
			this.TimerMonitorConnection.Interval = new TimeSpan(0, 0, Properties.Settings.Default.IntervalTimerCheckConnection / 1000); 

			//this.TimerMonitorDataUpdates = new DispatcherTimer(DispatcherPriority.Normal);
			//this.TimerMonitorDataUpdates.Interval = new TimeSpan(0, 0, Properties.Settings.Default.IntervalTimerCheckDataUpdates / 1000); 

			this.TimerMonitorAppVersion = new DispatcherTimer(DispatcherPriority.Normal);
            this.TimerMonitorAppVersion.Interval = new TimeSpan(0, 0, Properties.Settings.Default.IntervalTimerCheckVersion / 1000); 
		}

		/// <summary>
		/// стартуем все таймеры
		/// </summary>
		public void StartTimers()
		{
			this.BackgroundWorkerMonitorConnection = new BackgroundWorker();

			this.TimerMonitorConnection.Start();
			this.TimerMonitorAppVersion.Start();
			//this.TimerMonitorDataUpdates.Start();
		}

        public void Start_TimerMonitorConnection()
        {
            if (this.TimerMonitorConnection != null)
            {
                this.TimerMonitorConnection.Start();
            }
        }

        public void Stop_TimerMonitorConnection()
        {
            if (this.TimerMonitorConnection != null)
            {
                this.TimerMonitorConnection.Stop();
            }
        }
	}
}
