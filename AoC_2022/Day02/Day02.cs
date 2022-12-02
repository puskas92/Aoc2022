using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.XPath;
using Xunit;

namespace AoC_2022
{
    public static class Day02
    {
        public class Day02_Input : List<(char, char)> //Define input type
        {
        }
        public static void Day02_Main()
        {
            var input = Day02_ReadInput();
            Console.WriteLine($"Day02 Part1: {Day02_Part1(input)}");
            Console.WriteLine($"Day02 Part2: {Day02_Part2(input)}");
        }

        public static Day02_Input Day02_ReadInput(string rawinput = "")
        {
            if (rawinput == "")
            {
                rawinput = new StreamReader("Day02\\Day02_input.txt").ReadToEnd();
            }

            var result = new Day02_Input();

            foreach (string line in rawinput.Split(new string[] { "\r\n", "\r", "\n" }, StringSplitOptions.None).Select(s => s.Trim()))
            {
                var s = line.Split(" ").Select(f => f.First()).ToArray();
                result.Add((s[0], s[1]));
            }

            return result;
        }


        public static int Day02_Part1(Day02_Input input)
        {
            var score = 0;
            foreach(var play in input)
            {
                switch (((((play.Item2 - 'X')-(play.Item1-'A')) % 3)+3)%3)
                {
                    case 0:
                        score += 3;
                        break;
                    case 1:
                        score += 6;
                        break;
                    case 2:
                        score += 0;
                        break;
                    default:
                        throw new UnreachableException();
                }

                switch (play.Item2)
                {
                    case 'X':
                        score += 1;
                        break;
                    case 'Y':
                        score += 2;
                        break;
                    case 'Z':
                        score += 3;
                        break;
                    default:
                        throw new UnreachableException();
                }
            }
            return score;
        }

        public static int Day02_Part2(Day02_Input input)
        {
            var score = 0;
            foreach (var play in input)
            {
                switch (((((play.Item2 - 'X') + (play.Item1 - 'A')) % 3) + 3) % 3)
                {
                    case 0:
                        score += 3;
                        break;
                    case 1:
                        score += 1;
                        break;
                    case 2:
                        score += 2;
                        break;
                    default:
                        throw new UnreachableException();
                }

                switch (play.Item2)
                {
                    case 'X':
                        score += 0;
                        break;
                    case 'Y':
                        score += 3;
                        break;
                    case 'Z':
                        score += 6;
                        break;
                    default:
                        throw new UnreachableException();
                }
            }
            return score;
        }
    }

    public class Day02_Test
    {
        [Theory]
        [InlineData("A Y", 8)]
        [InlineData("B X", 1)]
        [InlineData("C Z", 6)]
        [InlineData("A Y\r\nB X\r\nC Z", 15)]
        public static void Day02Part1Test(string rawinput, int expectedValue)
        {
            Assert.Equal(expectedValue, Day02.Day02_Part1(Day02.Day02_ReadInput(rawinput)));
        }

        [Theory]
        [InlineData("A Y", 4)]
        [InlineData("B X", 1)]
        [InlineData("C Z", 7)]
        [InlineData("A X", 3)]
        [InlineData("A Z", 8)]
        [InlineData("A Y\r\nB X\r\nC Z", 12)]
        public static void Day02Part2Test(string rawinput, int expectedValue)
        {
            Assert.Equal(expectedValue, Day02.Day02_Part2(Day02.Day02_ReadInput(rawinput)));
        }
    }
}
