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
            return string.Equals(Name, other.Name) && Price == other.Price && Count == other.Count;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is Item && Equals((Item) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = Name?.GetHashCode() ?? 0;
                hashCode = (hashCode*397) ^ Price.GetHashCode();
                hashCode = (hashCode*397) ^ Count;
                return hashCode;
            }
        }
    }
}
