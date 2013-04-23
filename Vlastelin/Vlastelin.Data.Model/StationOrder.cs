using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vlastelin.Common.Attributes;

namespace Vlastelin.Data.Model
{
    public sealed class StationOrder : BaseItem
    {
        /// <summary>
        /// Маршрут
        /// </summary>
        [FieldName("TripId")]
        public long tripId { get; set; }
        private Trip _trip;
        public Trip Trip
        {
            get { return _trip; }
            set
            {
                _trip = value;
                if (value.Id != 0)
                    tripId = value.Id;
            }
        }

        /// <summary>
        /// Город 
        /// </summary>
        [FieldName("TownId")]
        public long _tId { get; set; }
        private Town _t;
        public Town Town
        {
            get { return _t; }
            set
            {
                _tId = value.Id;
                _t = value;
            }
        }


        [FieldName("StationOrder")]
        public int Order { get; set; }

        public override bool Equals(object obj)
        {
            if (!(obj is StationOrder)) return false;
            StationOrder so = obj as StationOrder;
            return
                base.Equals(obj) &&
                so.tripId.Equals(this.tripId) &&
                so._tId.Equals(this._tId) &&
                so.Order.Equals(this.Order);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode() ^ tripId.GetHashCode() ^ _tId.GetHashCode() ^ Order.GetHashCode();
        }

        public override string ToString()
        {
            return String.Format("[ID {0}] {1}", base.Id, this.Trip.NameString);
        }
    }
}
