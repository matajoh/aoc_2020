using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

namespace day9
{
    class Program
    {
        static long Part1(long[] sequence, int preamble_length)
        {
            int preamble_start = 0;
            int preamble_end = preamble_start + preamble_length;
            while(preamble_end < sequence.Length)
            {
                long value = sequence[preamble_end];
                for(int i=preamble_start; i < preamble_end - 1; ++i)
                {
                    long result = value - sequence[i];
                    for(int j=i+1; j<preamble_end; ++j)
                    {
                        if(sequence[j] == result)
                        {
                            goto Found;
                        }
                    }
                }
            
                break;
            Found:
                ++preamble_start;
                ++preamble_end;
            }

            long invalid = sequence[preamble_end];
            Console.WriteLine("Part 1: {0}", invalid);
            return invalid;
        }

        static void Part2(long[] sequence, long target)
        {
            int start = 0;
            int end = 1;
            long sum = sequence[start] + sequence[end];
            while(sum != target)
            {
                end += 1;
                sum += sequence[end];
                while(sum > target)
                {
                    sum -= sequence[start];
                    start += 1;                    
                }
            }

            List<long> range = sequence.Skip(start).Take(end - start).ToList();
            Console.WriteLine("Part 2: {0}", range.Min() + range.Max());
        }

        static void Main(string[] args)
        {
            int preamble = 25;
            if(args.Length == 2){
                preamble = int.Parse(args[1]);
            }

            long[] sequence = File.ReadLines(args[0]).Select(line => long.Parse(line)).ToArray();
            long invalid = Part1(sequence, preamble);
            Part2(sequence, invalid);
        }
    }
}
