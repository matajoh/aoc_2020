using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace day20
{
    class Tile
    {
        public Tile(long id, bool[,] pixels)
        {
            Id = id;
            Pixels = pixels;
            Size = pixels.GetLength(0);
            Left = border(0, Size - 1, 0, 0);
            Top = border(0, 0, 0, Size - 1);
            Right = border(0, Size - 1, Size - 1, Size - 1);
            Bottom = border(Size - 1, Size - 1, 0, Size - 1);
        }

        public void CropTo(bool[,] pixels, int gridRow, int gridColumn)
        {
            int cropSize = Size - 2;
            for (int row = 0; row < cropSize; ++row)
            {
                for (int column = 0; column < cropSize; ++column)
                {
                    pixels[gridRow * cropSize + row, gridColumn * cropSize + column] = Pixels[row + 1, column + 1];
                }
            }
        }

        public int Opposite(int border)
        {
            if (border == Left)
            {
                return Right;
            }
            else if (border == Top)
            {
                return Bottom;
            }
            else if (border == Right)
            {
                return Left;
            }
            else if (border == Bottom)
            {
                return Top;
            }
            else
            {
                throw new ArgumentException("Invalid border");
            }
        }

        private int border(int rowStart, int rowEnd, int columnStart, int columnEnd)
        {
            int code = 0;
            for (int row = rowStart; row <= rowEnd; ++row)
            {
                for (int column = columnStart; column <= columnEnd; ++column)
                {
                    code += Pixels[row, column] ? 1 : 0;
                    code = code << 1;
                }
            }

            return code;
        }

        public long Id { get; private set; }
        public bool[,] Pixels { get; private set; }
        public int Size { get; private set; }
        public int Left { get; private set; }
        public int Top { get; private set; }
        public int Right { get; private set; }
        public int Bottom { get; private set; }

        public static Tile Parse(Queue<string> lines)
        {
            string line = lines.Dequeue();
            long id = long.Parse(line.Substring(5, line.IndexOf(':') - 5));
            int size = lines.Peek().Length;
            bool[,] pixels = new bool[size, size];
            for (int row = 0; row < size; ++row)
            {
                line = lines.Dequeue();
                for (int column = 0; column < size; ++column)
                {
                    pixels[row, column] = line[column] == '#';
                }
            }

            return new Tile(id, pixels);
        }
    }

    static class Program
    {
        static T[,] Rotate90<T>(this T[,] grid)
        {
            int rows = grid.GetLength(1);
            int columns = grid.GetLength(0);
            T[,] rotated = new T[rows, columns];
            for (int row = 0; row < rows; ++row)
            {
                for (int column = 0; column < columns; ++column)
                {
                    rotated[row, column] = grid[rows - column - 1, row];
                }
            }

            return rotated;
        }

        static T[,] FlipHorizontal<T>(this T[,] grid)
        {
            int rows = grid.GetLength(0);
            int columns = grid.GetLength(1);
            T[,] flipped = new T[rows, columns];
            for (int row = 0; row < rows; ++row)
            {
                for (int column = 0; column < columns; ++column)
                {
                    flipped[row, column] = grid[row, columns - column - 1];
                }
            }

            return flipped;
        }

        static IEnumerable<T[,]> Permutations<T>(this T[,] grid)
        {
            var rot90 = grid.Rotate90();
            var rot180 = rot90.Rotate90();
            var rot270 = rot180.Rotate90();
            yield return grid;
            yield return grid.FlipHorizontal();
            yield return rot90;
            yield return rot90.FlipHorizontal();
            yield return rot180;
            yield return rot180.FlipHorizontal();
            yield return rot270;
            yield return rot270.FlipHorizontal();
        }

        static IEnumerable<Tile> Permutations(this Tile tile)
        {
            foreach (var pixels in tile.Pixels.Permutations())
            {
                yield return new Tile(tile.Id, pixels);
            }
        }

        static Tile[,] Load(string path)
        {
            List<Tile> tiles = new();
            Queue<string> lines = new(File.ReadLines(path));
            while (lines.Count > 0)
            {
                tiles.Add(Tile.Parse(lines));
                if (lines.Count > 0)
                {
                    lines.Dequeue();
                }
            }

            return Assemble(tiles.SelectMany(tile => tile.Permutations()).ToList());
        }

        static void Add(this Dictionary<int, List<Tile>> edges, int edge, Tile tile)
        {
            if (!edges.ContainsKey(edge))
            {
                edges[edge] = new();
            }

            edges[edge].Add(tile);
        }

        static List<Tile> FindPath(this Dictionary<int, List<Tile>> edges, Tile start, int direction, int length, Func<Tile, bool> omit)
        {
            Queue<(int, List<Tile>)> frontier = new();
            frontier.Enqueue((direction, new() { start }));
            while (frontier.Count > 0)
            {
                (int edge, List<Tile> path) = frontier.Dequeue();
                if (path.Count == length)
                {
                    return path;
                }

                if (!edges.ContainsKey(edge))
                {
                    continue;
                }

                foreach (Tile tile in edges[edge])
                {
                    if (path.Count(other => tile.Id == other.Id) > 0 || omit(tile))
                    {
                        continue;
                    }

                    int next = tile.Opposite(edge);
                    frontier.Enqueue((next, new List<Tile>(path.Append(tile))));
                }
            }

            throw new Exception("Unable to find path");
        }

        static T Only<T>(this IEnumerable<T> list)
        {
            if (list.Count() != 1)
            {
                throw new InvalidOperationException("Not a singleton enumerable");
            }

            return list.First();
        }

        static Tile[,] Assemble(List<Tile> tiles)
        {
            int size = (int)Math.Sqrt(tiles.Count / 8);
            Dictionary<int, List<Tile>> left = new();
            Dictionary<int, List<Tile>> top = new();
            Dictionary<int, List<Tile>> right = new();
            Dictionary<int, List<Tile>> bottom = new();
            foreach (Tile tile in tiles)
            {
                left.Add(tile.Left, tile);
                top.Add(tile.Top, tile);
                right.Add(tile.Right, tile);
                bottom.Add(tile.Bottom, tile);
            }

            Tile[,] grid = new Tile[size, size];

            // pick and position the top left
            grid[0, 0] = tiles.Where(tile => bottom[tile.Top].Count == 1)
                              .Where(tile => right[tile.Left].Count == 1)
                              .First();

            // place top edge
            var path = left.FindPath(grid[0, 0], grid[0, 0].Right, size, tile => bottom[tile.Top].Count > 1);
            for (int column = 1; column < path.Count; ++column)
            {
                grid[0, column] = path[column];
            }

            // place left edge
            path = top.FindPath(grid[0, 0], grid[0, 0].Bottom, size, tile => right[tile.Left].Count > 1);
            for (int row = 1; row < path.Count; ++row)
            {
                grid[row, 0] = path[row];
            }

            for (int row = 1; row < size; ++row)
            {
                for (int column = 1; column < size; ++column)
                {
                    grid[row, column] = left[grid[row, column - 1].Right].Intersect(top[grid[row - 1, column].Bottom]).Only();
                }
            }

            return grid;
        }

        static bool[,] Compose(Tile[,] tiles)
        {
            int size = tiles.GetLength(0);
            int cropSize = tiles[0, 0].Size - 2;
            bool[,] result = new bool[size * cropSize, size * cropSize];
            for (int row = 0; row < size; ++row)
            {
                for (int column = 0; column < size; ++column)
                {
                    tiles[row, column].CropTo(result, row, column);
                }
            }

            return result;
        }

        static bool[,] SeaMonster = new bool[,]{
            {false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, true, false},
            {true, false, false, false, false, true, true, false, false, false, false, true, true, false, false, false, false, true, true, true},
            {false, true, false, false, true, false, false, true, false, false, true, false, false, true, false, false, true, false, false, false}
        };
        const int SeaMonsterRows = 3;
        const int SeaMonsterColumns = 20;
        const int SeaMonsterPixels = 15;

        static int CountMask(this bool[,] pixels, int row, int column)
        {
            int count = 0;
            for (int maskRow = 0; maskRow < SeaMonsterRows; ++maskRow)
            {
                for (int maskColumn = 0; maskColumn < SeaMonsterColumns; ++maskColumn)
                {
                    if (SeaMonster[maskRow, maskColumn])
                    {
                        count += pixels[row + maskRow, column + maskColumn] ? 1 : 0;
                    }
                }
            }

            return count;
        }

        static int FindMonsters(bool[,] pixels)
        {
            int count = 0;
            for (int row = 0; row < pixels.GetLength(0) - SeaMonsterRows; ++row)
            {
                for (int column = 0; column < pixels.GetLength(1) - SeaMonsterColumns; ++column)
                {
                    if (CountMask(pixels, row, column) == SeaMonsterPixels)
                    {
                        count += 1;
                    }
                }
            }

            return count;
        }

        static int Count(this bool[,] pixels)
        {
            int count = 0;
            for (int row = 0; row < pixels.GetLength(0); ++row)
            {
                for (int column = 0; column < pixels.GetLength(1); ++column)
                {
                    if (pixels[row, column])
                    {
                        count += 1;
                    }
                }
            }

            return count;
        }

        static void Part1(Tile[,] tiles)
        {
            int size = tiles.GetLength(0);
            long product = tiles[0, 0].Id * tiles[0, size - 1].Id * tiles[size - 1, 0].Id * tiles[size - 1, size - 1].Id;
            Console.WriteLine("Part 1: {0}", product);
        }

        static void Part2(Tile[,] tiles)
        {
            bool[,] image = Compose(tiles);
            int count = 0;
            foreach (var pixels in image.Permutations())
            {
                int numMonsters = FindMonsters(pixels);
                if (numMonsters > 0)
                {
                    count = image.Count() - numMonsters * SeaMonsterPixels;
                }
            }
            Console.WriteLine("Part 2: {0}", count);
        }

        static void Main(string[] args)
        {
            Tile[,] tiles = Load(args[0]);
            Part1(tiles);
            Part2(tiles);
        }
    }
}
