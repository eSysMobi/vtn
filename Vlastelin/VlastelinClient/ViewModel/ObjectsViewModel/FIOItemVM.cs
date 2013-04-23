using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vlastelin.Data.Model;
using Vlastelin.Common;

namespace VlastelinClient.ViewModel.ObjectsViewModel
{
    public class FIOItemVM : BaseItemVM
    {
        /// <summary>
        /// класс именованного объекта из модели
        /// </summary>
        protected FIOItem fioItem
        {
            get
            {
                return this.item as FIOItem;
            }
            set
            {
                this.item = value;
            }

        }

        /// <summary>
        /// имя
        /// </summary>
        public String Name
        {
            get
            {
                return this.fioItem.Name;
            }
            set
            {
                this.fioItem.Name = value;
                this.OnPropertyChanged("Name");
                this.OnPropertyChanged("FullName");
                this.OnPropertyChanged("InitialName");
            }
        }

        /// <summary>
        /// фамилия
        /// </summary>
        public String Surname
        {
            get
            {
                return this.fioItem.Surname;
            }
            set
            {
                this.fioItem.Surname = value;
                this.OnPropertyChanged("Surname");
                this.OnPropertyChanged("FullName");
                this.OnPropertyChanged("InitialName");
            }
        }

        /// <summary>
        /// отчетство
        /// </summary>
        public String Patronymic
        {
            get
            {
                return this.fioItem.Patronymic;
            }
            set
            {
                this.fioItem.Patronymic = value;
                this.OnPropertyChanged("Patronymic");
                this.OnPropertyChanged("FullName");
                this.OnPropertyChanged("InitialName");
            }
        }

        /// <summary>
        /// полное имя
        /// </summary>
        public String FullName
        {
            get
            {
                return this.fioItem.FullName;
            }
        }

        /// <summary>
        /// имя с инициалами
        /// </summary>
        public String InitialName
        {
            get
            {
                return this.fioItem.InitialName;
            }
        }
    }
}
