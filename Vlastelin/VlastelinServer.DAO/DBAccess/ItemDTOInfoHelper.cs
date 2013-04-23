using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VlastelinServer.DAO.DBAccess
{
    /// <summary>
    /// Кэш для работы с типами.
    /// </summary>
    public static class ItemDTOInfoHelper
    {
        /// <summary>
        /// Коллекция исследованных объектов.
        /// </summary>
        private static Dictionary<Type, ItemDTOInfo> _items;

        /// <summary>
        /// Объект синхронизации дотупа к коллекции.
        /// </summary>
        private static object _synk;

        /// <summary>
        /// Конструктор - инициализация статических полей.
        /// </summary>
        static ItemDTOInfoHelper()
        {
            ItemDTOInfoHelper._synk = new object();
            ItemDTOInfoHelper._items = new Dictionary<Type, ItemDTOInfo>();
        }

        /// <summary>
        /// Получить информацию о типе.
        /// </summary>
        /// <param name="type">Исследуемый тип.</param>
        /// <returns>Информация о типе.</returns>
        public static ItemDTOInfo GetItemDTOInfo(Type type)
        {
            ItemDTOInfo itemInfo;
            if (!ItemDTOInfoHelper._items.TryGetValue(type, out itemInfo))
            {
                lock (ItemDTOInfoHelper._synk)
                {
                    if (!ItemDTOInfoHelper._items.TryGetValue(type, out itemInfo))
                    {
                        itemInfo = new ItemDTOInfo(type);
                        ItemDTOInfoHelper._items.Add(type, itemInfo);
                    }
                }
            }
            return itemInfo;
        }
    }

}
