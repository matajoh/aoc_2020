using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

namespace day5
{
    static class Program
    {
        static int Id(this (int row, int column) seat)
        {
            return seat.row * 8 + seat.column;
        }

        static int getValue(Queue<char> code, int start, int end, char lower, char upper)
        {
            while(code.Count != 0){
                char next = code.Dequeue();
                int mid = (start + end) >> 1;
                if(next == lower)
                {
                    end = mid;
                }
                else
                {
                    start = mid;
                }
            }

            return start;
        }

        static (int row, int column) ParseSeat(string code)
        {
            int row = getValue(new Queue<char>(code.Substring(0, 7)), 0, 128, 'F', 'B');
            int column = getValue(new Queue<char>(code.Substring(7)), 0, 8, 'L', 'R');
            return (row, column);
        }

        static void Part1(List<(int row, int column)> seats)
        {
            Console.WriteLine("Part 1: {0}", seats.Max(seat => seat.Id()));
        }

        static void Part2(List<(int row, int column)> seats)
        {
            var ids = seats.Select(seat => seat.Id()).OrderBy(id => id).ToList();
            for(int i=1; i<ids.Count - 1; ++i)
            {
                if(ids[i] + 2 == ids[i+1]){
                    Console.WriteLine("Part 2: {0}", ids[i] + 1);
                    return;
                }
            }
        }

        static void Main(string[] args)
        {
            var seats = File.ReadLines(args[0]).Select(line => ParseSeat(line)).ToList();
            Part1(seats);
            Part2(seats);
        }
    }
}
