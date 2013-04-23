using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vlastelin.Common.Attributes;

namespace Vlastelin.Data.Model
{
    public sealed class SalesHistory: BaseItem
    {
        [FieldName("SoldItemKind")]
        public int SoldItemKind { get; set; }

        [FieldName("SoldItemId")]
        public long SoldItemId { get; set; }

        [FieldName("FactPrice")]
        public double FactPrice { get; set; }

        [FieldName("SoldTime")]
        public DateTime SoldTime { get; set; }

        [FieldName("CheckNumber")]
        public int CheckNumber { get; set; }

        [FieldName("Returned")]
        public bool Returned { get; set; }

        [FieldName("ReturnedCheckNum")]
        public int ReturnedCheckNumber { get; set; }

        [FieldName("OperatorId")]
        public long operatorId { get; set; }
        private Operator _op;
        public Operator Operator
        {
            get { return _op; }
            set
            {
                operatorId = value.Id;
                _op = value;
            }
        }

        public override string ToString()
        {
            return base.ToString();
        }
    }
}
