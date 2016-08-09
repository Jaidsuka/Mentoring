using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EvictionPolicyDictionary.Dictionary;

namespace EvictionPolicyDictionary
{
    class Program
    {
        static void Main(string[] args)
        {
            LRUTest();
            Console.ReadKey();
            Console.Clear();
            PeriodTimeTest();
            Console.ReadKey();
        }

        private static async void PeriodTimeTest()
        {
            IDictionary<int, string> dictionary = new ExpirableDictionary<int, string>(new TimePeriodDiscardingEvictionPolicy<int, string>(TimeSpan.FromSeconds(5)));

            dictionary.Add(1, "One");
            Console.WriteLine("One added");
            await Task.Delay(1500);

            dictionary.Add(2, "Two");
            Console.WriteLine("Two added");
            await Task.Delay(1500);

            dictionary.Add(3, "Three");
            Console.WriteLine("Three added");
            await Task.Delay(1500);

            Console.WriteLine("Getting the first one and waiting for 3 seconds. Three and one should be there.");
            Console.WriteLine(dictionary[1]);

            await Task.Delay(3000);
            Console.WriteLine("All records in the dictionary:");
            foreach (var pair in dictionary)
            {
                Console.WriteLine($"Key: {pair.Key}, Value: {pair.Value}");
            }
        }

        static async void LRUTest()
        {
            IDictionary<int, string> dictionary = new ExpirableDictionary<int, string>(new LeastRecentlyUsedDiscardingEvictionPolicy<int, string>(3));

            dictionary.Add(1, "One");
            Console.WriteLine("One added");
            await Task.Delay(1500);

            dictionary.Add(2, "Two");
            Console.WriteLine("Two added");
            await Task.Delay(1500);

            dictionary.Add(3, "Three");
            Console.WriteLine("Three added");
            await Task.Delay(1500);

            Console.WriteLine("Get first value");
            Console.WriteLine(dictionary[1]);

            await Task.Delay(1500);

            Console.WriteLine("Get second value");
            Console.WriteLine(dictionary[2]);

            await Task.Delay(1500);

            Console.WriteLine("Add one more -> First and seconds should stay, third should be deleted");
            dictionary.Add(4, "Four");

            await Task.Delay(1500);

            Console.WriteLine("Get all values:");
            foreach (var pair in dictionary)
            {
                Console.WriteLine($"Key: {pair.Key}, Value: {pair.Value}");
            }
        }
    }
}
