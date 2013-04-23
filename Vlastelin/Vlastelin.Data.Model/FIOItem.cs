using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vlastelin.Common.Attributes;
using Vlastelin.Common;

namespace Vlastelin.Data.Model
{
    /// <summary>
    /// базовый класс для объектов, которые содержат ФИО (операторы, водители, пассажиры итд)
    /// </summary>
    public class FIOItem : BaseItem
    {
        private String _name;
        private String _surname;
        private String _patronymic;

        /// <summary>
        /// ФИО
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
				Ct.CheckRequireField("Имя", value);
				if (String.IsNullOrWhiteSpace(value))
                {
                    throw new IncorrectDataException("Имя должно быть заполнено");
                }
                this._name = value;
            }
        }
        [FieldName("Surname")]
        public string Surname
        {
            get
            {
                return this._surname;
            }
            set
            {
				Ct.CheckRequireField("Фамилия", value);
				if (String.IsNullOrWhiteSpace(value))
                {
                    throw new IncorrectDataException("Фамилия должна быть заполнена");
                }
                this._surname = value;
            }
        }
        [FieldName("Patronymic")]
        public string Patronymic
        {
            get
            {
                return this._patronymic;
            }
            set
            {
				Ct.CheckRequireField("Отчество", value);
				if (String.IsNullOrWhiteSpace(value))
                {
                    throw new IncorrectDataException("Отчество должно быть заполнено");
                }
                this._patronymic = value;
            }
        }

        /// <summary>
        /// полное имя
        /// пример: Иванов Иван Андреевич
        /// </summary>
        public String FullName
        {
            get 
            {
                return Ct.FullName(this.Name, this.Surname, this.Patronymic);
            }
        }

        /// <summary>
        /// имя с инициалами
        /// пример: Иванов ИА
        /// </summary>
        public String InitialName
        {
            get
            {
                return Ct.InitialName(this.Name, this.Surname, this.Patronymic);
            }
        }

        public FIOItem()
        {
            this._name = String.Empty;
            this._surname = String.Empty;
            this._patronymic = String.Empty;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is FIOItem)) return false;

            FIOItem ni = obj as FIOItem;
            return
                base.Equals(obj) &&
                ni.Name.Equals(this.Name) &&
                ni.Surname.Equals(this.Surname) &&
                ni.Patronymic.Equals(this.Patronymic);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode() ^
                this.Name.GetHashCode() ^
                this.Surname.GetHashCode() ^
                this.Patronymic.GetHashCode();
        }

        public override string ToString()
        {
            return String.Format("[ID {0}] {1}", base.Id, this.InitialName);
        }
    }
}
