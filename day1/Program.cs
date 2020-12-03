using System;
using System.IO;
using System.Linq;

namespace day1
{
    class Program
    {
        const int VALUE = 2020;

        static void Part1(int[] values)
        {
            int right = values.Length - 1;
            for(int left=0; left < values.Length; ++left)
            {
                int other = VALUE - values[left];
                while(values[right] > other)
                {
                    --right;
                }

                if(values[right] == other)
                {
                    Console.WriteLine("Part 1: {0}", values[left] * values[right]);
                    return;
                }
            }

            throw new Exception("Pair not found.");
        }

        static void Part2(int[] values)
        {
            int rightStart = values.Length - 1;
            for(int left=0; left < values.Length; ++left)
            {
                int remainder = VALUE - values[left];
                while(values[rightStart] >= remainder)
                {
                    --rightStart;
                }

                int right = rightStart;

                for(int mid=left; mid < values.Length; ++mid)
                {
                    remainder = VALUE - values[left] - values[mid];
                    if(remainder < values[left])
                    {
                        break;
                    }

                    while(values[right] > remainder)
                    {
                        --right;
                    }

                    if(values[right] == remainder)
                    {
                        Console.Write("Part 2: {0}", values[left] * values[mid] * values[right]);
                        return;
                    }
                }
            }

            throw new Exception("Triplet not found.");
        }

        static void Main(string[] args)
        {
            int[] values = File.ReadLines(args[0]).Select(s => int.Parse(s.Trim())).ToArray();
            Array.Sort(values);
            Part1(values);
            Part2(values);
        }
    }
}
