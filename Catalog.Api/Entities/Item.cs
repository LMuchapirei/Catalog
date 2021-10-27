using System;

namespace Catalog.Api.Entities
{
    public record Item
    {
        public Guid Id { get; init; }
        public string Name { get; init; }
        public decimal Price { get; init; }
        public DateTimeOffset CreatedDate { get; init; }
    }
}




/*
Record Types provide us with the following
1. Use for immutable objects
2. With-expressions support
3. Value-based equality support
4. Init ony properties
  Demoed here below
  
  Item potion1 = new () { Name="Potion", Price=9};
  Item potion2= new () { Name="Potion", Price=9};
  bool areEqual= potion1==potion2;  // true  point 3 All properties of the two items are equivalent
  
  point 1
  Item potion3 = potion1 with { Price = 15 };
  areEqual = potion1 == potion3; // = false; 
  
5. Only set the property when the item is being created

We can do this 
Item item=new ()
{
    Id= Guid.NewGuid()
};

// But this is not allowed 
item.Id=Guid.NewGuid();
*/ 