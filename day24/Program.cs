using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace day24
{
    record HexCoordinate(int X, int Y, int Z);

    static class Program
    {
        static HexCoordinate Add(this HexCoordinate lhs, HexCoordinate rhs)
        {
            return new HexCoordinate(lhs.X + rhs.X, lhs.Y + rhs.Y, lhs.Z + rhs.Z);
        }

        static HexCoordinate East = new HexCoordinate(1, 0, -1);
        static HexCoordinate NorthEast = new HexCoordinate(0, 1, -1);
        static HexCoordinate NorthWest = new HexCoordinate(-1, 1, 0);
        static HexCoordinate West = new HexCoordinate(-1, 0, 1);
        static HexCoordinate SouthWest = new HexCoordinate(0, -1, 1);
        static HexCoordinate SouthEast = new HexCoordinate(1, -1, 0);

        static HexCoordinate Parse(string line)
        {
            Queue<char> tokens = new Queue<char>(line);
            HexCoordinate hex = new HexCoordinate(0, 0, 0);
            while(tokens.Count > 0)
            {
                char token = tokens.Dequeue();
                switch(token)
                {
                    case 'e':
                        hex = hex.Add(East);
                        break;
                    
                    case 'w':
                        hex = hex.Add(West);
                        break;
                    
                    case 'n':
                        token = tokens.Dequeue();
                        hex = hex.Add(token == 'w' ? NorthWest : NorthEast);
                        break;

                    case 's':
                        token = tokens.Dequeue();
                        hex = hex.Add(token == 'w' ? SouthWest : SouthEast);
                        break;
                    
                    default:
                        throw new FormatException("Invalid hex string");
                }                
            }

            return hex;
        }

        static List<HexCoordinate> Load(string path)
        {
            return File.ReadLines(path).Select(line => Parse(line)).ToList();
        }

        static HashSet<HexCoordinate> Flip(List<HexCoordinate> coordinates)
        {
            HashSet<HexCoordinate> black = new();
            foreach(HexCoordinate hex in coordinates)
            {
                if(black.Contains(hex))
                {
                    black.Remove(hex);
                }else{
                    black.Add(hex);
                }
            }
            
            return black;
        }

        static void Part1(List<HexCoordinate> coordinates)
        {
            HashSet<HexCoordinate> black = Flip(coordinates);
            Console.WriteLine("Part 1: {0}", black.Count);
        }

        static IEnumerable<HexCoordinate> Neighbors(this HexCoordinate hex)
        {
            yield return hex.Add(East);
            yield return hex.Add(West);
            yield return hex.Add(NorthEast);
            yield return hex.Add(NorthWest);
            yield return hex.Add(SouthEast);
            yield return hex.Add(SouthWest);
        }

        static HashSet<HexCoordinate> Step(HashSet<HexCoordinate> black)
        {
            HashSet<HexCoordinate> nearby = new(black.SelectMany(hex => hex.Neighbors().Append(hex)));
            HashSet<HexCoordinate> newBlack = new();
            foreach(HexCoordinate hex in nearby)
            {
                var neighbors = new HashSet<HexCoordinate>(hex.Neighbors());
                neighbors.IntersectWith(black);
                if((black.Contains(hex) && (neighbors.Count > 0 && neighbors.Count < 3)) || 
                   (!black.Contains(hex) && neighbors.Count == 2))
                {
                    newBlack.Add(hex);
                }
            }

            return newBlack;
        }

        static void Part2(List<HexCoordinate> coordinates)
        {
            HashSet<HexCoordinate> black = Flip(coordinates);
            for(int i=0; i<100; ++i)
            {
                black = Step(black);
            }

            Console.WriteLine("Part 2: {0}", black.Count);
        }

        static void Main(string[] args)
        {
            List<HexCoordinate> tiles = Load(args[0]);
            Part1(tiles);
            Part2(tiles);
        }
    }
}
