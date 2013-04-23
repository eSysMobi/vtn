using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vlastelin.Data.Model;
using Vlastelin.Common.Attributes;
using VlastelinClient.Util;
using Vlastelin.Common;

namespace VlastelinClient.ViewModel.ObjectsViewModel
{
	public class StationScheduleVM : BaseItemVM
	{
        public StationSchedule ss
        {
            get
            {
                return this.item as StationSchedule;
            }
            set
            {
                this.item = value;
            }
        }

        public TripScheduleVM TS
        {
            get 
            { 
                return new TripScheduleVM(this.ss.TS); 
            }
            set
            {
                this.ss.TS = value != null ? value.ts : null;
				this.OnPropertyChanged("TS");
            }
        }

        /// <summary>
        /// Город
        /// </summary>
        public TownVM Town
        {
            get
            {
                return new TownVM(this.ss.Town);
            }
            set
            {
                this.ss.Town = value.town;
                this.OnPropertyChanged("Town");
            }
        }

		/// <summary>
		/// время отправления
		/// </summary>
        public DateTime DepartureTime
        {
            get
            {
                return this.ss.DepartureTime;
            }
            set
            {
                this.ss.DepartureTime = value;
                this.OnPropertyChanged("DepartureTime");
            }
        }

		/// <summary>
		/// вспомогательное свойство для времени отправления
		/// </summary>
        public String Time
        {
            get 
            {
                return this.DepartureTime.GetTimeString();
            }
        }

        public StationScheduleVM()
        {
            this.ss = new StationSchedule();
        }

        public StationScheduleVM(StationSchedule sch)
        {
            if (sch == null)
            {
                this.ss = new StationSchedule();
            }else
                this.ss = sch;
        }

        /// <summary>
        /// условие фильтрации для интерфейса отправки автобусов
        /// </summary>
        /// <param name="trip">маршрут</param>
        /// <param name="date">дата</param>
        /// <returns></returns>
        public bool FilterTripCondition(DateTime? date)
        {
            // если маршрут или дата не задана, то фильтр сразу не пройден
            if (!date.HasValue || this.TS == null)
            {
                return false;
            }

            bool matchDate = UtilManager.IsCalendarDateMatch(this.TS.ScheduleType, date.Value.Date, this.DepartureTime.Date);
            bool matchTrip = this.Town.town.Equals(UtilManager.Instance.CurrentOperator.Branch.Town);

            return matchDate && matchTrip;
        }

        /// <summary>
        /// условие фильтрации в главном окне (для показа мест)
        /// </summary>
        /// <param name="date">дата</param>
        /// <returns></returns>
        public bool FilterTripCondition(DateTime? date, TripPriceVM price, TripVM trip)
        {
            if (this.TS == null || date == null || trip == null)
            {
                return false;
            }
            bool matchDate = UtilManager.IsCalendarDateMatch(this.TS.ScheduleType, date.Value.Date, this.DepartureTime.Date);
            bool matchTrip = this.Town.Equals(price.Departure) && trip.Equals(this.TS.Trip);

            return matchDate && matchTrip;
        }
	}
}
