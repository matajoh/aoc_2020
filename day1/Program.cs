using System;
using System.IO;
using System.Linq;

namespace day1
{
    class Program
    {
        static Tuple<int, int> part1(int[] values)
        {
            for(int i=0; i<values.Length; ++i)
            {
                for(int j=i + 1; j < values.Length; ++j)
                {
                    if(values[i] + values[j] == 2020)
                    {
                        return new Tuple<int, int>(values[i], values[j]);
                    }
                }
            }
            
            throw new Exception("Pair not found");
        }

        static Tuple<int, int, int> part2(int[] values)
        {
            for(int i=0; i<values.Length; ++i)
            {
                int value0 = values[i];
                int value0_2 = value0 + value0;
                for(int j=i; j<values.Length; ++j)
                {
                    int value1 = values[j];
                    int value01 = value0 + value1;
                    if(value01 + value0 == 2020)
                    {
                        return new Tuple<int, int, int>(value0, value0, value1);
                    }

                    for(int k=j; k < values.Length; ++k)
                    {
                        int value2 = values[k];
                        if(value01 + value2 == 2020)
                        {
                            return new Tuple<int, int, int>(value0, value1, value2);
                        }
                    }
                }
            }

            throw new Exception("Triplet not found");
        }

        static void Main(string[] args)
        {
            int[] values = File.ReadLines(args[0]).Select(s => int.Parse(s.Trim())).ToArray();
            var pair = part1(values);
            Console.WriteLine("Part 1: {0}", pair.Item1 * pair.Item2);
            var triplet = part2(values);
            Console.WriteLine("Part 2: {0}", triplet.Item1 * triplet.Item2 * triplet.Item3);
        }
    }
}
