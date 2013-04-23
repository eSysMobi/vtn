using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vlastelin.Common.Attributes;

namespace Vlastelin.Data.Model
{
    public sealed class StationSchedule:BaseItem
    {
        [FieldName("TSId")]
        public long _tsId { get; set; }
        private TripSchedule _ts;
        public TripSchedule TS
        {
            get { return _ts; }
            set
            {
                _tsId = value.Id;
                _ts = value;
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

        [FieldName("DepartureTime")]
        public DateTime DepartureTime { get; set; }

        public override bool Equals(object obj)
        {
            if (!(obj is StationSchedule)) return false;
            StationSchedule ss = obj as StationSchedule;
            return
                base.Equals(obj) &&
                ss._tsId.Equals(this._tsId) &&
                ss._tId.Equals(this._tId) &&
                ss.DepartureTime.Equals(this.DepartureTime);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode() ^ _tsId.GetHashCode() ^ _tId.GetHashCode() ^ DepartureTime.GetHashCode();
        }

        public override string ToString()
        {
            return String.Format("[ID {0}] [{1}] [{2:HH:mm}]", base.Id, this.TS.Trip.NameString, this.DepartureTime);
        }
    }
}
