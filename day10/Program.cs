using System;
using System.Linq;
using System.IO;
using System.Collections.Generic;

namespace day10
{
    class Program
    {
        static void Part1(List<int> adapters)
        {
            Dictionary<int, int> diff_count = adapters.Take(adapters.Count - 1)
                                                      .Zip(adapters.Skip(1), (x, y) => {return y - x;})
                                                      .GroupBy(diff => diff)
                                                      .ToDictionary(group => group.Key, group => group.Count());
            Console.WriteLine("Part 1: {0}", diff_count[1] * diff_count[3]);
        }

        static long countCombos(List<int> adapters, Dictionary<int, long> comboCounts, int index)
        {
            if(index == adapters.Count - 1)
            {
                return 1;
            }

            if(!comboCounts.ContainsKey(index))
            {
                List<long> options = new();
                int value = adapters[index];
                for(int i=index+1; i < adapters.Count && adapters[i] <= value + 3; ++i)
                {
                    options.Add(countCombos(adapters, comboCounts, i));
                }

                comboCounts[index] = options.Sum();                
            }

            return comboCounts[index];
        }

        static void Part2(List<int> adapters)
        {
            Dictionary<int, long> comboCounts = new();
            Console.WriteLine("Part 2: {0}", countCombos(adapters, new Dictionary<int, long>(), 0));
        }

        static void Main(string[] args)
        {
            List<int> adapters = File.ReadLines(args[0])
                                 .Select(line => int.Parse(line))
                                 .OrderBy(adapter => adapter)
                                 .ToList();
            adapters.Insert(0, 0);
            adapters.Add(adapters.Last() + 3);
            Part1(adapters);
            Part2(adapters);
        }
    }
}
