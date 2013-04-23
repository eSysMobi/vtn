using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vlastelin.Data.Model;
using Vlastelin.Common;
using VlastelinClient.Util;

namespace VlastelinClient.ViewModel.ObjectsViewModel
{
    /// <summary>
    /// посадочное место - вьюмодель
    /// </summary>
    public class SeatVM : BaseItemVM
    {
        /// <summary>
        /// класс место из модели
        /// </summary>
        public Seat seat
        {
            get
            {
                return this.item as Seat;
            }
            set
            {
                this.item = value;
            }
        }

        /// <summary>
        /// рейс
        /// </summary>
        public StationScheduleVM SS
        {
            get
            {
                return new StationScheduleVM(this.seat.SS);
            }
            set
            {
                this.seat.SS = value.ss;
                this.OnPropertyChanged("SS");
            }
        }

        /// <summary>
        /// выбранный пользователем подмаршрут
        /// </summary>
        public TripPriceVM TripPrice
        {
            get
            {
                return new TripPriceVM(this.seat.TripPrice);
            }
            set
            {
                this.seat.TripPrice = value.tripPrice;
                this.Price = (double)this.TripPrice.Price;
            }
        }

        /// <summary>
        /// номер места в автобусе
        /// </summary>
        public long SeatNumber
        {
            get
            {
                return this.seat.SeatNumber;
            }
            set
            {
                this.seat.SeatNumber = value;
                this.OnPropertyChanged("SeatNumber");
            }
        }

        /// <summary>
        /// состояние места: 0 - свободно, 1 - заблокировано на время оформления документов, 2 - забронировано, 3 - занято
        /// </summary>
        public SeatState State
        {
            get
            {
                return this.seat.State;
            }
            set
            {
                this.seat.State = value;
                this.OnPropertyChanged("State");
                this.OnPropertyChanged("TownDeparture");
                this.OnPropertyChanged("TownArrival");
                this.OnPropertyChanged("PriceString");
				this.OnPropertyChanged("PassengerFullName");
				this.OnPropertyChanged("SellingOperatorString");
            }
        }

        /// <summary>
        /// пассажир
        /// </summary>
        public PassengerVM Passenger
        {
            get
            {
                return new PassengerVM(this.seat.Passenger);
            }
            set
            {
                this.seat.Passenger = value != null ? value.passenger : null;
                this.OnPropertyChanged("Passenger");
                this.OnPropertyChanged("PassengerFullName");
            }
        }

		/// <summary>
		/// полное имя пассажира
		/// </summary>
        public String PassengerFullName
        {
            get
            {
                return this.seat.Passenger != null ? this.seat.Passenger.FullName : String.Empty;
            }
        }

		/// <summary>
		/// дата отправления
		/// </summary>
        public DateTime TripDate
        {
            get 
            {
                return this.seat.TripDate;
            }
            set
            {
                this.seat.TripDate = value;
                this.OnPropertyChanged("TripDate");
				this.OnPropertyChanged("TripDateStr");
            }
        }

		/// <summary>
		/// дата отправления (форматированная)
		/// </summary>
		public String TripDateStr
		{
			get
			{
				return this.seat.TripDate.ToString(Ct.LongDateTimeFormat, Ct.RussianCulture);
			}
		}

		/// <summary>
		/// пользовательский конечный пункт
		/// </summary>
        public String DesiredDestination
        {
            get
            {
                return this.seat.DesiredDestination;
            }
            set
            {
                this.seat.DesiredDestination = value;
                this.OnPropertyChanged("DesiredDestination");
            }
        }

        #region вспомогательные свойства для таблицы мест

        /// <summary>
        /// город отправления (из подмаршрута)
        /// </summary>
        public String TownDeparture
        {
            get
            {
                if (this.State != SeatState.Free)
                {
                    return this.TripPrice.Departure.Name;
                }
                return String.Empty;
            }
        }

        /// <summary>
        /// город прибытия (из подмаршрута)
        /// </summary>
        public String TownArrival
        {
            get
            {
                if (this.SS != null && this.State != SeatState.Free)
                {
                    if (!String.IsNullOrWhiteSpace(this.DesiredDestination)) return this.DesiredDestination;
                    return TripPrice.Arrival.Name;
                }
                return String.Empty;
            }
        }

        /// <summary>
        /// цена места (вспомогательная переменная для переноса цены в чек)
        /// </summary>
		public double Price { get; set; }

		/// <summary>
		/// оператор, продавший место
		/// </summary>
		public String SellingOperator { get; set; }

		/// <summary>
		/// строковое представление цены, для таблицы
		/// </summary>
		public String PriceString
		{
			get
			{
				if (this.SS != null && this.State == SeatState.Sold)
				{
					return this.Price.ToString();
				} 
				return String.Empty;
			}
		}

		/// <summary>
		/// оператор, продавший место (для отображения в таблице)
		/// </summary>
		public String SellingOperatorString
		{
			get
			{
				if (this.SS != null && (this.State == SeatState.Sold || this.State == SeatState.Reserved) && UtilManager.Instance.CurrentOperator != null)
				{
					return this.SellingOperator;
				}
				return String.Empty;
			}
		}

		/// <summary>
		/// маршрут
		/// </summary>
        public TripVM Trip
        {
            get
            {
                return this.SS.TS.Trip;
            }
        }

        #endregion

        public SeatVM()
        {
            this.seat = new Seat();
        }

        public SeatVM(Seat st)
        {
            this.seat = st;
        }

		public void ClearSeat()
		{
			this.seat.Id = 0;
			this.Passenger = new PassengerVM();
			this.State = SeatState.Free;
		}
    }
}
