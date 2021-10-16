using System;
using System.Collections.Generic;
using Catalog.Entities;

namespace Catalog.Repositories
{
    public interface IItemsRepository
    {
        IEnumerable<Item> GetItems();
        Item GetItem(Guid id);
    }
}