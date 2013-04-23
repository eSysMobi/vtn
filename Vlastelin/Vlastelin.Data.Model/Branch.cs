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
    public sealed class Branch
        :BaseItem
    {
        private String _name;
        private String _phone;
        private String _address;

        [FieldName("Name")]
        public string Name
        {
            get
            {
                return this._name;
            }
            set
            {
                this._name = value;
            }
        }

        [FieldName("Address")]
        public string Address
        {
            get
            {
                return this._address;
            }
            set
            {
				Ct.CheckRequireField("Адрес", value);
				if (String.IsNullOrWhiteSpace(value))
                {
                    throw new IncorrectDataException("Адрес должен быть введен");
                }
                this._address = value;
            }
        }

        [FieldName("Phone")]
        public string Phone
        {
            get
            {
                return this._phone;
            }
            set
            {
                if (!Regex.IsMatch(value, RegExp.RegexpPhone) && !String.IsNullOrEmpty(value))
                {
                    throw new IncorrectDataException("Неверный номер телефона. Номер может содержать цифры, -, пробелы и +(первым символом)");
                }
                this._phone = value;
            }
        }

        [FieldName("townId")]
        public long townId { get; set; }
        private Town _town;
        public Town Town
        {
            get { return _town; }
            set
            {
				Ct.CheckRequireField("Город", value);
				if (value == null)
                {
                    throw new IncorrectDataException("Не задан город, где находится филиал");
                }

                townId = value.Id;
                _town = value;
            }
        }

        public override string ToString()
        {
            return String.Format("[ID {0}] {1}", base.Id, this.Name);
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Branch)) return false;

            Branch b = obj as Branch;
            return
                base.Equals(obj) && // comparing ID
                b.Name == this.Name &&
                b.Address == this.Address &&
                b.Phone == this.Phone &&
                b.townId == this.townId;                
        }

        public override int GetHashCode()
        {
            return 
                base.GetHashCode() ^
                Ct.GetHashCode(Name) ^ 
                Ct.GetHashCode(Address) ^ 
                Ct.GetHashCode(Phone) ^ 
                townId.GetHashCode();
        }
    }
}
