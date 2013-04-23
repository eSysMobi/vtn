using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace VlastelinServer.DAO.DBAccess
{
    /// <summary>
    /// ItemDTOInfo class
    /// </summary>
    public class ItemDTOInfo
    {
        /// <summary>
        /// Свойства типа заполняемые из БД.
        /// </summary>
        public DTOProperty[] Properties
        {
            get;
            private set;
        }

        /// <summary>
        /// Конструктор.
        /// </summary>
        /// <param name="type">Исследуемый тип.</param>
        public ItemDTOInfo(Type type)
        {
            PropertyInfo[] properties = type.GetProperties();
            List<DTOProperty> propertiesList = new List<DTOProperty>(properties.Length);
            foreach (PropertyInfo pInfo in properties)
            {
                DTOProperty p = new DTOProperty(pInfo);
                if (p.FieldNameAttribute != null)
                {
                    propertiesList.Add(p);
                }
            }
            this.Properties = propertiesList.ToArray();
        }
    }

}
