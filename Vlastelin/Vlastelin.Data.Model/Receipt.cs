using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vlastelin.Common.Attributes;

namespace Vlastelin.Data.Model
{
    /// <summary>
    /// чек.
    /// данные об уже проданном билете
    /// </summary>
    public class Receipt
        :BaseItem
    {
        /// <summary>
        /// оператор - кассир
        /// </summary>
        [FieldName("OperatorId")]
        public long OperatorId { get; set; }
        private Operator _op;
        public Operator Operator
        {
            get { return _op; }
            set
            {
                OperatorId = value.Id;
                _op = value;
            }
        }

        /// <summary>
        /// сущность места, которое продано
        /// </summary>
        [FieldName("SeatId")]
        public long SeatId { get; set; }
        private Seat _seat;
        public Seat Seat
        {
            get { return _seat; }
            set
            {
                SeatId = value.Id;
                _seat = value;
            }
        }

        /// <summary>
        /// пассажир, покупатель
        /// </summary>
        [FieldName("PassengerId")]
        public long PassengerId { get; set; }

        /// <summary>
        /// сумма покупки
        /// </summary>
        [FieldName("Sum")]
        public double Sum { get; set; }

        /// <summary>
        /// время продажи
        /// </summary>
        [FieldName("SoldTime")]
        public DateTime soldTime { get; set; }

        //public Bus Bus { get { return Seat.SubTrip.Trip.Bus; } }
        //public TripSchedule TS { get { return Seat.TS; } }
        public Passenger Passenger{ get { return this.Seat.Passenger; }}
        //public SubTrip SubTrip { get { return this.Seat.SubTrip; } }
        public DateTime TripDate { get { return this.Seat.TripDate; } }
    }
}
