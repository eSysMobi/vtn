using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vlastelin.Common.Attributes;
using System.Reflection;

namespace Vlastelin.Data.Model
{
    /// <summary>
    /// базовый класс для элементов (города, автобусы итд)
    /// </summary>
    public class BaseItem:IBaseItem
    {
        /// <summary>
        /// идентификатор из базы данных
        /// </summary>
        [FieldName("id")]
        public long Id { get; set; }

        private object GetPropertyValue(object obj, String name)
        {
            PropertyInfo prop = this.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public).FirstOrDefault(p => p.Name == name);
            return prop == null ? null : prop.GetValue(obj, null);
        }

        /// <summary>
        /// копирование объекта по значению
        /// </summary>
        /// <param name="item">объект</param>
        public virtual void CopyFrom(BaseItem item)
        {
            foreach (PropertyInfo prop in this.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public))
            {
                if (prop.GetCustomAttributes(typeof(FieldNameAttribute), true).FirstOrDefault() != null)
                {
                    prop.SetValue(this, this.GetPropertyValue(item, prop.Name), null);
                }
            }
        }

        #region переопредленные методы

        public override string ToString()
        {
            return String.Format("[ID {0}]", this.Id);
        }

        public override bool Equals(object obj)
        {
            if (!(obj is BaseItem)) return false;
            return this.Id == (obj as BaseItem).Id;
        }

        public override int GetHashCode()
        {
            return (int)this.Id;
        }

        #endregion
    }
}
