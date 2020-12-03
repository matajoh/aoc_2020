using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace day3
{
    class Forest
    {
        private Forest(HashSet<(int Row, int Column)> trees, int rows, int columns)
        {
            this._trees = trees;
            this._rows = rows;
            this._columns = columns;
        }

        public bool TreeAt((int Row, int Column) location)
        {
            (int Row, int Column) query = (location.Row, location.Column % this._columns);
            return this._trees.Contains(query);
        }

        public bool IsInside((int Row, int Column) location)
        {
            return location.Row >= 0 && location.Row < this._rows;
        }

        public static Forest Parse(string path)
        {
            HashSet<(int Row, int Column)> trees = new HashSet<(int Row, int Column)>();
            var lines = File.ReadAllLines(path);
            for(int i=0; i<lines.Length; ++i)
            {
                for(int j=0; j<lines[i].Length; ++j)
                {
                    if(lines[i][j] == '#')
                    {
                        trees.Add((i, j));
                    }
                }
            }

            return new Forest(trees, lines.Length, lines[0].Length);        
        }

        private HashSet<(int Row, int Column)> _trees;
        private int _rows;
        private int _columns;
    }
    class Program
    {
        static (int Row, int Column) Sled((int Row, int Column) location, (int Right, int Down) slope)
        {
            return (location.Row + slope.Down, location.Column + slope.Right);
        }

        static long CheckSlope(Forest forest, (int Right, int Down) slope)
        {
            (int Row, int Column) location = (0, 0);
            int numTrees = 0;
            while(forest.IsInside(location))
            {
                if(forest.TreeAt(location))
                {
                    numTrees += 1;
                }

                location = Sled(location, slope);
            }

            return numTrees;
        }

        static void Part1(Forest forest)
        {
            Console.WriteLine("Part 1: {0}", CheckSlope(forest, (3, 1)));
        }

        static void Part2(Forest forest)
        {
            var slopes = new []{(1, 1), (3, 1), (5, 1), (7, 1), (1, 2)};
            var numTrees = slopes.Select(slope => CheckSlope(forest, slope));
            var product = numTrees.Aggregate(1L, (product, x) => product * x);
            Console.WriteLine("Part 2: {0}", product);
        }

        static void Main(string[] args)
        {
            Forest forest = Forest.Parse(args[0]);
            Part1(forest);
            Part2(forest);
        }
    }
}
