using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using VlastelinClient.ServiceReference3;
using Vlastelin.Common;
using Vlastelin.Data.Model;
using System.Windows;
using VlastelinClient.Windows;
using Vlastelin.KKM;
using VlastelinClient.ServiceReference1;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Globalization;
using System.Deployment.Application;

namespace VlastelinClient.Util
{
    public class UtilManager
    {
        /// <summary>
        /// возможен только один экземпляр менеджера утилит
        /// </summary>
        private static UtilManager _util;
        public static UtilManager Instance
        {
            get
            {
                if (_util == null)
                {
                    _util = new UtilManager();
                }
                return _util;
            }
        }

        /// <summary>
        /// заставка показывается при загрузке данных
        /// </summary>
        public SplashWindow Splash { get; set; }
        
        /// <summary>
        /// клиент для сервиса работы с базой данных
        /// </summary>
        public VlastelinSrvClient Client { get; set; }

        /// <summary>
        /// менеджер по работе с диалоговыми окнами
        /// </summary>
        public IMessageBox MessageProvider { get; set; }

        /// <summary>
        /// логгирование
        /// </summary>
        public Logger logger { get; set; }

        /// <summary>
        /// текущий оператор системы
        /// </summary>
        public Operator CurrentOperator { get; set; }

		/// <summary>
		/// текущая версия приложения
		/// </summary>
		public Version CurrentAppVersion
		{
			get
			{
				try
				{
					return ApplicationDeployment.CurrentDeployment.CurrentVersion;
				}
				catch
				{
					return Assembly.GetExecutingAssembly().GetName().Version;
				}
			}
		}

        /// <summary>
        /// словарь используемый для проверки, какие таблицы были изменены
        /// </summary>
        public Dictionary<ModifiedObjects, DateTime> ModifiedTables { get; set; }

        /// <summary>
        /// класс для работы с кассовым аппаратом
        /// </summary>
        public KKMManager KKMManager { get; set; }

		/// <summary>
		/// класс для работы с таймерами
		/// </summary>
		public TimerManager TimerManager { get; set; }

		/// <summary>
		/// настройки которые хранятся в базе данных
		/// </summary>
		public MainSettings ServerSettings { get; set; }

		/// <summary>
		/// класс для работы с стркой состояния
		/// </summary>
		public StateManager StateManager { get; set; }

        /// <summary>
        /// класс для работы с номерами документов
        /// </summary>
        public NumbersManager NumbersManager { get; set; }

        private UtilManager()
        {
            // создаем список последних изменений таблиц
            this.ModifiedTables = new Dictionary<ModifiedObjects, DateTime>();
            foreach (ModifiedObjects mo in Enum.GetValues(typeof(ModifiedObjects)))
            {
                this.ModifiedTables.Add(mo, DateTime.MinValue);
            }

            this.MessageProvider = new WindowsMessageBox();
            this.logger = new Logger();
            this.KKMManager = new KKMManager();
			this.StateManager = new StateManager();
            this.NumbersManager = new NumbersManager();
			this.TimerManager = new TimerManager();
        }

        /// <summary>
        /// показываем слэш-окно
        /// </summary>
        public void ShowSplash()
        {
            this.Splash = new SplashWindow();
			this.Splash.ToolTip = "Идет загрузка данных...";
			if (this.StateManager.IsServerEnabled) this.StateManager.ServerState = ServerStates.DataLoading;
            this.Splash.Show();
        }

		/// <summary>
		/// показываем слэш-окно
		/// </summary>
		public void ChangeSplashString(String content)
		{
			if (this.Splash != null)
			{
				this.Splash.ToolTip = content;
			}
		}

        /// <summary>
        /// закрываем сплэш-окно
        /// </summary>
        public void CloseSplash()
        {
			if (this.StateManager.IsServerEnabled || this.StateManager.ServerState == ServerStates.DataLoading) this.StateManager.ServerState = ServerStates.Connected;
			if (this.Splash != null)
            {
                this.Splash.Close();
            }
        }

        /// <summary>
        /// проверка на соответствие даты и типа расписания
        /// </summary>
        /// <param name="type">тип расписания</param>
        /// <param name="date">дата</param>
        /// <returns>соответствует или нет</returns>
        public static bool IsCalendarDateMatch(TripScheduleType type, DateTime date)
        {
            switch (type)
            {
                case TripScheduleType.ByOdd:
                    {
                        return date.Day % 2 == 1;
                    }
                case TripScheduleType.ByEven:
                    {
                        return date.Day % 2 == 0;
                    }
                case TripScheduleType.Daily:
                    {
                        return true;
                    }
                case TripScheduleType.ByWeekday:
                    {
                        return (date.DayOfWeek != DayOfWeek.Saturday && date.DayOfWeek != DayOfWeek.Sunday);
                    }
            }
            return false;
        }

        /// <summary>
        /// проверка на соответствие даты и типа расписания, включая одноразовое расписание
        /// </summary>
        /// <param name="type">тип расписания</param>
        /// <param name="date">дата</param>
        /// <param name="source">дата в расписании (для одноразового)</param>
        /// <returns>соответствует или нет</returns>
        public static bool IsCalendarDateMatch(TripScheduleType type, DateTime date, DateTime source)
        {
            if (type == TripScheduleType.Once) return date.Date == source.Date;
            else return IsCalendarDateMatch(type, date);
        }

