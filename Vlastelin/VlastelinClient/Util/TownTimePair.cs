using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VlastelinClient.ViewModel.ObjectsViewModel;
using VlastelinClient.ViewModel;

namespace VlastelinClient.Util
{
    /// <summary>
    /// класс - пара Город + Время (в строковом формате)
    /// </summary>
    public class TownTimePair : BaseViewModel
    {
        private TownVM _town;
        private String _time;

        /// <summary>
        /// город
        /// </summary>
        public TownVM Town
        {
            get
            {
                return this._town;
            }
            set
            {
                this._town = value;
                this.OnPropertyChanged("Town");
            }
        }

        /// <summary>
        /// время в 24-часовом вормате
        /// </summary>
        public String Time
        {
            get
            {
                return this._time;
            }
            set
            {
                this._time = value;
                this.OnPropertyChanged("Time");
            }
        }

        public TownTimePair(TownVM town, String time)
        {
            this.Town = town;
            this.Time = time;
        }

        public override string ToString()
        {
            return String.Format("{0} {1}", this.Town.Name, this.Time);
        }

    }
}
