using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vlastelin.Common.Attributes;

namespace Vlastelin.Data.Model
{
    /// <summary>
    /// Основные настройки системы
    /// </summary>
    public sealed class MainSettings
    {
        /// <summary>
        /// Название организации
        /// </summary>
        [FieldName("OrganizationName")]
        public string OrganizationName { get; set; }

        /// <summary>
        /// Имя директора
        /// </summary>
        [FieldName("OrganizationDirName")]
        public string OrganizationDirName { get; set; }

        /// <summary>
        /// Фамилия директора
        /// </summary>
        [FieldName("OrganizationDirSurname")]
        public string OrganizationDirSurname { get; set; }

        /// <summary>
        /// Отчество директора
        /// </summary>
        [FieldName("OrganizationDirPatronymic")]
        public string OrganizationDirPatronymic { get; set; }

        /// <summary>
        /// ИНН организации
        /// </summary>
        [FieldName("OrganizationINN")]
        public string OrganizationINN { get; set; }

        /// <summary>
        /// КПП организации
        /// </summary>
        [FieldName("OrganizationKPP")]
        public string OrganizationKPP { get; set; }

        /// <summary>
        /// Корреспондентский счет организации
        /// </summary>
        [FieldName("OrganizationCorrAccount")]
        public string OrganizationCorrAccount { get; set; }

        /// <summary>
        /// Комиссия при возвтареб илета (в процентах)
        /// </summary>
        [FieldName("ReturnedCommission")]
        public decimal ReturnedCommission { get; set; }

        public override bool Equals(object obj)
        {
            if (!(obj is MainSettings)) return false;
            MainSettings ms = obj as MainSettings;
            return
                ms.OrganizationCorrAccount.Equals(this.OrganizationCorrAccount) &&
                ms.OrganizationDirName.Equals(this.OrganizationDirName) &&
                ms.OrganizationDirPatronymic.Equals(this.OrganizationDirPatronymic) &&
                ms.OrganizationDirSurname.Equals(this.OrganizationDirSurname) &&
                ms.OrganizationINN.Equals(this.OrganizationINN) &&
                ms.OrganizationKPP.Equals(this.OrganizationKPP) &&
                ms.OrganizationName.Equals(this.OrganizationName) &&
                ms.ReturnedCommission.Equals(this.ReturnedCommission);
        }
        public override int GetHashCode()
        {
            return
                OrganizationCorrAccount.GetHashCode() ^
                OrganizationDirName.GetHashCode() ^
                OrganizationDirPatronymic.GetHashCode() ^
                OrganizationDirSurname.GetHashCode() ^
                OrganizationINN.GetHashCode() ^
                OrganizationKPP.GetHashCode() ^
                OrganizationName.GetHashCode() ^
                ReturnedCommission.GetHashCode();
        }
    }
}
