using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace day14
{
    record Assignment(long Address, long Value);

    static class Program
    {
        const int BIT_LENGTH = 36;

        static string parseMask(string line)
        {
            return line.Substring(line.IndexOf('=') + 1).Trim();
        }

        static Assignment parseAssignment(string line)
        {
            int start = 4;
            int end = line.IndexOf(']');
            long address = long.Parse(line.Substring(start, end-start));
            start = line.IndexOf('=') + 1;
            long value = long.Parse(line.Substring(start).Trim());
            return new Assignment(address, value);
        }

        static ArrayList load(string path)
        {
            ArrayList instructions = new ArrayList();
            foreach(string line in File.ReadLines(path))
            {
                if(line.StartsWith("mask")){
                    instructions.Add(parseMask(line));
                }else{
                    instructions.Add(parseAssignment(line));
                }
            }

            return instructions;
        }

        static byte[] toBits(long value)
        {
            List<byte> bytes = new List<byte>();
            for(int i=0; i<BIT_LENGTH; ++i)
            {
                bytes.Add((byte)(value & 0x1));
                value = value >> 1;
            }

            bytes.Reverse();
            return bytes.ToArray();
        }

        static long toLong(byte[] bits)
        {
            long value = 0;
            foreach(var bit in bits)
            {
                value = (value | bit) << 1;
            }

            return value >> 1;
        }

        static List<long> Apply(this string mask, long value, bool floating)
        {
            Queue<(int, byte[])> frontier = new();
            frontier.Enqueue((0, toBits(value)));
            List<long> values = new();
            while(frontier.Count > 0)
            {
                (int index, byte[] bits) = frontier.Dequeue();
                if(index == 36)
                {
                    values.Add(toLong(bits));
                    continue;
                }

                char maskBit = mask[index];
                switch(maskBit)
                {
                    case 'X':
                        if(floating)
                        {
                            byte[] copy = new byte[BIT_LENGTH];
                            Array.Copy(bits, copy, BIT_LENGTH);
                            bits[index] = 0;
                            copy[index] = 1;
                            frontier.Enqueue((index + 1, bits));
                            frontier.Enqueue((index + 1, copy));
                        }
                        else
                        {
                            frontier.Enqueue((index + 1, bits));
                        }
                        break;
                    
                    case '0':
                        if(!floating)
                        {
                            bits[index] = 0;
                        }

                        frontier.Enqueue((index + 1, bits));
                        break;
                    
                    case '1':
                        bits[index] = 1;
                        frontier.Enqueue((index + 1, bits));
                        break;
                }
            }

            return values;
        }

        static long Execute(ArrayList instructions, bool addressMode)
        {
            Dictionary<long, long> memory = new();
            string mask = null;
            foreach(var inst in instructions)
            {
                if(inst is Assignment)
                {
                    Assignment assignment = inst as Assignment;
                    if(addressMode)
                    {
                        List<long> addresses = mask.Apply(assignment.Address, true);
                        foreach(var address in addresses)
                        {
                            memory[address] = assignment.Value;
                        }
                    }   
                    else
                    {
                        memory[assignment.Address] = mask.Apply(assignment.Value, false)[0];
                    }
                }
                else
                {
                    mask = inst as string;
                }
            }

            return memory.Sum(kv => kv.Value);
        }

        static void Main(string[] args)
        {
            ArrayList instructions = load(args[0]);
            Console.WriteLine("Part 1: {0}", Execute(instructions, false));
            Console.WriteLine("Part 2: {0}", Execute(instructions, true));
        }
    }
}
