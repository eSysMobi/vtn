using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vlastelin.Common.Attributes;

namespace Vlastelin.Data.Model
{
    public class TablesTime
    {
        [FieldName("tName")]
        public string tName { get; set; }

        [FieldName("lastModifiedTime")]
        public DateTime LastModifiedTime { get; set; }
    }
}
