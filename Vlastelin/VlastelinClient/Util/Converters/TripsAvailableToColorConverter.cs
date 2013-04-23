using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows.Media;
using VlastelinClient.ViewModel.ObjectsViewModel;
using Vlastelin.Data.Model;
using System.Threading;

namespace VlastelinClient.Util.Converters
{
    public class TripsAvailableToColorConverter : IMultiValueConverter
    {
        #region IValueConverter Members

        /// <summary>
        /// конвертор позволяет выделять цвет даты в календаре в зависимости от наличия на этот день поездок
        /// </summary>
        /// <param name="value">1 - список поездок, 2 - выделеный маршрут, 3 - дата в календаре</param>
        /// <param name="targetType">тип</param>
        /// <param name="parameter">не используется</param>
        /// <param name="culture">не используется</param>
        /// <returns>цвет ячейки в календаре</returns>
        public object Convert(object[] value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value != null)
            {
                int c = value.Count();
                bool res = false;

                // выделяет даты в зависимости от маршрута и даты
                if (c == 4)
                {
                    // получаем объекты из конвертора, ищем количество совпадений и возвращаем цвет
                    // нет поездок - белый

                    IEnumerable<StationScheduleVM> sh = value[0] as IEnumerable<StationScheduleVM>;
                    TripVM trip = value[1] as TripVM;
                    TownVM dep = value[2] as TownVM;
                    DateTime date = (DateTime)value[3];

                    if (sh != null && dep != null && sh.Count() > 0 && trip != null)
                    {
                        // получаем действительный список расписаний для выбранного города и маршрута
                        IEnumerable<StationScheduleVM> currentSchedules = sh.Where(s => s.Town.Equals(dep) && s.TS.Trip.Equals(trip));
                        
                        // если расписания есть и хоть какое-нибудь удовлетворяет условиям даты, то расписания для данной даты доступны
                        res =  (currentSchedules.Count() > 0 && currentSchedules.Any(s => UtilManager.IsCalendarDateMatch(s.TS.ScheduleType, date, s.DepartureTime)));
                    }
                }

                // цвет даты в зависимости от наличия поездок
                if (res) return new BrushConverter().ConvertFromString(Properties.Settings.Default.ColorCalendarSelectDate);
            }
                      
            return Brushes.White;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException("This converter do not support backward conversion.");
        }

        #endregion
    }
}
