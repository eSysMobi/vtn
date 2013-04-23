using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vlastelin.Common.Attributes;
using System.Text.RegularExpressions;
using Vlastelin.Data.Model.Util;
using Vlastelin.Common;
using System.ComponentModel;

namespace Vlastelin.Data.Model
{
    /// <summary>
    /// Типы комиссии владельца
    /// </summary>
    public enum FeeTypes
    {
        [DescriptionAttribute("Процент")] Precent = 0,      // процент
        [DescriptionAttribute("Фикс")] FixedAmount = 1   // фиксированная сумма
    }
    /// <summary>
    /// Организация-перевозчик
    /// </summary>
    public sealed class Owner : BaseItem
    {
        private String _name;
        private String _numSv;
        private String _ogrn;
        private String _inn;
        private String _docNum;

		private DateTime _docDate;
		private DateTime _docEndDate;
		private double _feeAmount;

        private String _dirName;
        private String _dirSurname;
        private String _dirPatronymic;

        public Owner()
            :base()
        {
            this.authorities = new List<DriverAuthority>();
        }

        /// <summary>
        /// Название организации
        /// </summary>
        [FieldName("Name")]
        public string Name 
        {
            get
            {
                return this._name;
            }
            set
            {
				Ct.CheckRequireField("Название", value);
				if (String.IsNullOrWhiteSpace(value))
                {
                    throw new IncorrectDataException("Название организации должно быть введено");
                }
                this._name = value;
            } 
        }

        /// <summary>
        /// Номер свидетельства
        /// </summary>
        [FieldName("NumSv")]
        public string NumSv 
        {
            get
            {
                return this._numSv;
            }
            set
            {
				if (!String.IsNullOrEmpty(value) && !Regex.IsMatch(value, RegExp.RegexpLongNumber))
                {
                    throw new IncorrectDataException("Номер свидетельства должен быть числом");
                }
                this._numSv = value;
            }
        }

        /// <summary>
        /// ОГРН
        /// </summary>
        [FieldName("OGRN")]

        public string OGRN 
        {
            get
            {
                return this._ogrn;
            }
            set
            {
				if (!String.IsNullOrEmpty(value) && (!Regex.IsMatch(value, RegExp.RegexpLongNumber) || (value.Length != 13 && value.Length != 15)))
                {
                    throw new IncorrectDataException("ОГРН должен быть числом длиной 13 или 15 символов");
                }
                this._ogrn = value;
            }
        }

        /// <summary>
        /// ИНН
        /// </summary>
        [FieldName("INN")]
        public string INN
        {
            get
            {
                return this._inn;
            }
            set
            {
				if (!String.IsNullOrEmpty(value) && (!Regex.IsMatch(value, RegExp.RegexpLongNumber) || (value.Length != 10 && value.Length != 12)))
                {
                    throw new IncorrectDataException("ИНН должен быть числом длиной 10 или 12 символов");
                }
                this._inn = value;
            }
        }

        /// <summary>
        /// Юридический адрес
        /// </summary>
        [FieldName("Address")]
        public string Address { get; set; }

        /// <summary>
        /// Номер и дата договора коммиссии
        /// </summary>
        [FieldName("DocNum")]
        public string DocNum 
        {
            get
            {
                return this._docNum;
            }
            set
            {
				if (String.IsNullOrEmpty(value))
                {
                    throw new IncorrectDataException("Номер документа не может быть пустым!");
                }
                this._docNum = value;
            }
        }

        [FieldName("DocDate")]
        public DateTime DocDate 
		{
			get
			{
				return this._docDate;
			}
			set
			{
				Ct.CheckRequireField("Дата документа", value);
				this._docDate = value;
			}
		}

        [FieldName("DocEndDate")]
		public DateTime DocEndDate
		{
			get
			{
				return this._docEndDate;
			}
			set
			{
				//Ct.CheckRequireField("Дата окончания документа", value);
				this._docEndDate = value;
			}
		}

