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
    /// Водитель
    /// </summary>
    public sealed class Driver : FIOItem
    {
        private String _passportSer;
        private String _passportNum;

        public Driver()
            : base()
        {
            this.authorities = new List<DriverAuthority>();
            this.PassportDate = DateTime.MinValue;
        }

        /// <summary>
        /// Серия и номер пасморта
        /// </summary>
        [FieldName("PassportSer")]
        public string PassportSer 
        {
            get
            {
                return this._passportSer;
            }
            set
            {
				if (String.IsNullOrEmpty(value))
                {
                    throw new IncorrectDataException("Серия паспорта должна быть указана!");
                }
                this._passportSer = value;
            }
        }

        [FieldName("PassportNum")]
        public string PassportNum 
        {
            get
            {
                return this._passportNum;
            }
            set
            {
				if (!String.IsNullOrEmpty(value) && !Regex.IsMatch(value, RegExp.RegexpPassportNum))
                {
                    throw new IncorrectDataException(String.Format("{0}", "Номер паспорта должен быть числом длиной от 1 до 10"));
                }
                this._passportNum = value;
            }
        }

        [FieldName("PassportDate")]
        public DateTime PassportDate { get; set; }

        [FieldName("PassportIssuer")]
        public string PassportIssuer { get; set; }

        /// <summary>
        /// список доверенностей
        /// </summary>
        public List<DriverAuthority> authorities { get; set; }

        public override bool Equals(object obj)
        {
            if (!(obj is Driver)) return false;
            Driver d = obj as Driver;
            return
                base.Equals(obj) &&
                d.PassportSer == this.PassportSer &&
                d.PassportNum == this.PassportNum &&
                d.PassportIssuer == this.PassportIssuer &&
                d.PassportDate == this.PassportDate;
        }
        public override int GetHashCode()
        {
            return base.GetHashCode() ^
                Ct.GetHashCode(PassportDate)^
                Ct.GetHashCode(PassportIssuer) ^
                Ct.GetHashCode(PassportNum)^
                Ct.GetHashCode(PassportSer);
        }

        public override string ToString()
        {
            return base.ToString();
        }
    }
}
