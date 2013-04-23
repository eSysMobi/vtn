using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using Vlastelin.Data.Model;
using System.Windows;

namespace VlastelinClient
{
    /// <summary>
    /// настройки цветов
    /// </summary>
    public static class ConfiguratonSettings
    {
        /// <summary>
        /// цвет выделения купленного места
        /// </summary>
        public static String ColorSeatBought
        {
            get
            {
                return Properties.Settings.Default.ColorSeatBought;
            }
        }

        /// <summary>
        /// цвет залоченного места
        /// </summary>
        public static String ColorSeatLocked
        {
            get
            {
                return Properties.Settings.Default.ColorSeatLocked;
            }
        }

        /// <summary>
        /// цвет забронированного места
        /// </summary>
        public static String ColorSeatReserved
        {
            get
            {
                return Properties.Settings.Default.ColorSeatReserved;
            }
        }

        /// <summary>
        /// цвет строки отправленного автобуса
        /// </summary>
        public static String ColorBusDepSelection
        {
            get
            {
                return Properties.Settings.Default.ColorBusDepSelection;
            }
        }

		/// <summary>
		/// цвет рамки текущего дня календаря
		/// </summary>
		public static String ColorCurrentDateBorder
		{
			get
			{
				return Properties.Settings.Default.ColorCurrentDateBorder;
			}
		}

        /// <summary>
        /// конвертор цветов
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private static Color ColorConvert(String value)
        {
            Color c = (Color)ColorConverter.ConvertFromString(value);
           return c;
        }
    }
}
