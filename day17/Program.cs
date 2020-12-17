using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace day17
{
    record Cube(int X, int Y, int Z, int W);

    static class Program
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

        static HashSet<Cube> Load(string path)
        {
            HashSet<Cube> cubes = new();
            foreach((int y, string line) in File.ReadLines(path).Enumerate())
            {
                foreach((int x, char state) in line.Enumerate())
                {
                    if(state == '#')
                    {
                        cubes.Add(new Cube(x, y, 0, 0));
                    }
                }
            }

            return cubes;
        }

        static int ActiveNeighbors(this HashSet<Cube> cubes, Cube cube)
        {
            int count = 0;
            for(int x=-1; x < 2; ++x)
            {
                for(int y=-1; y < 2; ++y)
                {
                    for(int z=-1; z < 2; ++z)
                    {
                        for(int w=-1; w < 2; ++w)
                        {
                            if(x == 0 && y == 0 && z == 0 && w == 0)
                            {
                                continue;
                            }

                            Cube neighbor = new Cube(x + cube.X, y + cube.Y, z + cube.Z, w + cube.W);
                            if(cubes.Contains(neighbor))
                            {
                                count += 1;
                                if(count > 3)
                                {
                                    return count;
                                }
                            }
                        }
                    }
                }
            }

            return count;
        }

        static Cube Min(this Cube cube, Cube other)
        {
            return new Cube(cube.X < other.X ? cube.X : other.X,
                            cube.Y < other.Y ? cube.Y : other.Y,
                            cube.Z < other.Z ? cube.Z : other.Z,
                            cube.W < other.W ? cube.W : other.W);
        }

        static Cube Max(this Cube cube, Cube other)
        {
            return new Cube(cube.X > other.X ? cube.X : other.X,
                            cube.Y > other.Y ? cube.Y : other.Y,
                            cube.Z > other.Z ? cube.Z : other.Z,
                            cube.W > other.W ? cube.W : other.W);
        }

        static IEnumerable<Cube> Nearby(this HashSet<Cube> cubes, bool includeW)
        {
            Cube min = cubes.First();
            Cube max = cubes.First();
            foreach(Cube cube in cubes)
            {
                min = min.Min(cube);
                max = max.Max(cube);
            }

            min = new Cube(min.X - 1, min.Y - 1, min.Z - 1, includeW ? min.W - 1 : min.W);
            max = new Cube(max.X + 1, max.Y + 1, max.Z + 1, includeW ? max.W + 1 : max.W);

            for(int x=min.X; x <= max.X; ++x)
            {
                for(int y=min.Y; y <= max.Y; ++y)
                {
                    for(int z=min.Z; z <= max.Z; ++z)
                    {
                        for(int w=min.W; w <= max.W; ++w)
                        {
                            yield return new Cube(x, y, z, w);
                        }
                    }
                }
            }
        }

        static HashSet<Cube> Update(HashSet<Cube> cubes, bool includeW)
        {
            HashSet<Cube> active = new();
            foreach(Cube cube in cubes.Nearby(includeW))
            {
                int count = cubes.ActiveNeighbors(cube);
                if(count == 3 || (count == 2 && cubes.Contains(cube)))
                {
                    active.Add(cube);
                }
            }

            return active;
        }

        static int Simulate(HashSet<Cube> cubes, bool includeW)
        {
            for(int i=0; i<6; ++i)
            {
                cubes = Update(cubes, includeW);
            }

            return cubes.Count;
        }

        static void Main(string[] args)
        {
            HashSet<Cube> cubes = Load(args[0]);
            Console.WriteLine("Part 1: {0}", Simulate(cubes, false));
            Console.WriteLine("Part 2: {0}", Simulate(cubes, true));
        }
    }
}
