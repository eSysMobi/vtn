using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vlastelin.Data.Model;
using System.Collections.ObjectModel;
using Vlastelin.Common;

namespace VlastelinClient.ViewModel.ObjectsViewModel
{
    /// <summary>
    /// вьюмодель для класса водитель
    /// </summary>
    public class DriverVM : FIOItemVM
    {
        /// <summary>
        /// класс водителя из модели
        /// </summary>
        public Driver driver
        {
            get 
            {
                return this.item as Driver;
            }
            set
            {
                this.item = value;
            }
        
        }

        /// <summary>
        /// серия паспорта
        /// </summary>
        public String PassportSer
        {
            get
            {
                return this.driver.PassportSer;
            }
            set
            {
                this.driver.PassportSer = value;
                this.OnPropertyChanged("PassportSer");
            }
        }

        /// <summary>
        /// номер паспорта
        /// </summary>
        public String PassportNum
        {
            get
            {
                return this.driver.PassportNum;
            }
            set
            {
                this.driver.PassportNum = value;
                this.OnPropertyChanged("PassportNum");
            }
        }

        /// <summary>
        /// дата выдачи паспорта
        /// </summary>
        public DateTime PassportDate 
        {
            get
            {
                return this.driver.PassportDate;
            }
            set
            {
                this.driver.PassportDate = value;
                this.OnPropertyChanged("PassportDate");
            }
        }

        /// <summary>
        /// кем выдан паспорт
        /// </summary>
        public string PassportIssuer 
        {
            get
            {
                return this.driver.PassportIssuer;
            }
            set
            {
                this.driver.PassportIssuer = value;
                this.OnPropertyChanged("PassportIssuer");
            }
        }

        /// <summary>
        /// список доверенностей
        /// </summary>
        public ObservableCollection<DriverAuthority> _authorities;

        public ObservableCollection<DriverAuthority> Authorities
        {
            get 
            {
                return this._authorities;
            }
            set
            {
                this._authorities = value;
                OnPropertyChanged("Authorities");
            }

        }

        public DriverVM()
        {
            this.driver = new Driver();
            this._authorities = new ObservableCollection<DriverAuthority>();
        }

        public DriverVM(Driver dr)
        {
            if (dr == null)
            {
                dr = new Driver();
            } 
            this.driver = dr;
            this._authorities = new ObservableCollection<DriverAuthority>(this.driver.authorities);
        }

        public override void CopyFrom(BaseItemVM itm)
        {
            base.CopyFrom(itm);
        }
        /// <summary>
        /// условие фильтрации
        /// </summary>
        /// <param name="name">имя</param>
        /// <param name="surname">фамилия</param>
        /// <returns>результат фильтра</returns>
        public bool FilterCondition(String name, String surname)
        {
            return this.Name.ToUpper().Contains(name.ToUpper()) && this.Surname.ToUpper().Contains(surname.ToUpper());
        }

    }
}
