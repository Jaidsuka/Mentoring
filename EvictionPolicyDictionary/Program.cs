using System;
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
        }

        static async void LRUTest()
        {
            IEvictionPolicy<int, string> dictionary = new LeastRecentlyUsedDiscardingDictionary<int, string>(3);

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

            Console.WriteLine("Add one more -> First one should stay, third should be deleted");
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
