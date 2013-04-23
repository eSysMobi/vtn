using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vlastelin.Data.Model;
//using VlastelinClient.ServiceReference1;

namespace VlastelinClient.ViewModel.ObjectsViewModel
{
    /// <summary>
    /// вьюмодель для города
    /// </summary>
    public class TownVM : BaseItemVM
    {
        /// <summary>
        /// класс города из модели
        /// </summary>
        public Town town
        {
            get
            {
                return this.item as Town;
            }
            set
            {
                this.item = value;
            }
        }

        /// <summary>
        /// название города
        /// </summary>
        public String Name
        {
            get
            {
                return this.town.Name;
            }
            set
            {
                this.town.Name = value;
                this.OnPropertyChanged("Name");
            }
        }

        /// <summary>
        /// префикс города для номеров документа
        /// </summary>
        public string Prefix 
        {
            get
            {
                return this.town.Prefix;
            }
            set
            {
                this.town.Prefix = value;
                this.OnPropertyChanged("Prefix");
            }
        }

        /// <summary>
        /// последний номер документа для города
        /// </summary>
        public int LastNumber 
        {
            get
            {
                return this.town.LastNumber;
            }
            set
            {
                this.town.LastNumber = value;
                this.OnPropertyChanged("LastNumber");
            }
        }

        public TownVM()
        {
            this.town = new Town();
        }

        public TownVM(Town twn)
        {
            this.town = twn;
        }

        public override string ToString()
        {
            return this.town.ToString();
        }

        public bool FilterCondition(String name)
        {
            return this.Name.ToUpper().Contains(name.ToUpper());
        }
    }
}