        /// <summary>
        /// обновляет список последних изменений таблиц
        /// </summary>
        public void RefreshModifiedTables()
        {
            foreach (ModifiedObjects mo in Enum.GetValues(typeof(ModifiedObjects)))
            {
                this.ModifiedTables[mo] = this.Client.GetLastModifiedTime(mo);
            }
        }

        /// <summary>
        /// проверка на то, нужно ли загружать данные или нет
        /// </summary>
        /// <returns></returns>
        public bool NeedLoad(ModifiedObjects modObj)
        {
            DateTime modDate = UtilManager.Instance.Client.GetLastModifiedTime(modObj);
            if (ModifiedTables[modObj] == DateTime.MinValue)
            {
                ModifiedTables[modObj] = modDate;
                return true;
            }

			if (modDate > ModifiedTables[modObj])
			{
				ModifiedTables[modObj] = modDate;
				return true;
			}
			return false;
        }

		/// <summary>
		/// обновляем измененные объекты
		/// </summary>
		/// <param name="modObj">тип измененного объекта</param>
		public void UpdateModifiedObject(ModifiedObjects modObj)
		{
			ModifiedTables[modObj] = UtilManager.Instance.Client.GetLastModifiedTime(modObj);
		}

		/// <summary>
		/// посылает логи на фтп
		/// </summary>
		public void SendErrorNotification(String description)
		{
			List<String> systemInfo = new List<string>();
			String separator = new String('-', 20);
			int byteInMB = 1048576; // байт в одном мегабайте

			// системная информация и аппаратная часть компьютера
			systemInfo.Add(String.Format("Имя компьютера : {0}", Environment.MachineName));
			systemInfo.Add(String.Format("Имя пользователя : {0}", Environment.UserName));
			systemInfo.Add(String.Format("Версия операционной системы : {0}", Environment.OSVersion));
			systemInfo.Add(String.Format("Количество ядер процессора : {0}", Environment.ProcessorCount));
			systemInfo.Add(String.Format("Объем физической памяти текущего процесса : {0} Mb", Environment.WorkingSet / byteInMB));

			// жесткие диски
			systemInfo.Add(separator);
			systemInfo.Add("Носители информации");
			String str;
			foreach (DriveInfo drive in DriveInfo.GetDrives())
			{
				str = String.Empty;

				if (drive.IsReady) str =
					String.Format("{0}, Общий объем {1} Мб, Файловая система {2}, Доступное пространство {3} Мб, Свободное пространство {4} Мб", 
					drive.RootDirectory.Name,
					drive.TotalSize / byteInMB, 
					drive.DriveFormat,
					drive.AvailableFreeSpace / byteInMB,
					drive.TotalFreeSpace / byteInMB);

				systemInfo.Add(str);
			}

			// параметры приложения
			systemInfo.Add(separator);
			systemInfo.Add(String.Format("Оператор : {0}", this.CurrentOperator != null ? this.CurrentOperator.FullName : "не установлено"));
			systemInfo.Add(String.Format("Версия приложения : {0}", this.CurrentAppVersion));
			systemInfo.Add(String.Format("Директория приложения : {0}", Environment.CurrentDirectory));
			systemInfo.Add(String.Format("Время компьютера : {0}", DateTime.Now));
			systemInfo.Add(String.Format("Пользовательская культура : {0}", CultureInfo.CurrentUICulture));

			// параметры ККМ
			systemInfo.Add(separator);
			systemInfo.Add(String.Format("Наличие ККМ : {0}", this.KKMManager.KKM != null ? "Да" : "Нет"));
			systemInfo.Add(String.Format("Состояние ККМ (в интерфейсе) : {0}", this.StateManager.KKMStateString));
			systemInfo.Add(String.Format("Режим ККМ (внутренней) : {0}", this.KKMManager.KKM != null ? this.KKMManager.KKM.Mode.ToString() : "не установлено"));
			String sysInfoPath = Environment.CurrentDirectory + @"\SystemInformation.txt";
			String logPath = Environment.CurrentDirectory + @"\VlastelinClient.log";
			String descPath = Environment.CurrentDirectory + @"\Description.txt";
			String destFileName = Environment.CurrentDirectory + @"\VlastelinLogInfo.zip";

			DriveInfo[] allDrives = DriveInfo.GetDrives();

			File.WriteAllLines(sysInfoPath , systemInfo.ToArray());
			File.WriteAllText(descPath, description);

			FileManager.AddFileToZip(destFileName, sysInfoPath);
			FileManager.AddFileToZip(destFileName, logPath);
			FileManager.AddFileToZip(destFileName, descPath);

			String ftpUser = "itinsightpro-vlastelin";
			String ftpPassword = "Mia&230777";
			String ftpURL = "office24.pro:21";

			FileManager.FTPUpload(String.Format("VlastelinLogInfo_{0:ddMMyyyyHHmm}.zip", DateTime.Now), destFileName, ftpURL, ftpUser, ftpPassword);
		}
    }
}
