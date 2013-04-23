using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Vlastelin.Data.Model.Util
{
    /// <summary>
    /// класс хранит регулярные выражения для объектов
    /// </summary>
    public static class RegExp
    {
        //// города
        //public static String RegexpName = "^[А-Я]{1}[а-я]*([- ][А-Я])?[а-я]*$";

        //автобусы
        //public static String RegexpBusRegNumber = @"^[АВЕКМНОРСТУХавекмнорстухABEKMHOPCTYX]{2}[ ]?\d{3}[ ]?([ ]?\d{1,3}[ ]?[a-zA-Zа-яА-Я]{1,4})?$";
        public static String RegexpBusRegNumber = @"^[АВЕКМНОРСТУХавекмнорстухABEKMHOPCTYX]{1,2}[ ]?\d{3,4}[ ]?[АВЕКМНОРСТУХавекмнорстухABEKMHOPCTYX]{0,2}([ ]?\d{1,3}[ ]?[a-zA-Zа-яА-Я]{1,4})?$";

        //инициалы
        public static String RegexpPersonName = @"^[А-ЯA-Z]{1}[а-яa-z -]*$";

        //паспорт
        public static String RegexpPassportSer = @"^\d{0,5}$";
        public static String RegexpPassportNum = @"^\d{0,10}$";

        //владелец
        public static String RegexpLongNumber = @"^\d+$";

        public static String RegexpPhone = @"^[+]?[\d -]*$";
    }
}
