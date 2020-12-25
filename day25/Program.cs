using System;
using System.IO;
using System.Linq;

namespace day25
{
    class Program
    {
        const long Modulo = 20201227;

        static long Power(long x, long y, long p)
        {
            long res = 1;
            x = x % p;

            if(x == 0)
            {
                return 0;
            }

            while(y > 0)
            {
                if ((y & 1) == 1)
                    res = (res * x) % p;

                y = y >> 1;
                x = (x * x) % p;
            }

            return res;
        }

        static (long, long) Load(string path)
        {
            long[] values = File.ReadLines(path).Select(line => long.Parse(line)).ToArray();
            return (values[0], values[1]);
        }

        static long ComputeLoopNumber(long key)
        {
            long loopNumber = 1;
            while (true)
            {
                if (Power(7, loopNumber, Modulo) == key)
                {
                    break;
                }

                loopNumber += 1;
            }

            return loopNumber;
        }

        static void Part1(long cardKey, long doorKey)
        {
            long cardLoopNumber = ComputeLoopNumber(cardKey);
            long encryptionKey = Power(doorKey, cardLoopNumber, Modulo);
            Console.WriteLine("Part 1: {0}", encryptionKey);
        }

        static void Main(string[] args)
        {
            (long cardKey, long doorKey) = Load(args[0]);
            Part1(cardKey, doorKey);
        }
    }
}
