using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vlastelin.Common.Attributes;
using Vlastelin.Common;
using System.ComponentModel;

namespace Vlastelin.Data.Model
{
    /// <summary>
    /// Маршрут
    /// </summary>
    public sealed class Trip : BaseItem
    {
        /// <summary>
        /// название маршрута
        /// </summary>
		[FieldName("Name")]
		public string Name { get; set; }

        /// <summary>
        /// текстовое значение имени маршрута (по пунктам прибытия и отправления)
        /// </summary>
        public string NameString
        {
            get 
            {
                return String.Format("{0} - {1}", this.Departure.Name, this.Arrival.Name);
            }
        }
		
        /// <summary>
        /// Город отправления
        /// </summary>
        [FieldName("DepartureId")]
        public long _departureId { get; set; }
        private Town _dep;
        public Town Departure
        {
            get { return _dep; }
            set
            {
				Ct.CheckRequireField("Город отправления", value);

                if (value.Equals(_arr))
                {
                    throw new IncorrectDataException("Города отправления и прибытия должны быть различны");
                }
                _departureId = value.Id;
                _dep = value;
            }
        }

        /// <summary>
        /// Город прибытия
        /// </summary>
        [FieldName("ArrivalId")]
        public long _arrivalId { get; set; }
        private Town _arr;
        public Town Arrival
        {
            get { return _arr; }
            set
            {
				Ct.CheckRequireField("Город прибытия", value);

                if (value.Equals(_dep))
                {
                    throw new IncorrectDataException("Города отправления и прибытия должны быть различны");
                }

                _arrivalId = value.Id;
                _arr = value;
            }
        }

        /// <summary>
        /// Описание
        /// </summary>
        [FieldName("Description")]
        public string Description { get; set; }

        public Trip()
        {
            
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Trip)) return false;
            Trip t = obj as Trip;
            return
                base.Equals(obj) &&
                t._arrivalId.Equals(this._arrivalId) &&
                t._departureId.Equals(this._departureId);
        }

        public override int GetHashCode()
        {
			return
				base.GetHashCode() ^
				_arrivalId.GetHashCode() ^
				_departureId.GetHashCode() ^
				Ct.GetHashCode(Description);
        }

        public override string ToString()
        {
            return String.Format("[ID {0}] {1}", base.Id, this.NameString);
        }
    }
}
