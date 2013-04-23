using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vlastelin.Data.Model;
using Vlastelin.Common;

namespace VlastelinClient.ViewModel.ObjectsViewModel
{
    public class NamedItemVM : BaseItemVM
    {
        /// <summary>
        /// класс именованного объекта из модели
        /// </summary>
        protected FIOItem namedItem
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
                return this.namedItem.Name;
            }
            set
            {
                this.namedItem.Name = value;
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
                return this.namedItem.Surname;
            }
            set
            {
                this.namedItem.Surname = value;
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
                return this.namedItem.Patronymic;
            }
            set
            {
                this.namedItem.Patronymic = value;
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
                return this.namedItem.FullName;
            }
        }

        /// <summary>
        /// имя с инициалами
        /// </summary>
        public String InitialName
        {
            get
            {
                return this.namedItem.InitialName;
            }
        }
    }
}
