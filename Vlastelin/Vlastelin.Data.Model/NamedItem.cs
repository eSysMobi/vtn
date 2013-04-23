using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vlastelin.Common.Attributes;

namespace Vlastelin.Data.Model
{
    public class NamedItem: BaseItem
    {
        [FieldName("Name")]
        public string Name { get; set; }



        public override int GetHashCode()
        {
            return base.GetHashCode() ^ Name.GetHashCode();
        }
        public override bool Equals(object obj)
        {
            if (!(obj is NamedItem)) return false;

            return base.Equals(obj) && (obj as NamedItem).Name.Equals(this.Name);
        }

        public override string ToString()
        {
            return String.Format("[ID {0}] {1}", base.Id, this.Name);
        }
    }
}
