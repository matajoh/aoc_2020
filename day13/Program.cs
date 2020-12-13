using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;

namespace day13
{
    static class Program
    {
        record Bus(int ID, int Offset);

        static (int, List<Bus>) load(string path)
        {
            var lines = File.ReadAllLines(path);
            int departureTime = int.Parse(lines[0]);
            List<Bus> buses = new();
            string[] parts = lines[1].Split(',');
            for(int i=0; i<parts.Length; ++i)
            {
                int id;
                if(int.TryParse(parts[i], out id))
                {
                    buses.Add(new Bus(id, i));
                }
            }

            return (departureTime, buses);
        }

        static (Bus bus, int time) findNextTime(this Bus bus, int departureTime)
        {
            int previousTime = (departureTime / bus.ID) * bus.ID;
            return (bus, previousTime + bus.ID);
        }

        static void Part1(int departureTime, List<Bus> buses)
        {
            (Bus bus, int time) nextTime = buses.Select(bus => bus.findNextTime(departureTime))
                                                .OrderBy(nextTime => nextTime.time)
                                                .First();
            
            Console.WriteLine("Part 1: {0}", nextTime.bus.ID * (nextTime.time - departureTime));
        }

        static (BigInteger, BigInteger) bezout_coeffs(BigInteger a, BigInteger b)
        {
            (BigInteger old_r, BigInteger r) = (a, b);
            (BigInteger old_s, BigInteger s) = (1, 0);
            (BigInteger old_t, BigInteger t) = (0, 1);
            
            while(r != 0)
            {
                BigInteger quotient = old_r / r;
                (old_r, r) = (r, old_r - quotient * r);
                (old_s, s) = (s, old_s - quotient * s);
                (old_t, t) = (t, old_t - quotient * t);
            }

            return (old_s, old_t);
        }

        static void Part2(List<Bus> buses)
        {
            BigInteger maxOffset = buses.Max(bus => bus.Offset);
            BigInteger n1 = buses[0].ID;
            BigInteger a1 = maxOffset - buses[0].Offset;
            for(int i=1; i<buses.Count; ++i)
            {
                BigInteger n2 = buses[i].ID;
                BigInteger a2 = maxOffset - buses[i].Offset;
                (BigInteger m1, BigInteger m2) = bezout_coeffs(n1, n2);
                a1 = m2*n2*a1 + m1*n1*a2;
                n1 = n1 * n2;
                a1 = a1 % n1;
                if(a1 < 0)
                {
                    a1 += n1;
                }
            }

            Console.WriteLine("Part 2: {0}", (a1 - maxOffset) % n1);     
        }

        static void Main(string[] args)
        {
            (int departureTime, List<Bus> buses) = load(args[0]);
            Part1(departureTime, buses);
            Part2(buses);
        }
    }
}
