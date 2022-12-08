using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.XPath;
using Xunit;

namespace AoC_2022
{
    public static class Day08
    {
        public class Day08_Input : List<List<Day08_Tree>> //Define input type
        {
        }

        public record struct Day08_Tree(int Height, bool Visible, int ScenicScore);
            

        public static void Day08_Main()
        {
            var input = Day08_ReadInput();
            Console.WriteLine($"Day08 Part1: {Day08_Part1(input)}");
            Console.WriteLine($"Day08 Part2: {Day08_Part2(input)}");
        }

        public static Day08_Input Day08_ReadInput(string rawinput = "")
        {
            if (rawinput == "")
            {
                rawinput = new StreamReader("Day08\\Day08_input.txt").ReadToEnd();
            }

            var result = new Day08_Input();

            foreach (string line in rawinput.Split(new string[] { "\r\n", "\r", "\n" }, StringSplitOptions.None).Select(s => s.Trim()))
            {
                result.Add(line.Select(f => new Day08_Tree(int.Parse(f.ToString()), false, 0)).ToList());
            }

            return result;
        }


        public static int Day08_Part1(Day08_Input input)
        {
            //This part could be done with the same method as Part2, but I thought this would be faster, but since the iteration is done also by part2, it is not.

            for(var i = 0; i<input.Count; i++)
            {
                var height = -1;
                for(var j = 0; j< input[i].Count; j++)
                {
                    if (input[i][j].Height > height)
                    {
                        height = input[i][j].Height;
                        input[i][j] = new Day08_Tree(height, true, 0);
                    }
                    else if (input[i][j].Height == 9) break;
                }

                height = -1;
                for (var j = input[i].Count-1; j >= 0; j--)
                {
                    if (input[i][j].Height > height)
                    {
                        height = input[i][j].Height;
                        input[i][j] = new Day08_Tree(height, true,0 );
                    }
                    else if (input[i][j].Height == 9) break;
                }
            }

            for (var i = 0; i < input[0].Count; i++)
            {
                var height = -1;
                for (var j = 0; j < input.Count; j++)
                {
                    if (input[j][i].Height > height)
                    {
                        height = input[j][i].Height;
                        input[j][i] = new Day08_Tree(height, true,0 );
                    }
                    else if (input[j][i].Height == 9) break;
                }

                height = -1;
                for (var j = input.Count - 1; j >= 0; j--)
                {
                    if (input[j][i].Height > height)
                    {
                        height = input[j][i].Height;
                        input[j][i] = new Day08_Tree(height, true, 0);
                    }
                    else if (input[j][i].Height==9) break;
                }
            }



            return input.Sum(f => f.Count(g => g.Visible));
        }

        public static int Day08_Part2(Day08_Input input)
        {
            var directions = new List<(int, int)>
            {
                (1,0),
                (0,1),
                (-1,0),
                (0,-1)
            };

            for(var i = 1; i<input.Count-1; i++)
            {
                for(var j = 1; j < input[i].Count-1; j++)
                {
                    var score = 1;
                    
                    foreach(var dir in directions)
                    {
                        var height = input[i][j].Height;
                        var visible = 0;
                        for(var m = 1; m<input.Count; m++)
                        {
                            var k = i + dir.Item1*m;
                            var l = j + dir.Item2*m;
                            if (k < 0 || k > input.Count - 1 || l < 0 || l > input[i].Count - 1) break;
                            visible += 1;
                            if (input[k][l].Height >= height) break;
                        }
                        score *= visible;
                    }

                    input[i][j] = new Day08_Tree(input[i][j].Height, input[i][j].Visible, score);
                }
            }

            return input.Max(f=> f.Max(g => g.ScenicScore));
        }


    }
    public class Day08_Test
    {
        [Theory]
        [InlineData("30373\r\n25512\r\n65332\r\n33549\r\n35390", 21)]
        public static void Day08Part1Test(string rawinput, int expectedValue)
        {
            Assert.Equal(expectedValue, Day08.Day08_Part1(Day08.Day08_ReadInput(rawinput)));
        }

        [Theory]
        [InlineData("30373\r\n25512\r\n65332\r\n33549\r\n35390", 8)]
        public static void Day08Part2Test(string rawinput, int expectedValue)
        {
            Assert.Equal(expectedValue, Day08.Day08_Part2(Day08.Day08_ReadInput(rawinput)));
        }
    }
}
