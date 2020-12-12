using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace day12
{
    record Instruction(char Action, int Value);
    record Position(int East, int North);
    record Ship(Position Position, char Facing);
    static class Program
    {
        static List<Instruction> load(string path)
        {
            return File.ReadLines(path)
                       .Select(line => new Instruction(line[0], int.Parse(line.Substring(1))))
                       .ToList();
        }

        static Position MoveCardinal(this Position position, char direction, int value)
        {
            return direction switch {
                'N' => position with {North=position.North + value},
                'E' => position with {East=position.East + value},
                'S' => position with {North=position.North - value},
                'W' => position with {East=position.East - value},
                _ => throw new InvalidOperationException("Invalid direction")
            };
        }

        static Dictionary<char, int> FacingToAngle = new Dictionary<char, int>(){
            {'E', 0}, {'N', 90}, {'W', 180}, {'S', 270}
        };

        static Dictionary<int, char> AngleToFacing = new Dictionary<int, char>(){
            {0, 'E'}, {90, 'N'}, {180, 'W'}, {270, 'S'}
        };

        static Ship Turn(this Ship state, int value)
        {
            int angle = FacingToAngle[state.Facing] + value;
            if(angle < 0)
            {
                angle += 360;
            }

            if(angle >= 360)
            {
                angle -= 360;
            }

            return state with {Facing=AngleToFacing[angle]};
        }

        static Ship Execute(this Instruction instruction, Ship ship)
        {
            return instruction.Action switch {
                'N' => ship with {Position = ship.Position.MoveCardinal(instruction.Action, instruction.Value)},
                'E' => ship with {Position = ship.Position.MoveCardinal(instruction.Action, instruction.Value)},
                'S' => ship with {Position = ship.Position.MoveCardinal(instruction.Action, instruction.Value)},
                'W' => ship with {Position = ship.Position.MoveCardinal(instruction.Action, instruction.Value)},
                'F' => ship with {Position = ship.Position.MoveCardinal(ship.Facing, instruction.Value)},
                'L' => ship.Turn(instruction.Value),
                'R' => ship.Turn(-instruction.Value),
                _ => throw new InvalidOperationException("Invalid action")
            };
        }

        static Position MoveToWaypoint(this Position ship, Position waypoint, int value)
        {
            return ship with {East = ship.East + value * waypoint.East, North = ship.North + value * waypoint.North};
        }

        static Position Rotate(this Position waypoint, int value)
        {
            return value switch {
                90 => new Position(-waypoint.North, waypoint.East),
                180 => new Position(-waypoint.East, -waypoint.North),
                270 => new Position(waypoint.North, -waypoint.East),
                _ => throw new InvalidOperationException("Invalid angle")
            };
        }

        static (Position, Position) Execute(this Instruction instruction, Position ship, Position waypoint)
        {
            return instruction.Action switch {
                'N' => (ship, waypoint.MoveCardinal(instruction.Action, instruction.Value)),
                'E' => (ship, waypoint.MoveCardinal(instruction.Action, instruction.Value)),
                'S' => (ship, waypoint.MoveCardinal(instruction.Action, instruction.Value)),
                'W' => (ship, waypoint.MoveCardinal(instruction.Action, instruction.Value)),
                'F' => (ship.MoveToWaypoint(waypoint, instruction.Value), waypoint),
                'L' => (ship, waypoint.Rotate(instruction.Value)),
                'R' => (ship, waypoint.Rotate(360 - instruction.Value)),
                _ => throw new InvalidOperationException("Invalid action")
            };
        }

        static int Manhattan(this Position state)
        {
            return Math.Abs(state.North) + Math.Abs(state.East);
        }

        static void Part1(List<Instruction> instructions)
        {
            Ship state = new Ship(new Position(0, 0), 'E');
            foreach(var instruction in instructions)
            {
                state = instruction.Execute(state);
            }

            Console.WriteLine("Part 1: {0}", state.Position.Manhattan());
        }

        static void Part2(List<Instruction> instructions)
        {
            Position ship = new Position(0, 0);
            Position waypoint = new Position(10, 1);
            foreach(var instruction in instructions)
            {
                (ship, waypoint) = instruction.Execute(ship, waypoint);
            }

            Console.WriteLine("Part 2: {0}", ship.Manhattan());
        }

        static void Main(string[] args)
        {
            List<Instruction> instructions = load(args[0]);
            Part1(instructions);
            Part2(instructions);
        }
    }
}
