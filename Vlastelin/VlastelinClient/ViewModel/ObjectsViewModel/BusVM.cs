using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vlastelin.Data.Model;

namespace VlastelinClient.ViewModel.ObjectsViewModel
{
    /// <summary>
    /// вьюмодель для автобуса
    /// </summary>
    public class BusVM : BaseItemVM
    {
        /// <summary>
        /// класс автобус из модели
        /// </summary>
        public Bus bus 
        {
            get
            {
                return this.item as Bus;
            }
            set
            {
                this.item = value;
            }
        }

        /// <summary>
        /// производитель автобуса
        /// </summary>
        public String Manufacter 
        { 
            get 
            { 
                return this.bus.Manufacter;
            } 
            set 
            {
                this.bus.Manufacter = value;
                OnPropertyChanged("Manufacter");
            }
        }

        /// <summary>
        /// модель автобуса
        /// </summary>
        public String Model 
        { 
            get 
            {
                return this.bus.Model;
            } 
            set 
            {
                this.bus.Model = value;
                OnPropertyChanged("Model "); 
            }
        }

        /// <summary>
        /// регистрационный номер
        /// </summary>
        public String RegNumber 
        { 
            get 
            { 
                return this.bus.RegNumber;
            } 
            set 
            {
                this.bus.RegNumber  = value;
                OnPropertyChanged("RegNumber "); 
            }
        }

        /// <summary>
        /// максимальное количество пассажиров
        /// </summary>
        public long PassengersCount
        {
            get
            {
                return this.bus.PassengersCount;
            }
            set
            {
                this.bus.PassengersCount = value;
                OnPropertyChanged("PassengersCount");
            }
        }

		/// <summary>
		/// перевозчик автобуса
		/// </summary>
        public OwnerVM Owner
        {
            get
            {
                return new OwnerVM(this.bus.Owner);
            }
            set
            {
                this.bus.Owner = value != null ? value.owner : null;
                OnPropertyChanged("Owner");
            }
        }

		/// <summary>
		/// для отображения в выпадающих спиках
		/// </summary>
		public String DisplayName
		{
			get
			{
				return String.Format("{0} {1}", this.Manufacter, this.RegNumber);
			}
		}

		/// <summary>
		/// для выпадающего списка с показом количества мест
		/// </summary>
		public String DisplayNameSeats
		{
			get
			{
				return String.Format("{0} ({1} мест)", this.DisplayName, this.PassengersCount);
			}
		}

        public BusVM()
        {
            this.bus = new Bus();
        }

        public BusVM(Bus bs)
        {
            this.bus = bs;
        }

        /// <summary>
        /// копирование объекта автобус
        /// </summary>
        /// <param name="itm">объект</param>
        public override void CopyFrom(BaseItemVM itm)
        {
            base.CopyFrom(itm);

            BusVM bus = (BusVM)itm ;
            if (bus != null)
            {
                this.Owner.CopyFrom(bus.Owner);
            }
        }

		/// <summary>
		/// фильтрация списка автобусов 
		/// </summary>
		/// <param name="manufacter">производитель</param>
		/// <param name="model">модель</param>
		/// <param name="pass">количество пассажиров</param>
		/// <returns>подходит или нет</returns>
        public bool FilterCondition(String manufacter, String model, String pass)
        {
            return this.item == null ? false : this.Manufacter.ToUpper().Contains(manufacter.ToUpper()) && 
                                               this.Model.ToUpper().Contains(model.ToUpper()) && 
                                               this.PassengersCount.ToString().Contains(!String.IsNullOrEmpty(pass) ? pass : String.Empty);
        }
    }
}
