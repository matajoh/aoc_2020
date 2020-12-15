using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace day15
{
    class Program
    {
        static long GetIndex(long[] prefix, long index)
        {
            Dictionary<long, long> lookup = new();
            long spoken = -1;
            for(int i=0; i<prefix.Length; ++i)
            {
                spoken = prefix[i];
                lookup[prefix[i]] = i;
            }

            for(long i=prefix.Length; i < index; ++i)
            {
                long next;
                if(lookup.ContainsKey(spoken))
                {
                    next = i - 1 - lookup[spoken];
                }
                else
                {
                    next = 0;
                }

                lookup[spoken] = i - 1;
                spoken = next;
            }

            return spoken;
        }

        static void Main(string[] args)
        {
            foreach(string line in File.ReadLines(args[0]))
            {
                var prefix = line.Split(',').Select(item => long.Parse(item)).ToArray();
                Console.WriteLine(line);
                Console.WriteLine("Part 1: {0}", GetIndex(prefix, 2020));
                Console.WriteLine("Part 2: {0}", GetIndex(prefix, 30000000));
            }
        }
    }
}
