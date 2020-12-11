using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace day11
{
    record Position(int Row, int Column);

    static class Extensions
    {
        public static IEnumerable<(int Index, T Value)> Enumerate<T>(this IEnumerable<T> values)
        {
            int index = 0;
            foreach (var value in values)
            {
                yield return (index, value);
                index += 1;
            }
        }

        public static Position Move(this Position position, int drow, int dcolumn)
        {
            return new Position(position.Row + drow, position.Column + dcolumn);
        }

        public static bool IsInside(this Position position, int rows, int columns)
        {
            return !(position.Row < 0 || position.Column < 0 || position.Row >= rows || position.Column >= columns);
        }
    }

    class Seating
    {
        private Seating(int crowding)
        {
            _crowding = crowding;
            _floor = new();
            _empty = new();
            _occupied = new();
            _neighbors = new();
        }

        private HashSet<Position> neighborsOf(Position position, bool lineOfSight)
        {
            HashSet<Position> neighbors = new();
            for(int drow=-1; drow < 2; ++drow)
            {
                for(int dcolumn=-1; dcolumn < 2; ++dcolumn)
                {
                    if(drow == 0 && dcolumn == 0)
                    {
                        continue;
                    }

                    Position neighbor = position.Move(drow, dcolumn);
                    if(lineOfSight)
                    {
                        while(!_empty.Contains(neighbor) && neighbor.IsInside(Rows, Columns))
                        {
                            neighbor = neighbor.Move(drow, dcolumn);
                        }
                    }

                    if(_empty.Contains(neighbor))
                    {
                        neighbors.Add(neighbor);
                    }
                }
            }

            return neighbors;
        }

        private void complete(bool lineOfSight)
        {
            this.Rows = _empty.Max(pos => pos.Row) + 1;
            this.Columns = _empty.Max(pos => pos.Column) + 1;

            _neighbors = new();
            foreach(Position position in _empty)
            {
                _neighbors[position] = neighborsOf(position, lineOfSight);
            }
        }

        public void Add(Position position, bool isChair)
        {
            if(isChair){
                _empty.Add(position);
            }
            else{
                _floor.Add(position);
            }
        }

        public int Update()
        {
            HashSet<Position> empty = new();
            HashSet<Position> occupied = new();
            int numChanges = 0;
            foreach(Position position in _empty)
            {
                HashSet<Position> neighbors = new HashSet<Position>(_neighbors[position]);
                neighbors.IntersectWith(_occupied);
                if(neighbors.Count == 0)
                {
                    occupied.Add(position);
                    numChanges++;
                }
                else
                {
                    empty.Add(position);
                }
            }

            foreach(Position position in _occupied)
            {
                HashSet<Position> neighbors = new HashSet<Position>(_neighbors[position]);
                neighbors.IntersectWith(_occupied);
                if(neighbors.Count >= _crowding)
                {
                    empty.Add(position);
                    numChanges++;
                }
                else
                {
                    occupied.Add(position);
                }
            }

            _occupied = occupied;
            _empty = empty;
            return numChanges;
        }

        public int OccupiedCount
        {
            get
            {
                return _occupied.Count;
            }
        }

        public static Seating Load(string path, int crowding, bool lineOfSight)
        {
            Seating seating = new(crowding);
            foreach((int row, string line) in File.ReadLines(path).Enumerate())
            {
                foreach((int column, char value) in line.Enumerate())
                {
                    seating.Add(new Position(row, column), value == 'L');
                }
            }

            seating.complete(lineOfSight);

            return seating;
        }

        public override string ToString()
        {
            StringBuilder builder = new();
            for(int row=0; row < this.Rows; ++row)
            {
                for(int column=0; column < this.Columns; ++column)
                {
                    Position pos = new Position(row, column);
                    if(_floor.Contains(pos))
                    {
                        builder.Append('.');
                    }

                    if(_empty.Contains(pos))
                    {
                        builder.Append('L');
                    }

                    if(_occupied.Contains(pos))
                    {
                        builder.Append('#');
                    }
                }

                builder.AppendLine();
            }

            return builder.ToString();
        }

        public int Rows{get; private set;}
        public int Columns{get; private set;}
        private int _crowding;
        private Dictionary<Position, HashSet<Position>> _neighbors;
        private HashSet<Position> _floor;
        private HashSet<Position> _empty;
        private HashSet<Position> _occupied;
    }

    static class Program
    {
        static void Main(string[] args)
        {
            Seating seating = Seating.Load(args[0], 4, false);
            while(seating.Update() > 0);
            Console.WriteLine("Part 1: {0}", seating.OccupiedCount);

            seating = Seating.Load(args[0], 5, true);
            while(seating.Update() > 0);
            Console.WriteLine("Part 2: {0}", seating.OccupiedCount);
        }
    }
}
