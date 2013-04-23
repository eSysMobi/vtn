using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vlastelin.Common.Attributes;
using Vlastelin.Common;
using System.ComponentModel;

namespace Vlastelin.Data.Model
{
    public enum TripScheduleType
    {
        [DescriptionAttribute("Единоразово")]
        Once = 4,
        [DescriptionAttribute("Ежедневно")]
        Daily = 1,
        [DescriptionAttribute("По будним дням")]
        ByWeekday = 5,
        [DescriptionAttribute("По четным дням")]
        ByEven = 2,
        [DescriptionAttribute("По нечетным дням")]
        ByOdd = 3
    };

    /// <summary>
    /// Элемент расписания движения автобусов
    /// </summary>
    public sealed class TripSchedule: BaseItem
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

        [FieldName("ScheduleType")]
        public int _scheduleType { get; set; }

        public TripScheduleType ScheduleType
        {
            get
            {
                return (TripScheduleType)this._scheduleType;
            }
            set 
            {
                this._scheduleType = (int)value;
            }

        }


        /// <summary>
        /// Автобус
        /// </summary>
        [FieldName("busId")]
        public long BusId { get; set; }
        private Bus _bus;
        public Bus Bus
        {
            get { return _bus; }
            set
            {
                Ct.CheckRequireField("Автобус", value);

                _bus = value;
                if (value.Id != 0)
                    BusId = value.Id;
            }
        }

        /// <summary>
        /// Время начала действия расписания
        /// </summary>
        [FieldName("StartTime")]
        public DateTime StartTime { get; set; }

        /// <summary>
        /// Время окончания действия расписания(если есть)
        /// </summary>
        [FieldName("EndTime")]
        public DateTime EndTime { get; set; }

        public override bool Equals(object obj)
        {
            if (!(obj is TripSchedule)) return false;
            TripSchedule ts = obj as TripSchedule;
            return
                base.Equals(obj) &&
                ts.tripId.Equals(this.tripId) &&
                ts.ScheduleType.Equals(this.ScheduleType) &&
                ts.StartTime.Equals(this.StartTime) &&
                ts.EndTime.Equals(this.EndTime) &&
                ts.BusId.Equals(this.BusId);
        }

        public override int GetHashCode()
        {
            return
                base.GetHashCode() ^
                tripId.GetHashCode() ^
                ScheduleType.GetHashCode() ^
                StartTime.GetHashCode() ^
                BusId.GetHashCode() ^
                EndTime.GetHashCode();
        }

        public override string ToString()
        {
            return String.Format("[ID {0}] [{1}] [{2}]", base.Id, this.Trip.NameString);
        }
    }
}
