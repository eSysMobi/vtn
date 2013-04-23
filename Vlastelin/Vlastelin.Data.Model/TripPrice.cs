using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vlastelin.Common.Attributes;

namespace Vlastelin.Data.Model
{
    public sealed class TripPrice:BaseItem
    {
        /// <summary>
        /// Город отправления
        /// </summary>
        [FieldName("DepartureId")]
        public long _departureId { get; set; }
        private Town _dep;
        public Town Departure
        {
            get { return _dep; }
            set
            {
                _departureId = value.Id;
                _dep = value;
            }
        }

        /// <summary>
        /// Город прибытия
        /// </summary>
        [FieldName("ArrivalId")]
        public long _arrivalId { get; set; }
        private Town _arr;
        public Town Arrival
        {
            get { return _arr; }
            set
            {
                _arrivalId = value.Id;
                _arr = value;
            }
        }

        [FieldName("Price")]
        public decimal Price { get; set; }

        public override bool Equals(object obj)
        {
            if (!(obj is TripPrice)) return false;
            TripPrice tp = obj as TripPrice;
            return
                base.Equals(obj) &&
                tp._arrivalId.Equals(this._arrivalId) &&
                tp._departureId.Equals(this._departureId) &&
                tp.Price.Equals(this.Price);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode() ^ _arrivalId.GetHashCode() ^ _departureId.GetHashCode() ^ Price.GetHashCode();
        }

        public override string ToString()
        {
            return String.Format("[ID {0}] {1} - {2}", base.Id, this.Departure.Name, this.Arrival.Name);
        }
    }
}
