using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.XPath;
using Xunit;

namespace AoC_2022
{
    public static class Day05
    {
        public record Day05_Input(List<Stack<char>> StackOfCreates, List<Day05_RearrangeStep> RearrangeSteps);
        public record Day05_RearrangeStep(int Quantity, int From, int To);

        public static void Day05_Main()
        {
            var input = Day05_ReadInput();
            Console.WriteLine($"Day05 Part1: {Day05_Part1(input)}");
            input = Day05_ReadInput();
            Console.WriteLine($"Day05 Part2: {Day05_Part2(input)}");
        }

        public static Day05_Input Day05_ReadInput(string rawinput = "")
        {
            if (rawinput == "")
            {
                rawinput = new StreamReader("Day05\\Day05_input.txt").ReadToEnd();
            }

            var StackOfCreates = new List<Stack<char>>();
            var RearrangeSteps = new List<Day05_RearrangeStep>();

            var lines = rawinput.Split(new string[] { "\r\n", "\r", "\n" }, StringSplitOptions.None).ToArray();
            
            var i = 0;

            for (i = 1; i<= lines[0].Length; i += 4)
            {
                StackOfCreates.Add(new Stack<char>());
            }

            var textsplitline = 0;
            while (lines[textsplitline] !="")
            {
                textsplitline++;
            }

            for (i = textsplitline-2; i >= 0; i--)
            {
                var k = 0;
                for (var j = 1; j <= lines[i].Length; j += 4)
                {
                    if (lines[i][j] != ' ') StackOfCreates[k].Push(lines[i][j]);
                    k++;
                }
            }

            for (i = textsplitline + 1; i < lines.Length; i++)
            {
                var numbers = new List<int>();
                string[] rawnumbers = Regex.Split(lines[i], @"\D+");
                foreach (string value in rawnumbers)
                {
                    if (!string.IsNullOrEmpty(value))
                    {
                        numbers.Add( int.Parse(value));
                    }
                }
                if (numbers.Count() != 3) throw new UnreachableException();
                RearrangeSteps.Add(new Day05_RearrangeStep(numbers[0], numbers[1], numbers[2]));
            }


            return new Day05_Input(StackOfCreates, RearrangeSteps);
        }


        public static string Day05_Part1(Day05_Input input)
        {
            var state = input.StackOfCreates;
            foreach(var step in input.RearrangeSteps)
            {
                for(var i = 1; i<= step.Quantity; i++)
                {
                    state[step.To - 1].Push(state[step.From - 1].Pop());
                }
                
            }

            var result = "";
            foreach(var stack in state)
            {
                result = result + (stack.Peek());
            }

            return result;
        }

        public static string Day05_Part2(Day05_Input input)
        {
            var state = input.StackOfCreates;
            foreach (var step in input.RearrangeSteps)
            {
                var tempStack = new Stack<char>();

                for (var i = 1; i <= step.Quantity; i++)
                {
                    tempStack.Push(state[step.From - 1].Pop());
                }
                for (var i = 1; i <= step.Quantity; i++)
                {
                    state[step.To - 1].Push(tempStack.Pop());
                }

            }

            var result = "";
            foreach (var stack in state)
            {
                result = result + (stack.Peek());
            }

            return result;
        }


    }
    public class Day05_Test
    {
        [Theory]
        [InlineData("    [D]    \r\n[N] [C]    \r\n[Z] [M] [P]\r\n 1   2   3 \r\n\r\nmove 1 from 2 to 1\r\nmove 3 from 1 to 3\r\nmove 2 from 2 to 1\r\nmove 1 from 1 to 2", "CMZ")]
        public static void Day05Part1Test(string rawinput, string expectedValue)
        {
            Assert.Equal(expectedValue, Day05.Day05_Part1(Day05.Day05_ReadInput(rawinput)));
        }

        [Theory]
        [InlineData("    [D]    \r\n[N] [C]    \r\n[Z] [M] [P]\r\n 1   2   3 \r\n\r\nmove 1 from 2 to 1\r\nmove 3 from 1 to 3\r\nmove 2 from 2 to 1\r\nmove 1 from 1 to 2", "MCD")]
        public static void Day05Part2Test(string rawinput, string expectedValue)
        {
            Assert.Equal(expectedValue, Day05.Day05_Part2(Day05.Day05_ReadInput(rawinput)));
        }
    }
}
