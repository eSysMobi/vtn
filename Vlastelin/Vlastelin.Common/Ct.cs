using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Xml;

namespace Vlastelin.Common
{	
	public static class Ct
    {
        public static String DirPositionIP = "ИП";
        public static String DirPositionDirector = "Директор";

        public static String ReportShortDateFormat = "dd.MM.yyyy";
        public static String ReportLongDateFormat = "dd MMM yyyy г.";
        public static String ReportTimeFormat = "HH:mm";
		public static String LongDateTimeFormat = "dd.MM.yyyy HH:mm";

        public static CultureInfo RussianCulture { get { return CultureInfo.GetCultureInfo("ru-RU"); } }

        public static String FullName(String name, String surname, String patronymic)
        {
            return String.Format("{0} {1} {2}", surname, name, patronymic);
        }

        public static String InitialName(String name, String surname, String patronymic)
        {
            String result = surname;
            result += !String.IsNullOrWhiteSpace(name) ? " " + name.First().ToString().ToUpper() + "." : String.Empty;
			result += !String.IsNullOrWhiteSpace(patronymic) ? patronymic.First().ToString().ToUpper() + "." : String.Empty;

            return result;
        }

        /// <summary>
        /// формирует объект даты из строки времени
        /// она имеет формат HH:mm, например 21:00
        /// </summary>
        /// <param name="time">строка времени</param>
        /// <returns>дата</returns>
        public static DateTime GetTimeFromTimeString(String time)
        {
            // формируем строку, которую сможет распознать парсер, для простоты перевода
            // например для 21:00 получится: "1/1/0001 21:00:00", значение которой можно перевести в дату
            DateTime date;
            if (!DateTime.TryParse("1/1/0001 " + time + ":00", RussianCulture, DateTimeStyles.None, out date))
            {
                throw new IncorrectDataException("Время введено неверно");
            }

            return date;
        }

        /// <summary>
        /// получение хэш-кода объектов, учитывая нулл
        /// </summary>
        /// <param name="obj">объект</param>
        /// <returns>хэш код</returns>
        public static int GetHashCode(object obj)
        {
            if (obj == null)
            {
                return 0;
            }
            else
                return obj.GetHashCode();
        }

        /// <summary>
        /// получает описание элемента енума по атрибуту
        /// </summary>
        /// <param name="value">элемент енума</param>
        /// <returns>описание</returns>
        public static string GetDescription(this Enum value)
        {
            DescriptionAttribute[] da =
                       (DescriptionAttribute[])(value.GetType().
                              GetField(value.ToString()).
                                     GetCustomAttributes(typeof(DescriptionAttribute), false));
            return da.Length > 0 ? da[0].Description : value.ToString();
        }

        /// <summary>
        /// получает список описаний енума
        /// </summary>
        /// <param name="enumType">тип енума</param>
        /// <returns>список</returns>
        public static ICollection<String> GetEnumDescriptionValues(Type enumType)
        {
            Array arr = Enum.GetValues(enumType);
            ICollection<String> result = new Collection<String>();

            foreach (Enum item in arr)
            {
                result.Add(item.GetDescription());
            }

            return result;
        }

        /// <summary>
        /// получает элемент енума по описанию (атрибуту)
        /// </summary>
        /// <param name="enumType">тип енума</param>
        /// <param name="description">описание</param>
        /// <returns>элемент енума</returns>
        public static Enum GetEnumFromDescription(Type enumType, String description)
        {
            foreach (Enum item in Enum.GetValues(enumType))
            {
                if (item.GetDescription() == description)
                {
                    return item;
                }
            }
            return null;
        }

		/// <summary>
		/// проверка на заполнение обязательных полей
		/// </summary>
		/// <param name="fieldName">название поля</param>
		/// <param name="value">значение</param>
		public static void CheckRequireField(String fieldName, object value)
		{
			bool error = false;
			if (value is DateTime && (DateTime)value == DateTime.MinValue)
			{
				error = true;
			}else
			if (value == null || String.IsNullOrWhiteSpace(value.ToString()))
			{
				error = true; 
			}
			if (error)
			{
				throw new EmptyRequiredFieldException(String.Format("Поле \"{0}\" является обязательным и должно быть задано", fieldName));
			}
		}
    }
}
