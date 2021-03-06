﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;

namespace day3
{
    record Position(int Row, int Column);

    class Forest
    {
        private Forest(HashSet<Position> trees, int rows, int columns)
        {
            _trees = trees;
            _rows = rows;
            _columns = columns;
        }

        public bool TreeAt(Position location)
        {
            Position query = new Position(location.Row, location.Column % _columns);
            return _trees.Contains(query);
        }

        public bool IsInside(Position location)
        {
            return location.Row >= 0 && location.Row < _rows;
        }

        public static Forest Parse(string path)
        {
            HashSet<Position> trees = new HashSet<Position>();
            var lines = File.ReadAllLines(path);
            for(int i=0; i<lines.Length; ++i)
            {
                for(int j=0; j<lines[i].Length; ++j)
                {
                    if(lines[i][j] == '#')
                    {
                        trees.Add(new Position(i, j));
                    }
                }
            }

            return new Forest(trees, lines.Length, lines[0].Length);        
        }

        private HashSet<Position> _trees;
        private int _rows;
        private int _columns;
    }
    static class Program
    {
        static Position Sled(this Position location, (int Right, int Down) slope)
        {
            return new Position(location.Row + slope.Down, location.Column + slope.Right);
        }

        static int CheckSlope(Forest forest, (int Right, int Down) slope)
        {
            Position location = new Position(0, 0);
            int numTrees = 0;
            while(forest.IsInside(location))
            {
                if(forest.TreeAt(location))
                {
                    numTrees += 1;
                }

                location = location.Sled(slope);
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
            var product = numTrees.Aggregate(new BigInteger(1), (product, x) => product * x);
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
