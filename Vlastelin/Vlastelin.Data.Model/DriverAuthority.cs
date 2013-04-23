using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vlastelin.Common.Attributes;
using Vlastelin.Common;

namespace Vlastelin.Data.Model
{
    /// <summary>
    /// Доверенность
    /// </summary>
    public class DriverAuthority: BaseItem
    {
        private String _number;
        
        [FieldName("DriverId")]
        public long _driverId { get; set; }

        private Driver _driver;
        public Driver Driver
        {
            get { return _driver; }
            set 
            {
				if (value == null)
				{
					throw new IncorrectDataException("Не задан водитель у доверенности");
				}
				_driverId = value.Id;
                _driver = value;
            }
        }

        [FieldName("OwnerId")]
        public long _ownerId { get; set; }

        private Owner _owner;
        public Owner Owner
        {
            get { return _owner; }
            set
            {
				if (value == null)
				{
					throw new IncorrectDataException("Не задан владелец доверенности");
				}
				_ownerId = value.Id;
                _owner = value;
            }
        }

        /// <summary>
        /// Номер доверенности
        /// </summary>
        [FieldName("Number")]
        public string Number
        {
            get
            {
                return this._number;
            }
            set
            {
                if (String.IsNullOrWhiteSpace(value))
                {
                    throw new IncorrectDataException("Не введен номер доверенности");
                }
                this._number = value;
            }
        }

        /// <summary>
        /// Дата доверенности
        /// </summary>
        [FieldName("Date")]
        public DateTime Date { get; set; }

        public override void CopyFrom(BaseItem item)
        {
            base.CopyFrom(item);
            DriverAuthority da = item as DriverAuthority;
            if (da != null)
            {
                this.Driver.CopyFrom(da.Driver);
                this.Owner.CopyFrom(da.Owner);
            }
        }

        public override string ToString()
        {
            return String.Format("[ID {0}] [{1}] [{2}]", base.ToString(), this.Driver.InitialName, this.Owner.Name);
        }
    }
}
