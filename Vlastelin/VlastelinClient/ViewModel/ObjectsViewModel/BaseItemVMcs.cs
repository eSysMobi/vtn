using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vlastelin.Data.Model;

namespace VlastelinClient.ViewModel.ObjectsViewModel
{
    public class BaseItemVM : BaseViewModel
    {
        protected BaseItem item;

        public virtual void CopyFrom(BaseItemVM itm)
        {
            this.item.CopyFrom(itm.item);
            this.Refresh();
        }

        #region переопредленные методы

        public override string ToString()
        {
            return this.item.ToString();
        }

        public override bool Equals(object obj)
        {
            BaseItemVM bs = obj as BaseItemVM;
            return bs == null ? false : this.item.Equals(bs.item);
        }

        public override int GetHashCode()
        {
            return this.item.GetHashCode();
        }

        public virtual bool EqualsById(BaseItemVM obj)
        {
            return obj == null ? false : obj.item.Id.Equals(this.item.Id);
        }

        #endregion

    }
}