        /// <summary>
        /// ФИО директора
        /// </summary>
        [FieldName("DirName")]
        public string DirName 
        {
            get
            {
                return this._dirName;
            }
            set
            {
				Ct.CheckRequireField("Имя управляющего", value);
				if (String.IsNullOrWhiteSpace(value))
                {
                    throw new IncorrectDataException("Неправильно введено имя директора");
                }
                this._dirName = value;
            }
        }

        [FieldName("DirSurname")]
        public string DirSurname 
        {
            get
            {
                return this._dirSurname;
            }
            set
            {
				Ct.CheckRequireField("Фамилия управляющего", value);
				if (String.IsNullOrWhiteSpace(value))
                {
                    throw new IncorrectDataException("Неправильно введена фамилия директора");
                }
                this._dirSurname = value;
            } 
        }

        [FieldName("DirPatronymic")]
        public string DirPatronymic 
        {
            get
            {
                return this._dirPatronymic;
            }
            set
            {
				Ct.CheckRequireField("Отчество управляющего", value);
				if (String.IsNullOrWhiteSpace(value))
                {
                    throw new IncorrectDataException("Неправильно введено отчество директора");
                }
                this._dirPatronymic = value;
            }
        }

        /// <summary>
        /// тип комиссии
        /// 1 - фиксированная сумма, 0 - процент
        /// </summary>
        [FieldName("FeeType")]
        public long _feeType { get; set; }

        public FeeTypes FeeType
        {
            get { return (FeeTypes)_feeType; }
            set { _feeType = (long)value; }
        }

        /// <summary>
        /// Размер комиссии. интерпретация зависит от значения FeeType
        /// </summary>
        [FieldName("FeeAmount")]
        public double FeeAmount { get; set; }

        [FieldName("DirPosition")]
        public long _dirPosition { get; set; }
        private DirPosition _dp;
        public DirPosition DirPosition
        {
            get { return _dp; }
            set
            {
				Ct.CheckRequireField("Позиция управляющего", value);
				if (value == null)
                {
                    throw new IncorrectDataException("Не указана позиция управляющего");
                }
                _dirPosition = value.Id;
                _dp = value;
            }
        }

        // поле которое только для отображения позиции
        public string DirPositionString { get { return DirPosition != null ? DirPosition.Name : String.Empty; } }

        /// <summary>
        /// список доверенностей
        /// </summary>
        public List<DriverAuthority> authorities { get; set; }

        public override bool Equals(object obj)
        {
            if (!(obj is Owner)) return false;
            Owner o = obj as Owner;
            return
                base.Equals(obj) &&
                o._dirPosition == this._dirPosition &&
                o.Address == this.Address &&
                o.DirName == this.DirName &&
                o.DirSurname == this.DirSurname &&
                o.DirPatronymic == this.DirPatronymic &&
                o.DocDate == this.DocDate &&
                o.DocNum == this.DocNum &&
                o.FeeAmount == this.FeeAmount &&
                o.FeeType == this.FeeType &&
                o.INN == this.INN &&
                o.Name == this.Name &&
                o.NumSv == this.NumSv &&
                o.OGRN == this.OGRN;
        }
        public override int GetHashCode()
        {
            return
                base.GetHashCode() ^
                Ct.GetHashCode(_dirPosition) ^
                Ct.GetHashCode(Address) ^
                Ct.GetHashCode(DirName) ^
                Ct.GetHashCode(DirSurname) ^
                Ct.GetHashCode(DirPatronymic) ^
                Ct.GetHashCode(DocDate) ^
                Ct.GetHashCode(DocNum) ^
                Ct.GetHashCode(FeeAmount) ^
                Ct.GetHashCode(FeeType) ^
                Ct.GetHashCode(INN) ^
                Ct.GetHashCode(Name) ^
                Ct.GetHashCode(NumSv) ^
                Ct.GetHashCode(OGRN);
        }

        public override string ToString()
        {
            return String.Format("[ID {0}] {1}", base.Id, this.Name);
        }
    }
}
