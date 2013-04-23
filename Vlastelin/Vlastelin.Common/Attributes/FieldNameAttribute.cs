using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Vlastelin.Common.Attributes
{
    /// <summary>
    /// Attribute which helps to identificate data field name 
    /// </summary>
    [AttributeUsageAttribute(AttributeTargets.Field | AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
    public class FieldNameAttribute : Attribute
    {
        /// <summary>
        /// Data field name
        /// </summary>
        protected string fieldName;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="fieldName">Data field name</param>
        public FieldNameAttribute(string fieldName)
        {
            this.fieldName = fieldName;
        }

        /// <summary>
        /// Data field name
        /// </summary>
        public string FieldName
        {
            get
            {
                return fieldName;
            }
            set
            {
                fieldName = value;
            }
        }
    }

}
