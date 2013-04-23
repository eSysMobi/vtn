using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vlastelin.Data.Model;
using Vlastelin.Common;

namespace VlastelinClient.ViewModel.ObjectsViewModel
{
    public class PassengerVM : FIOItemVM
    {
        public Passenger passenger
        {
            get
            {
                return this.item as Passenger;
            }
            set
            {
                this.item = value;
            }
        }

        /// <summary>
        /// серия документа
        /// </summary>
        public string DocSer 
        { 
            get { return this.passenger.DocSer; } 
            set { this.passenger.DocSer = value; this.OnPropertyChanged("DocSer"); }
        }

        /// <summary>
        /// номер документа
        /// </summary>
        public long? DocNum 
        { 
            get { return this.passenger.DocNum != -1 ? new Nullable<long>(this.passenger.DocNum) : null; }
            set { this.passenger.DocNum = value.HasValue ? value.Value : -1;  this.OnPropertyChanged("DocNum"); }
        }

        /// <summary>
        /// тип документа
        /// </summary>
        public long DocType
        {
            get { return this.passenger.DocType; }
            set { this.passenger.DocType = value; this.OnPropertyChanged("DocType"); }
        }

        /// <summary>
        /// дата выдачи документа
        /// </summary>
        public DateTime DocDate
        {
            get { return this.passenger.DocDate; }
            set { this.passenger.DocDate = value; this.OnPropertyChanged("DocDate"); }
        }

        public PassengerVM()
        {
            this.passenger = new Passenger();
            this.DocNum = null;
        }

        public PassengerVM(Passenger pass)
        {
            this.passenger = pass;
        }

        public bool FilterCondition(String n, String s, String p)
        {
            bool nMatch = !String.IsNullOrEmpty(this.Name) ? this.Name.ToUpper().Contains(n.ToUpper()) : false;
            bool sMatch = !String.IsNullOrEmpty(this.Surname) ? this.Surname.ToUpper().Contains(s.ToUpper()) : false;
            bool pMatch = !String.IsNullOrEmpty(this.Patronymic) ? this.Patronymic.ToUpper().Contains(p.ToUpper()) : false;

            return nMatch && sMatch && pMatch;
        }
    }
}
