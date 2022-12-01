using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml.XPath;
using Xunit;

namespace AoC_2022
{
    public static class Day01
    {
        public class Day01_Input : List<List<int>> //Define input type
        {
        }
        public static void Day01_Main()
        {
            var input = Day01_ReadInput();
            Console.WriteLine($"Day01 Part1: {Day01_Part1(input)}");
            Console.WriteLine($"Day01 Part2: {Day01_Part2(input)}");
        }

        public static Day01_Input Day01_ReadInput(string rawinput = "")
        {
            if (rawinput == "")
            {
                rawinput = new StreamReader("Day01\\Day01_input.txt").ReadToEnd();
            }

            var result = new Day01_Input();
            var subresult = new List<int>();

            foreach (string line in rawinput.Split(new string[] { "\r\n", "\r", "\n" }, StringSplitOptions.None).Select(s => s.Trim()))
            {
                if (line == "") {
                    result.Add(subresult);
                    subresult = new List<int>();
                }
                else subresult.Add(int.Parse(line));
            }
            result.Add(subresult);

            return result;
        }


        public static int Day01_Part1(Day01_Input input)
        {
            return input.OrderByDescending(f => f.Sum())
                .First()
                .Sum();
        }

        public static int Day01_Part2(Day01_Input input)
        {
            return input.OrderByDescending(f => f.Sum())
                .Take(3)
                .Sum(f => f.Sum());
        }
    }
    public class Day01_Test
    {
        [Theory]
        [InlineData("1000\r\n2000\r\n3000\r\n\r\n4000\r\n\r\n5000\r\n6000\r\n\r\n7000\r\n8000\r\n9000\r\n\r\n10000", 24000)]
        public static void Day01Part1Test(string rawinput, int expectedValue)
        {
            Assert.Equal(expectedValue, Day01.Day01_Part1(Day01.Day01_ReadInput(rawinput)));
        }

        [Theory]
        [InlineData("1000\r\n2000\r\n3000\r\n\r\n4000\r\n\r\n5000\r\n6000\r\n\r\n7000\r\n8000\r\n9000\r\n\r\n10000", 45000)]
        public static void Day01Part2Test(string rawinput, int expectedValue)
        {
            Assert.Equal(expectedValue, Day01.Day01_Part2(Day01.Day01_ReadInput(rawinput)));
        }
    }
}
