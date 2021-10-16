using System;
using System.Collections.Generic;
using System.Linq;
using Catalog.Entities;

namespace Catalog.Repositories
{
    public class InMemItemsRepository : IItemsRepository
    {
        private readonly List<Item> _items = new() // Target-typed new expression
        {
            new Item {Id=Guid.NewGuid(),Name = "Potion",Price = 9,CreatedDate = DateTimeOffset.UtcNow},
            new Item {Id=Guid.NewGuid(),Name = "Iron Sword",Price = 20,CreatedDate = DateTimeOffset.UtcNow},
            new Item {Id=Guid.NewGuid(),Name = "Bronze Shield",Price = 18,CreatedDate = DateTimeOffset.UtcNow},
            new Item {Id=Guid.NewGuid(),Name = "Heat Map",Price = 9,CreatedDate = DateTimeOffset.UtcNow}
        };

        public IEnumerable<Item> GetItems()
        {
            return _items;
        }

        public Item GetItem(Guid id)
        {
            // items.Where(item=>item.id==id).SingleOrDefault();
            return _items.SingleOrDefault(item => item.Id == id);
        }

        public void CreateItem(Item item)
        {
            _items.Add(item);
        }

        public void UpdateItem(Item item)
        {
            var index = _items.FindIndex(existingItem => existingItem.Id == item.Id);
            _items[index] = item;
        }
    }
}