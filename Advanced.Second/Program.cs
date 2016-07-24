using System;
using System.Collections.Generic;
using System.Linq;

namespace Advanced.Second
{
    public class Program
    {
        static void Main(string[] args)
        {
            var items = new List<Item>
            {
                new Item("Hat", decimal.MaxValue, int.MaxValue),
                new Item("Cat", decimal.MinValue, int.MinValue),
                new Item("Hat", decimal.MaxValue, int.MaxValue),
            };
            foreach (var item in items.Distinct())
            {
                Console.WriteLine(item.Name);
            }

            Console.ReadKey();
        }
    }

    public struct Item : IEquatable<Item>
    {
        public Item(string name, decimal price, int count)
        {
            Name = name;
            Price = price;
            Count = count;
        }

        public string Name { get; }

        public decimal Price { get; }

        public int Count { get; }

        public bool Equals(Item other)
        {
            return Name == other.Name;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Item))
            {
                return false;
            }

            return Equals((Item)obj);
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }
    }
}
