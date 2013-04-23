using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vlastelin.Common.Attributes;

namespace Vlastelin.Data.Model
{
    public sealed class RKO:
        BaseItem
    {
        /// <summary>
        /// Номер документа
        /// </summary>
        [FieldName("Number")]
        public string Number { get; set; }

        /// <summary>
        /// дата документа
        /// </summary>
        [FieldName("DocDate")]
        public DateTime DocDate { get; set; }

        /// <summary>
        /// ID рейса
        /// </summary>
        [FieldName("tsfId")]
        public long tsfId { get; set; }
        private TripScheduleFact _tsf;
        public TripScheduleFact TSF
        {
            get { return _tsf; }
            set
            {
                tsfId = value.Id;
                _tsf = value;
            }
        }
        
        /// <summary>
        /// ID оператор-кассира
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

        [FieldName("Sum")]
        public decimal Sum { get; set; }

        // поля для упрощения биндинга
        public Bus Bus { get { return TSF.FactBus; } }
        public Owner Owner { get { return Bus.Owner; } }
        public string DogNum { get { return Owner.DocNum; } }
        public DateTime DogDate { get { return Owner.DocDate; } }
        public string BusNum { get { return Bus.RegNumber; } }
        public DateTime DepTime { get { return TSF.FactDepartureTime; } }

        public override bool Equals(object obj)
        {
            if(!(obj is RKO)) return false;
            RKO rko = obj as RKO;
            return
                base.Equals(obj) &&
                rko.Number.Equals(this.Number) &&
                rko.DocDate.Equals(this.DocDate) &&
                rko.tsfId.Equals(this.tsfId) &&
                rko.OperatorId.Equals(this.OperatorId) &&
                rko.Sum.Equals(this.Sum);
        }

        public override int GetHashCode()
        {
            return
                base.GetHashCode() ^
                Number.GetHashCode() ^
                DocDate.GetHashCode() ^
                tsfId.GetHashCode() ^
                OperatorId.GetHashCode() ^
                Sum.GetHashCode();
        }
    }
}
