using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Vlastelin.Common.Attributes
{
    /// <summary>
    /// Attribute which helps to identificate data field name with complex data
    /// </summary>
    [AttributeUsageAttribute(AttributeTargets.Field | AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
    public class FieldIsSerializedAttribute : Attribute
    {
    }
}
