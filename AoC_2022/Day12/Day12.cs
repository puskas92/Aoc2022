using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.XPath;
using Xunit;

namespace AoC_2022
{
    public static class Day12
    {
        public class Day12_Input : List<List<Day12_HeighMap>> //Define input type
        {
        }
        public class Day12_HeighMap
        {
            public bool Visited;

            public char Height;

            public int Distance ;

            public Day12_HeighMap(char height)
            {
                this.Height = height;
                Visited = false;
                Distance = int.MaxValue;
            }
        }
        public static void Day12_Main()
        {
            var input = Day12_ReadInput();
            Console.WriteLine($"Day12 Part1: {Day12_Part1(input)}");
            input = Day12_ReadInput();
            Console.WriteLine($"Day12 Part2: {Day12_Part2(input)}");
        }

        public static Day12_Input Day12_ReadInput(string rawinput = "")
        {
            if (rawinput == "")
            {
                rawinput = new StreamReader("Day12\\Day12_input.txt").ReadToEnd();
            }

            var result = new Day12_Input();

            foreach (string line in rawinput.Split(new string[] { "\r\n", "\r", "\n" }, StringSplitOptions.None).Select(s => s.Trim()))
            {
                result.Add(line.Select(f=> new Day12_HeighMap(f)).ToList());
            }

            return result;
        }


        public static int Day12_Part1(Day12_Input input)
        {
            return FindShortestPath(input);
        }

        private static int FindShortestPath(Day12_Input input)
        {
            var directions = new List<Point>
            {
                new Point(0,1),
                new Point(1,0),
                new Point(0,-1),
                new Point(-1,0),
            };

            var maxi = input.Count - 1;
            var maxj = input[maxi].Count - 1;
            var ToVisitQueue = new PriorityQueue<Point, int>();
            for (var i = 0; i < input.Count; i++)
            {
                for (var j = 0; j < input[i].Count; j++)
                {
                    if (input[i][j].Height == 'S')
                    {
                        ToVisitQueue.Enqueue(new Point(i, j), 0);
                        input[i][j].Distance = 0;
                        input[i][j].Visited = true;
                        break;
                    }
                }
                if (ToVisitQueue.Count > 0) break;
            }

            while (ToVisitQueue.Count > 0)
            {
                var Coords = ToVisitQueue.Dequeue();
                var Origin = input[Coords.X][Coords.Y];

                foreach (var dir in directions)
                {
                    var newCoord = new Point(Coords.X + dir.X, Coords.Y + dir.Y);

                    if (newCoord.X < 0 || newCoord.X > maxi || newCoord.Y < 0 || newCoord.Y > maxj) continue;
                    var toCheckTile = input[newCoord.X][newCoord.Y];
                    if (toCheckTile.Visited) continue;
                    var toCheckChar1 = toCheckTile.Height == 'E' ? 'z' : toCheckTile.Height;
                    var toCheckChar2 = Origin.Height == 'S' ? 'a' : Origin.Height;
                    if (toCheckChar1 - toCheckChar2 >= 2) continue;
                    toCheckTile.Distance = Origin.Distance + 1;
                    toCheckTile.Visited = true;
                    ToVisitQueue.Enqueue(newCoord, toCheckTile.Distance);

                    if (toCheckTile.Height == 'E')
                    {
                        return toCheckTile.Distance;
                    }

                }

            }
            return int.MaxValue;
        }

        public static int Day12_Part2(Day12_Input input)
        {
            var Paths = new List<Point>();

            for (var i = 0; i < input.Count; i++)
            {
                for (var j = 0; j < input[i].Count; j++)
                {
                    if (input[i][j].Height == 'S')
                    {
                        Paths.Add(new Point(i, j));
                        input[i][j].Height = 'a';
                    }
                    if (input[i][j].Height == 'a') Paths.Add(new Point(i, j));
                }
            }

            var minLength = int.MaxValue;
            foreach (var StartPoint in Paths)
            {
                for (var i = 0; i < input.Count; i++)
                {
                    for (var j = 0; j < input[i].Count; j++)
                    {
                        input[i][j].Distance = int.MaxValue;
                        input[i][j].Visited = false;
                    }
                }
                input[StartPoint.X][StartPoint.Y].Height = 'S';
                minLength = Math.Min(minLength, FindShortestPath(input));
                input[StartPoint.X][StartPoint.Y].Height = 'a';
            }
            return minLength;
        }


    }
    public class Day12_Test
    {
        [Theory]
        [InlineData("Sabqponm\r\nabcryxxl\r\naccszExk\r\nacctuvwj\r\nabdefghi", 31)]
        public static void Day12Part1Test(string rawinput, int expectedValue)
        {
            Assert.Equal(expectedValue, Day12.Day12_Part1(Day12.Day12_ReadInput(rawinput)));
        }

        [Theory]
        [InlineData("Sabqponm\r\nabcryxxl\r\naccszExk\r\nacctuvwj\r\nabdefghi", 29)]
        public static void Day12Part2Test(string rawinput, int expectedValue)
        {
            Assert.Equal(expectedValue, Day12.Day12_Part2(Day12.Day12_ReadInput(rawinput)));
        }
    }
}
