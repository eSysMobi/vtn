using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vlastelin.Data.Model;

namespace VlastelinClient.ViewModel.ObjectsViewModel
{
    public enum ActType
    {
        None = 0,
        Added = 1,
        Modified = 2,
        Deleted = 3
    }

    
    public class TripPriceVM : BaseItemVM
	{
        public TripPrice tripPrice
        {
            get
            {
                return this.item as TripPrice;
            }
            set
            {
                this.item = value;
            }
        }

        public String Name
        {
            get
            {
                return String.Format("{0} - {1}", this.Departure.Name, this.Arrival.Name);
            }
        }

        /// <summary>
        /// Город отправления
        /// </summary>
        public TownVM Departure
        {
            get 
            {
                return new TownVM(this.tripPrice.Departure);
            }
            set
            {
                this.tripPrice.Departure = value.town;
                this.OnPropertyChanged("Departure");
            }
        }

        /// <summary>
        /// Город прибытия
        /// </summary>
        public TownVM Arrival
        {
            get
            {
                return new TownVM(this.tripPrice.Arrival);
            }
            set
            {
                this.tripPrice.Arrival = value.town;
                this.OnPropertyChanged("Arrival");
            }
        }

        public decimal Price 
        {
            get
            {
                return this.tripPrice.Price;
            }
            set
            {
                this.tripPrice.Price = value;
                this.OnPropertyChanged("Price");
            }
        }

        public TripPriceVM()
        {
            this.tripPrice = new TripPrice();
        }

        public TripPriceVM(TripPrice price)
        {
            if (price == null)
            {
                this.tripPrice = new TripPrice();
            }
            else
                this.tripPrice = price;

        }
	}
}
