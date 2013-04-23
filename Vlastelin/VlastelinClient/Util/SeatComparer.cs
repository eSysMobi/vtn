using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VlastelinClient.ViewModel.ObjectsViewModel;

namespace VlastelinClient.Util
{
    public class SeatComparer : IEqualityComparer<SeatVM>
    {
        public bool Equals(SeatVM x, SeatVM y)
        {
            return x != null && y != null ? x.Equals(y) && x.SeatNumber == y.SeatNumber : false;
        }

        public int GetHashCode(SeatVM obj)
        {
            return (int)(obj != null ? obj.seat.Id : 0);
        }
    }
}
