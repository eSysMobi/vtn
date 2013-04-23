using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vlastelin.Common.Attributes;
using System.Text.RegularExpressions;
using Vlastelin.Data.Model.Util;
using Vlastelin.Common;

namespace Vlastelin.Data.Model
{
    /// <summary>
    /// Пассажир
    /// </summary>
    public sealed class Passenger : FIOItem
    {
        private String _docSer;

        /// <summary>
        /// Тип документа
        /// </summary>
        [FieldName("DocType")]
        public long DocType { get; set; }

        /// <summary>
        /// серия документа
        /// </summary>
        [FieldName("DocSer")]
        public string DocSer 
        {
            get
            {
                return this._docSer;
            }
            set
            {
				//if (value == null || !Regex.IsMatch(value, RegExp.RegexpPassportSer))
				//{
				//    throw new IncorrectDataException("Неверно введена серия документа");
				//}
                this._docSer = value;
            } 
        }

        /// <summary>
        /// номер документа
        /// </summary>
        [FieldName("DocNum")]
        public long DocNum { get; set; }

        /// <summary>
        /// дата выдачи документа
        /// </summary>
        [FieldName("DocDate")]
        public DateTime DocDate { get; set; }

        public Passenger():base()
        {
            this.DocDate = DateTime.Now.Date;
        }


        public override bool Equals(object obj)
        {
            if (!(obj is Passenger)) return false;
            Passenger p = obj as Passenger;
            return
                base.Equals(obj) &&
                p.DocDate.Equals(this.DocDate) &&
                p.DocNum.Equals(this.DocNum) &&
                p.DocSer.Equals(this.DocSer) &&
                p.DocType.Equals(this.DocType);                
        }

        public override int GetHashCode()
        {
            return
                base.GetHashCode() ^
                DocDate.GetHashCode() ^
                DocNum.GetHashCode() ^
                DocSer.GetHashCode() ^
                DocType.GetHashCode();
        }

        public override string ToString()
        {
            return base.ToString();
        }
    }
}
