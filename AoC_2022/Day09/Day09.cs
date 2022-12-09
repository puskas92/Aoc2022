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
    public static class Day09
    {
        public class Day09_Input : List<(char, int)> { } //Define input type
        public static void Day09_Main()
        {
            var input = Day09_ReadInput();
            Console.WriteLine($"Day09 Part1: {Day09_Part1(input)}");
            Console.WriteLine($"Day09 Part2: {Day09_Part2(input)}");
        }

        public static Day09_Input Day09_ReadInput(string rawinput = "")
        {
            if (rawinput == "")
            {
                rawinput = new StreamReader("Day09\\Day09_input.txt").ReadToEnd();
            }

            var result = new Day09_Input();

            foreach (string line in rawinput.Split(new string[] { "\r\n", "\r", "\n" }, StringSplitOptions.None).Select(s => s.Trim()))
            {
                var args = line.Split(" ");
                result.Add((args[0].First(), int.Parse(args[1])));
            }

            return result;
        }


        public static int Day09_Part1(Day09_Input input)
        {
            return Day09_SimulatNLengthRope(input, 2);
        }

        public static int Day09_Part2(Day09_Input input)
        {
            return Day09_SimulatNLengthRope(input, 10);
        }

        private static int Day09_SimulatNLengthRope(Day09_Input input, int length)
        {
            var TailBeen = new List<Point>();
            var KnotPositions = new List<Point>();
            for (var i = 1; i <= length; i++)
            {
                KnotPositions.Add(new Point(0, 0));
            }

            TailBeen.Add(KnotPositions.Last());

            var directions = new Dictionary<char, Point>()
            {
                { 'U',new Point(1,0) },
                { 'D',new Point(-1,0) },
                { 'L',new Point(0,-1) },
                { 'R',new Point(0,1) }
            };


            foreach (var motion in input)
            {
                var dir = directions[motion.Item1];
                for (var i = 1; i <= motion.Item2; i++)
                {
                    KnotPositions[0] = new Point(KnotPositions[0].X + dir.X, KnotPositions[0].Y + dir.Y);

                    for (var j = 1; j < KnotPositions.Count; j++)
                    {
                        KnotPositions[j] = Day09_CalculateRope1Knot(KnotPositions[j - 1], KnotPositions[j]);
                    }

                    if (!TailBeen.Contains(KnotPositions.Last())) TailBeen.Add(KnotPositions.Last());
                }
            }

            return TailBeen.Count;
        }

        private static Point Day09_CalculateRope1Knot(Point HeadPos, Point TailPos)
        {
            if (Math.Abs(HeadPos.X - TailPos.X) > 1 || Math.Abs(HeadPos.Y - TailPos.Y) > 1) TailPos = new Point(TailPos.X + Math.Sign(HeadPos.X - TailPos.X), TailPos.Y + Math.Sign(HeadPos.Y - TailPos.Y));
            return TailPos;
        }
    }
    public class Day09_Test
    {
        [Theory]
        [InlineData("R 4\r\nU 4\r\nL 3\r\nD 1\r\nR 4\r\nD 1\r\nL 5\r\nR 2", 13)]
        public static void Day09Part1Test(string rawinput, int expectedValue)
        {
            Assert.Equal(expectedValue, Day09.Day09_Part1(Day09.Day09_ReadInput(rawinput)));
        }

        [Theory]
        [InlineData("R 4\r\nU 4\r\nL 3\r\nD 1\r\nR 4\r\nD 1\r\nL 5\r\nR 2", 1)]
        [InlineData("R 5\r\nU 8\r\nL 8\r\nD 3\r\nR 17\r\nD 10\r\nL 25\r\nU 20", 36)]
        public static void Day09Part2Test(string rawinput, int expectedValue)
        {
            Assert.Equal(expectedValue, Day09.Day09_Part2(Day09.Day09_ReadInput(rawinput)));
        }
    }
}
