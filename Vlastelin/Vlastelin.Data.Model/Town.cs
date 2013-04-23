using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vlastelin.Common.Attributes;
using System.Text.RegularExpressions;
using Vlastelin.Data.Model.Util;
using Vlastelin.Common;
using System.Runtime.CompilerServices;

namespace Vlastelin.Data.Model
{
    public sealed class Town : BaseItem
    {
        private string _name;

        [FieldName("Name")]
        public string Name
        {
            get
            {
                return this._name;
            }
            set
            {
				Ct.CheckRequireField("Название", value);
				if (String.IsNullOrWhiteSpace(value))
                {
                    throw new IncorrectDataException("Название города должно быть введено");
                }
                this._name = value;
            }
        }

        [FieldName("Prefix")]
        public string Prefix { get; set; }

        [FieldName("LastNumber")]
        public int LastNumber { get; set; }

        public override bool Equals(object obj)
        {
            if (!(obj is Town)) return false;
            Town t = obj as Town;
            return 
                base.Equals(obj) && 
                t.Name == this.Name && 
				t.Prefix == this.Prefix;
				//t.LastNumber.Equals(this.LastNumber);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode() ^ Ct.GetHashCode(this.Name) ^ Ct.GetHashCode(Prefix) ^ Ct.GetHashCode(LastNumber);
        }

        public override string ToString()
        {
            return String.Format("[ID {0}] {1}", base.Id, this.Name);
        }
    }
}
