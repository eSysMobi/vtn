using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vlastelin.Common.Attributes;

namespace Vlastelin.Data.Model
{
    public sealed class TripScheduleFact
        :BaseItem
    {
        [FieldName("TSId")]
        public long TSId { get; set; }
        private TripSchedule _ts;
        public TripSchedule TS
        {
            get { return _ts; }
            set
            {
                TSId = value.Id;
                _ts = value;
            }
        }

        [FieldName("FactDepartureTime")]
        public DateTime FactDepartureTime { get; set; }

        [FieldName("FactBusId")]
        public long FactBusId { get; set; }
        private Bus _bus;
        public Bus FactBus
        {
            get { return _bus; }
            set
            {
                FactBusId = value.Id;
                _bus = value;
            }
        }

        [FieldName("FactDriverId_1")]
        public long FactDriverId_1 { get; set; }
        private Driver _driver1;
        public Driver FactDriver1
        {
            get { return _driver1; }
            set
            {
                FactDriverId_1 = value.Id;
                _driver1 = value;
            }
        }

        [FieldName("FactDriverId_2")]
        public long FactDriverId_2 { get; set; }
        private Driver _driver2;
        public Driver FactDriver2
        {
            get { return _driver2; }
            set
            {
                FactDriverId_2 = value != null ? value.Id : -1;
                _driver2 = value;
            }
        }
               

        [FieldName("DepartureTownId")]
        public long DepartureTownId { get; set; }
        private Town _depTown;
        public Town DepartureTown
        {
            get { return _depTown; }
            set
            {
                DepartureTownId = value.Id;
                _depTown = value;
            }
        }

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

        [FieldName("OperationTime")]
        public DateTime OperationTime { get; set; }
    }
}
