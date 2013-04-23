using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using Vlastelin.Common.Attributes;

namespace VlastelinServer.DAO.DBAccess
{
    /// <summary>
    /// Данные о свойстве DTO-объекта.
    /// </summary>
    public class DTOProperty
    {
        /// <summary>
        /// Значение свойства было сериализовано при передаче.
        /// </summary>
        public readonly bool IsSerializedField;

        /// <summary>
        /// Атрибут содержащий имя поля в БД.
        /// </summary>
        public FieldNameAttribute FieldNameAttribute
        {
            get;
            private set;
        }

        /// <summary>
        /// "Свойство".
        /// </summary>
        public PropertyInfo PropertyInfo
        {
            get;
            private set;
        }

        /// <summary>
        /// Конструктор.
        /// </summary>
        /// <param name="pInfo">Исследуемое "свойство".</param>
        public DTOProperty(PropertyInfo pInfo)
        {
            this.PropertyInfo = pInfo;
            object[] attrs = this.PropertyInfo.GetCustomAttributes(typeof(FieldNameAttribute), false);
            if (attrs.Length != 0)
            {
                this.FieldNameAttribute = attrs[0] as FieldNameAttribute;
            }
            if (this.PropertyInfo.GetCustomAttributes(typeof(FieldIsSerializedAttribute), false).Length != 0)
            {
                this.IsSerializedField = true;
            }
        }

        /// <summary>
        /// Устанавливает значение свойства.
        /// </summary>
        /// <param name="obj">Объект, для которого устанавливается значение свойства.</param>
        /// <param name="value">Значение свойства.</param>
        public void SetValue(object obj, object value)
        {
            this.PropertyInfo.SetValue(obj, value, null);
        }

        /// <summary>
        /// Устанавливает значение свойства.
        /// </summary>
        /// <param name="obj">Объект, для которого устанавливается значение свойства.</param>
        /// <param name="value">Значение свойства.</param>
        /// <param name="index"></param>
        public void SetValue(object obj, object value, object[] index)
        {
            this.PropertyInfo.SetValue(obj, value, index);
        }
    }

}
