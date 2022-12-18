using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.XPath;
using Xunit;

namespace AoC_2022
{
    public static class Day18
    {
        public class Day18_Input : Dictionary<int, Dictionary<int, Dictionary<int, char>>>
        {
        }
      //  public record struct Point3d(int X, int Y, int Z);
        public static void Day18_Main()
        {
            var input = Day18_ReadInput();
            Console.WriteLine($"Day18 Part1: {Day18_Part1(input)}");
            Console.WriteLine($"Day18 Part2: {Day18_Part2(input)}");
        }

        public static Day18_Input Day18_ReadInput(string rawinput = "")
        {
            if (rawinput == "")
            {
                rawinput = new StreamReader("Day18\\Day18_input.txt").ReadToEnd();
            }

            var result = new Day18_Input();

            var minX = int.MaxValue;
            var maxX = int.MinValue;
            var minY = int.MaxValue;
            var maxY = int.MinValue;
            var minZ = int.MaxValue;
            var maxZ = int.MinValue;

            foreach (string line in rawinput.Split(new string[] { "\r\n", "\r", "\n" }, StringSplitOptions.None).Select(s => s.Trim()))
            {
                var nums = line.Split(',').Select(f => int.Parse(f)).ToArray();
                if (!result.ContainsKey(nums[0])) result.Add(nums[0], new Dictionary<int, Dictionary<int, char>>());
                if (!result[nums[0]].ContainsKey(nums[1])) result[nums[0]].Add(nums[1], new Dictionary<int, char>());
                if (!result[nums[0]][nums[1]].ContainsKey(nums[2])) result[nums[0]][nums[1]].Add(nums[2], 'L');
                result[nums[0]][nums[1]][nums[2]] = 'L';
                minX = Math.Min(minX, nums[0]);
                maxX = Math.Max(maxX, nums[0]);
                minY = Math.Min(minY, nums[1]);
                maxY = Math.Max(maxY, nums[1]);
                minZ = Math.Min(minZ, nums[2]);
                maxZ = Math.Max(maxZ, nums[2]);
            }

            for (var X = minX - 1; X <=maxX + 1; X++)
            {
                if (!result.ContainsKey(X)) result.Add(X, new Dictionary<int, Dictionary<int, char>>());
                for (var Y = minY - 1; Y <= maxY + 1; Y++)
                {
                    if (!result[X].ContainsKey(Y)) result[X].Add(Y, new Dictionary<int, char>());
                    for (var Z = minZ - 1; Z <= maxZ + 1; Z++)
                    {
                        if (!result[X][Y].ContainsKey(Z)) result[X][Y].Add(Z, '.');
                    }
                }
            }

            return result;
        }


        public static int Day18_Part1(Day18_Input input)
        {
            var surface = 0;
            var directions = new List<(int, int, int)>()
            {
                (1,0,0),
                (-1,0,0),
                (0,1,0),
                (0,-1,0),
                (0,0,1),
                (0,0,-1)
            };
            
            foreach(var X in input.Keys)
            {
                foreach(var Y in input[X].Keys)
                {
                    foreach(var Z in input[X][Y].Keys)
                    {
                        if (input[X][Y][Z]!= 'L') continue;
                        foreach (var dir in directions)
                        {
                            if (!input.ContainsKey(X + dir.Item1) || !input[X + dir.Item1].ContainsKey(Y + dir.Item2) || !input[X + dir.Item1][Y + dir.Item2].ContainsKey(Z + dir.Item3) || input[X + dir.Item1][Y + dir.Item2][Z + dir.Item3]!='L') surface += 1;
                        }
                    }
                }
            }
            return surface;
        }

        public static int Day18_Part2(Day18_Input input)
        {
            var surface = 0;
            var directions = new List<(int, int, int)>()
            {
                (1,0,0),
                (-1,0,0),
                (0,1,0),
                (0,-1,0),
                (0,0,1),
                (0,0,-1)
            };

            var StartX = input.Keys.Min();
            var StartY = input[StartX].Keys.Min();
            var StartZ = input[StartX][StartY].Keys.Min();

            Debug.Assert(input[StartX][StartY][StartZ] != 'L');
            var ToCheckIfWaterQueue = new Queue<(int, int, int)>();
            ToCheckIfWaterQueue.Enqueue((StartX, StartY, StartZ));
            while (ToCheckIfWaterQueue.Count > 0)
            {
                var toCheckCoord = ToCheckIfWaterQueue.Dequeue();
                if (input[toCheckCoord.Item1][toCheckCoord.Item2][toCheckCoord.Item3] != '.') continue;
                input[toCheckCoord.Item1][toCheckCoord.Item2][toCheckCoord.Item3] = 'W';
                foreach(var dir in directions)
                {
                    if (input.ContainsKey(toCheckCoord.Item1 + dir.Item1) &&
                        input[toCheckCoord.Item1 + dir.Item1].ContainsKey(toCheckCoord.Item2 + dir.Item2) &&
                        input[toCheckCoord.Item1 + dir.Item1][toCheckCoord.Item2 + dir.Item2].ContainsKey(toCheckCoord.Item3 + dir.Item3) &&
                        input[toCheckCoord.Item1 + dir.Item1][toCheckCoord.Item2 + dir.Item2][toCheckCoord.Item3 + dir.Item3] == '.')
                    {
                        ToCheckIfWaterQueue.Enqueue((toCheckCoord.Item1 + dir.Item1, toCheckCoord.Item2 + dir.Item2, toCheckCoord.Item3 + dir.Item3));
                    }
                }
            }


            foreach (var X in input.Keys)
            {
                foreach (var Y in input[X].Keys)
                {
                    foreach (var Z in input[X][Y].Keys)
                    {
                        if (input[X][Y][Z] != 'L') continue;
                        foreach (var dir in directions)
                        {
                            if (input.ContainsKey(X + dir.Item1) &&
                                input[X + dir.Item1].ContainsKey(Y + dir.Item2) && 
                                input[X + dir.Item1][Y + dir.Item2].ContainsKey(Z + dir.Item3) &&
                                input[X + dir.Item1][Y + dir.Item2][Z + dir.Item3] == 'W') surface += 1;
                        }
                    }
                }
            }
            return surface;
        }


    }
    public class Day18_Test
    {
        [Theory]
        [InlineData("1,1,1\r\n2,1,1", 10)]
        [InlineData("2,2,2\r\n1,2,2\r\n3,2,2\r\n2,1,2\r\n2,3,2\r\n2,2,1\r\n2,2,3\r\n2,2,4\r\n2,2,6\r\n1,2,5\r\n3,2,5\r\n2,1,5\r\n2,3,5", 64)]
        public static void Day18Part1Test(string rawinput, int expectedValue)
        {
            Assert.Equal(expectedValue, Day18.Day18_Part1(Day18.Day18_ReadInput(rawinput)));
        }

        [Theory]
        [InlineData("2,2,2\r\n1,2,2\r\n3,2,2\r\n2,1,2\r\n2,3,2\r\n2,2,1\r\n2,2,3\r\n2,2,4\r\n2,2,6\r\n1,2,5\r\n3,2,5\r\n2,1,5\r\n2,3,5", 58)]
        public static void Day18Part2Test(string rawinput, int expectedValue)
        {
            Assert.Equal(expectedValue, Day18.Day18_Part2(Day18.Day18_ReadInput(rawinput)));
        }
    }
}
