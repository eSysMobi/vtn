using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Vlastelin.Data.Model
{
    /// <summary>
    /// Интерфейс сугубо для объединения 2х принципильно разных классов без внедрения в них общих элементов
    /// </summary>
    public interface IItemForSale:IBaseItem
    {
    }
}
