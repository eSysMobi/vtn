using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vlastelin.Common.Attributes;

namespace Vlastelin.Data.Model
{
    public class PKO : BaseItem
    {
        [FieldName("DocNum")]
        public string DocNum { get; set; }

        /// <summary>
        /// дата документа
        /// </summary>
        [FieldName("DocDate")]
        public DateTime DocDate { get; set; }

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
        public double Sum { get; set; }

        public override bool Equals(object obj)
        {
            if (!(obj is PKO)) return false;
            PKO pko = obj as PKO;
            return
                base.Equals(obj) &&
                pko.DocNum.Equals(this.DocNum) &&
                pko.DocDate.Equals(this.DocDate) &&
                pko.OperatorId.Equals(this.OperatorId) &&
                pko.Sum.Equals(this.Sum);
        }

        public override int GetHashCode()
        {
            return
                base.GetHashCode() ^
                DocNum.GetHashCode() ^
                DocDate.GetHashCode() ^
                OperatorId.GetHashCode() ^
                Sum.GetHashCode();
        }
    }
}
