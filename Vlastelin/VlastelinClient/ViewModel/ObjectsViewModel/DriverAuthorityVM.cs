using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vlastelin.Data.Model;

namespace VlastelinClient.ViewModel.ObjectsViewModel
{
    /// <summary>
    /// вьюмодель для доверенности
    /// </summary>
    public class DriverAuthorityVM : BaseItemVM
    {
        /// <summary>
        /// объект доверенности из модели
        /// </summary>
        public DriverAuthority driverAuth
        {
            get 
            {
                return this.item as DriverAuthority;
            }
            set
            {
                this.item = value;
            }

        }

        /// <summary>
        /// водитель
        /// </summary>
        public DriverVM Driver
        {
            get 
            {
                return new DriverVM(this.driverAuth.Driver);
            }
            set
            {
                this.driverAuth.Driver = value.driver;
                this.OnPropertyChanged("Driver");
            }
        }

        /// <summary>
        /// владелец
        /// </summary>
        public OwnerVM Owner
        {
            get 
            {
                return new OwnerVM(this.driverAuth.Owner);
            }
            set
            {
                this.driverAuth.Owner = value.owner;
                this.OnPropertyChanged("Owner");
            }
        }

        /// <summary>
        /// Номер доверенности
        /// </summary>
        public string Number 
        { 
            get 
            {
                return this.driverAuth.Number;
            }
            set
            {
                this.driverAuth.Number = value;
                this.OnPropertyChanged("Number");
            }
        }

        /// <summary>
        /// Дата доверенности
        /// </summary>
        public DateTime Date 
        {
            get
            {
                return this.driverAuth.Date;
            }
            set
            {
                this.driverAuth.Date = value;
                this.OnPropertyChanged("Date");
            }
        }
    }
}
