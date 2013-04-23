using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vlastelin.Common.Attributes;

namespace Vlastelin.Data.Model
{
    public class VlastelinRole
    {
        public long Id { get; set; }

        [FieldName("ApplicationName")]
        public string ApplicationName { get; set; }

        [FieldName("RoleName")]
        public string Name { get; set; }
    }
}
