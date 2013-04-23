using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vlastelin.Common.Attributes;

namespace Vlastelin.Data.Model.ExportedData
{
    public class ExportedRKO
    {
        [FieldName("rkoId")]
        public int rkoId { get; set; }

        [FieldName("DocDate")]
        public DateTime DocDate { get; set; }

        [FieldName("Number")]
        public string DocNum { get; set; }

        [FieldName("FactDepartureTime")]
        public DateTime TripDateTime { get; set; }

        [FieldName("bus")]
        public string BusName { get; set; }

        [FieldName("ownerId")]
        public int OwnerId { get; set; }

        [FieldName("Owner")]
        public string OwnerName { get; set; }

        [FieldName("driverId")]
        public int DriverId { get; set; }

        [FieldName("Driver")]
        public string DriverFIO { get; set; }

        [FieldName("DriverPassport")]
        public string DriverPassport { get; set; }

        [FieldName("Trip")]
        public string TripName { get; set; }

        [FieldName("sum")]
        public double Sum { get; set; }

        [FieldName("Operator")]
        public string OperatorFIO { get; set; }

        [FieldName("Attachment")]
        public string Attachment { get; set; }
    }
}
