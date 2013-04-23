using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vlastelin.Common.Attributes;

namespace Vlastelin.Data.Model
{
    public enum SeatState
    {
        Free = 0,
        Locked = 1,
        Reserved = 2,
        Sold = 3
    }
    /// <summary>
    /// посадочное место
    /// </summary>
    public sealed class Seat : BaseItem, IItemForSale
    {
        /// <summary>
        /// ID рейса (расписание)
        /// </summary>
        [FieldName("SSid")]
        public long SSid { get; set; }
        private StationSchedule _ss;
        public StationSchedule SS 
        {
            get { return _ss; }
            set
            {
                SSid = value.Id;
                _ss = value;
            }
        }
        /// <summary>
        ///  
        /// </summary>
        [FieldName("TripPriceId")]
        public long TripPriceId { get; set; }
        private TripPrice _tp;
        public TripPrice TripPrice
        {
            get { return _tp; }
            set
            {
                TripPriceId = value.Id;
                _tp = value;
            }
        }

        /// <summary>
        /// номер места в автобусе
        /// </summary>
        [FieldName("SeatNumber")]
        public long SeatNumber { get; set; }

        /// <summary>
        /// состояние места: 0 - свободно, 1 - заблокировано на время оформления документов, 2 - забронировано, 3 - занято
        /// </summary>
        [FieldName("SeatState")]
        public long _state { get; set; }

        public SeatState State 
        { 
            get 
            { 
                return (SeatState)_state; 
            } 
            set 
            { 
                _state = (long)value; } 
        }     

        /// <summary>
        /// ID пассажира
        /// </summary>
        [FieldName("PassengerId")]
        public long PassengerId { get; set; }
        private Passenger _passenger;
        public Passenger Passenger 
        {
            get { return _passenger; }
            set 
            {
                PassengerId = value != null ? value.Id : 0;
                _passenger = value;
            } 
        }

        [FieldName("TripDate")]
        public DateTime TripDate { get; set; }

        [FieldName("DesiredDestination")]
        public string _desiredDestination { get; set; }
        public string DesiredDestination
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_desiredDestination))
                    return TripPrice.Arrival.Name;
                else
                    return _desiredDestination;
            }
            set { _desiredDestination = value; }
        }

        /// <summary>
        /// признак возвращенного билета
        /// </summary>
        [FieldName("Returned")]
        public bool Returned { get; set; }

        public override int GetHashCode()
        {
            // посмотреть рихтера
            return 
                this.SSid.GetHashCode() ^ 
                this.SeatNumber.GetHashCode() ^
                this.TripDate.GetHashCode() ^ 
                this.TripPriceId.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            Seat s = obj as Seat;
            if (s == null)
                return false;
            // ключи для сравнения - маршрут, время и номер места

            return (s.SSid == this.SSid
                && s.SeatNumber == this.SeatNumber
                && s.TripDate == this.TripDate
                && s.TripPriceId == this.TripPriceId);
        }

        public override string ToString()
        {
            return String.Format("[ID {0}] [{1} - {2}] [{3}]", base.Id, this.TripPrice.Departure.Name, this.TripPrice.Arrival.Name, this.Passenger != null ? this.Passenger.InitialName : String.Empty);
        }
    }
}
