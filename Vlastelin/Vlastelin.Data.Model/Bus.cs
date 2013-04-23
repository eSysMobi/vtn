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
    /// Автобус
    /// </summary>
    public sealed class Bus : BaseItem
    {
        private String _manufacter;
        private String _model;
        private String _regNumber;
        private long _passengersCount;

        /// <summary>
        /// Марка автобуса. Мерседес, Скания итд.
        /// </summary>
        [FieldName("Manufacter")]
        public string Manufacter 
        {
            get
            {
                return this._manufacter;
            }
            set
            {
				Ct.CheckRequireField("Производитель", value);
				if (String.IsNullOrWhiteSpace(value))
                {
                    throw new IncorrectDataException("Не задан производитель автобуса");
                }
                this._manufacter = value;
            }
        }

        /// <summary>
        /// Модель автобуса
        /// </summary>
        [FieldName("Model")]
        public string Model 
        {
            get
            {
                return this._model;
            }
            set
            {
				Ct.CheckRequireField("Модель", value);
				if (String.IsNullOrWhiteSpace(value))
                {
                    throw new IncorrectDataException("Не задана модель автобуса");
                }
                this._model = value;
            }
        }

        /// <summary>
        /// Гос номер
        /// </summary>
        [FieldName("RegNumber")]
        public string RegNumber 
        {
            get
            {
                return this._regNumber;
            }
            set
            {
				Ct.CheckRequireField("Регистрационный номер", value);
				if (!Regex.IsMatch(value, RegExp.RegexpBusRegNumber))
                {
                    throw new IncorrectDataException(String.Format("{0} [{1}]", "Формат регистрационного номера должен соответствовать формату регистрационных номеров автобусов и маршрутных такси России", value));
                }
                this._regNumber = value;
            }
        }

        /// <summary>
        /// рассчетное количество пассажиров
        /// </summary>
        [FieldName("PassengersCount")]
        public long PassengersCount
        {
            get
            {
                return this._passengersCount;
            }
            set
            {
                if (value <= 0)
                {
                    throw new IncorrectDataException("Не задано количество пассажиров или оно меньше одного");
                }
                this._passengersCount = value;
            }
        }

        /// <summary>
        /// Перевозчик - владелец автобуса
        /// </summary>
        private Owner _Owner;
        public Owner Owner { 
            get { return _Owner; } 
            set 
            {
				Ct.CheckRequireField("Перевозчик", value);

                _Owner = value;
                if (value.Id != 0)
                    this.OwnerId = value.Id;
            } 
        }

        [FieldName("OwnerId")]
        public long OwnerId { get; set; }

        public override void CopyFrom(BaseItem item)
        {
            base.CopyFrom(item);

            Bus bus = item as Bus;
            this.Owner.CopyFrom(bus.Owner);
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Bus)) return false;

            Bus b = obj as Bus;
            return 
                base.Equals(obj) &&
                b.Manufacter == this.Manufacter &&
                b.Model == this.Model &&
                b.OwnerId == this.OwnerId &&
                b.RegNumber == this.RegNumber &&
                b.PassengersCount == this.PassengersCount;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode() ^ 
                Ct.GetHashCode(Manufacter) ^ 
                Ct.GetHashCode(Model) ^
                Ct.GetHashCode(OwnerId) ^
                Ct.GetHashCode(RegNumber) ^
                Ct.GetHashCode(PassengersCount);
        }

        public override string ToString()
        {
            return String.Format("[ID {0}] {1} {2}", base.Id, this.Model, this.RegNumber);
        }
    }
}
