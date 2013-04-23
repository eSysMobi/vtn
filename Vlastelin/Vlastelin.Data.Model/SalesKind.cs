using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vlastelin.Common.Attributes;

namespace Vlastelin.Data.Model
{
    public class SalesKind:NamedItem, IItemForSale
    {
        [FieldName("Price")]
        public double Price { get; set; }

        public static SalesKind Ticket 
        { 
            get 
            { 
                return new SalesKind() 
                { 
                    Id = 1,
                    Name = "Билет", /*должно быть в точности как в базе*/
                    Price = 0 
                }; 
            } 
        }

        public static SalesKind ReturnComission
        {
            get
            {
                return new SalesKind()
                {
                    Id = 4,
                    Name = "Удержание при возврате", /*должно быть в точности как в базе*/
                    Price = 0
                };
            }
        }

        public override bool Equals(object obj)
        {
            if (!(obj is SalesKind)) return false;
            return base.Equals(obj) && (obj as SalesKind).Price.Equals(this.Price);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode() ^ Price.GetHashCode();
        }

        public override string ToString()
        {
            return base.ToString();
        }
    }
}
