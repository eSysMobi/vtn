using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vlastelin.Data.Model;
using VlastelinClient.Util;
using System.Collections.ObjectModel;
using Vlastelin.Common;

namespace VlastelinClient.ViewModel.ObjectsViewModel
{
    /// <summary>
    /// вьюмодель для расписания автобусов
    /// </summary>
    public class TripScheduleVM : BaseItemVM
    {
        /// <summary>
        /// объект класса расписание из модели
        /// </summary>
        public TripSchedule ts 
        {
            get
            {
                return this.item as TripSchedule;
            }
            set
            {
                this.item = value;
            }
        }

        /// <summary>
        /// Маршрут
        /// </summary>
        public TripVM Trip
        {
            get 
            {
                return new TripVM(this.ts.Trip);
            }
            set
            {
                this.ts.Trip = value.trip;
                this.OnPropertyChanged("Trip");
            }
        }

		/// <summary>
		/// автобус
		/// </summary>
		public BusVM Bus
		{
			get
			{
				return new BusVM(this.ts.Bus);
			}
			set
			{
				this.ts.Bus = value != null ? value.bus : null;
				this.OnPropertyChanged("Bus");
			}
		}

        /// <summary>
        /// Тип расписания (ежедневно, по четным дням итд)
        /// </summary>
        public TripScheduleType ScheduleType
        {
            get
            {
                return this.ts.ScheduleType;
            }
            set
            {
                this.ts.ScheduleType = value;
                this.OnPropertyChanged("ScheduleType");
            }
        }

		/// <summary>
		/// вспомогательное свойство для получения строкового значения типа расписания
		/// </summary>
		public String ScheduleTypeStr
		{
			get 
			{
				return this.ScheduleType.GetDescription();
			}

		}

        /// <summary>
        /// Время начала расписания
        /// </summary>
        public DateTime StartTime
        {
            get
            {
                return this.ts.StartTime;
            }
            set
            {
                this.ts.StartTime = value;
                this.OnPropertyChanged("StartTime");
            }
        }

        /// <summary>
        /// Время конца расписания
        /// </summary>
        public DateTime EndTime
        {
            get
            {
                return this.ts.EndTime;
            }
            set
            {
                this.ts.EndTime = value;
                this.OnPropertyChanged("EndTime");
            }
        }

        public TripScheduleVM()
        {
            this.ts = new TripSchedule();
        }

        public TripScheduleVM(TripSchedule sh)
        {
            if (sh == null)
            {
                sh = new TripSchedule();
            }
            this.ts = sh;
        }

        /// <summary>
        /// полное копирование расписания
        /// </summary>
        /// <param name="itm">присваиваемае расписание</param>
        public override void CopyFrom(BaseItemVM itm)
        {
            TripScheduleVM item = itm as TripScheduleVM;

            if (item != null)
            {
                base.CopyFrom(itm);

                this.Trip.CopyFrom(item.Trip);

                this.Refresh();
            }
        }

        public override string ToString()
        {
            return this.ts.ToString();
        }
    }
}
