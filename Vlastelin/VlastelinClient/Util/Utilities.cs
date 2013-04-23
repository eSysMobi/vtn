using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VlastelinClient.ServiceReference1;
using Vlastelin.Common;
using Vlastelin.Data.Model;
using System.Windows;
using VlastelinClient.Windows;
using Vlastelin.KKM;

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
        /// словарь используемый для проверки, какие таблицы были изменены
        /// </summary>
        public Dictionary<ModifiedObjects, DateTime> ModifiedTables { get; set; }

        /// <summary>
        /// настройки которые хранятся в базе данных
        /// </summary>
        public MainSettings ServerSettings { get; set; }

        /// <summary>
        /// класс для работы с кассовым аппаратом
        /// </summary>
        public KKM KKM { get; set; }

        private UtilManager()
        {
            // создаем список последних изменений таблиц
            this.ModifiedTables = new Dictionary<ModifiedObjects, DateTime>();
            foreach (ModifiedObjects mo in Enum.GetValues(typeof(ModifiedObjects)))
            {
                this.ModifiedTables.Add(mo, DateTime.MinValue);
            }

            //KKM = new KKM(1);
        }

        /// <summary>
        /// показываем слэш-окно
        /// </summary>
        public void ShowSplash()
        {
            this.Splash = new SplashWindow();
            this.Splash.Show();
        }

        /// <summary>
        /// закрываем сплэш-окно
        /// </summary>
        public void CloseSplash()
        {
            this.Splash.Close();
        }

        // обновляет список последних изменений таблиц
        public void RefreshModifiedTables()
        {
            foreach (ModifiedObjects mo in Enum.GetValues(typeof(ModifiedObjects)))
            {
                this.ModifiedTables[mo] = this.Client.GetLastModifiedTime(mo);
            }       
        }

        public static bool IsCalendarDateMatch(TripScheduleType type, DateTime date)
        {
            switch (type)
            {
                case TripScheduleType.ByOdd:
                    {
                        return date.Day % 2 == 0;
                    }
                case TripScheduleType.ByEven:
                    {
                        return date.Day % 2 == 1;
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

        public static bool IsCalendarDateMatch(TripScheduleType type, DateTime date, DateTime source)
        {
            if (type == TripScheduleType.Once) return date.Date == source.Date;
            else return IsCalendarDateMatch(type, date);
        }
    }
}
