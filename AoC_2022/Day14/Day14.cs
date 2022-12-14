﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.XPath;
using Xunit;

namespace AoC_2022
{
    public static class Day14
    {
        public class Day14_Input : Dictionary<int, Dictionary<int, char>> { }

        public static void Day14_Main()
        {
            var input = Day14_ReadInput();
            Console.WriteLine($"Day14 Part1: {Day14_Part1(input)}");
            Console.WriteLine($"Day14 Part2: {Day14_Part2(input)}");
        }

        public static Day14_Input Day14_ReadInput(string rawinput = "")
        {
            if (rawinput == "")
            {
                rawinput = new StreamReader("Day14\\Day14_input.txt").ReadToEnd();
            }

            var result = new Day14_Input();

            foreach (string line in rawinput.Split(new string[] { "\r\n", "\r", "\n" }, StringSplitOptions.None).Select(s => s.Trim()))
            {
                var points = line.Split("->").Select(s => {
                    var coords = s.Trim().Split(",").Select(g => int.Parse(g)).ToArray();
                    return new Point(coords[0], coords[1]);
                    }).ToArray();
               
                for(var i = 0; i<points.Count()-1; i++)
                {
                    //for(var j = points[i].X; j != points[i+1].X; j+= Math.Sign(points[i + 1].X - points[i].X))
                    foreach(int j in Enumerable.Range(Math.Min(points[i].X, points[i+1].X),  Math.Abs(points[i+1].X - points[i].X)+1))
                    {
                        //for (var k = points[i].Y; k != points[i + 1].Y; k += Math.Sign(points[i + 1].Y - points[i].Y))
                        foreach(int k in Enumerable.Range(Math.Min(points[i].Y, points[i+1].Y), Math.Abs(points[i+1].Y -points[i].Y)+1))
                        {
                            if (!result.ContainsKey(j)) result.Add(j, new Dictionary<int, char>());
                            if (!result[j].ContainsKey(k)) result[j].Add(k, '#');
                            result[j][k] = '#';
                        }
                    }
                }
            }
            Day14_VisualazeMap(result);
            return result;
        }

        public static void Day14_VisualazeMap(Day14_Input input)
        {
            var mini = input.Keys.Min();
            var maxi = input.Keys.Max();
            var minj = input.Min(f => f.Value.Keys.Min());
            var maxj = input.Max(f => f.Value.Keys.Max());

            for(var j = minj; j<= maxj; j++)
            {
                var s = "";
         
                for (var i = mini; i<= maxi; i++)
                {
                    if (!input.ContainsKey(i)) s += '.';
                    else if (!input[i].ContainsKey(j)) s += '.';
                    else s += input[i][j];
                }
                Debug.WriteLine(s);
            }
        }


        public static int Day14_Part1(Day14_Input input)
        {
            var startPoint = new Point(500, 0);
            var goesToInfinite = false;
          
            while (!goesToInfinite)
            {
                Point sandParticle = new Point(startPoint.X, startPoint.Y);
            }
        }

        public static int Day14_Part2(Day14_Input input)
        {
            return 0;
        }


    }
    public class Day14_Test
    {
        [Theory]
        [InlineData("498,4 -> 498,6 -> 496,6\r\n503,4 -> 502,4 -> 502,9 -> 494,9", 24)]
        public static void Day14Part1Test(string rawinput, int expectedValue)
        {
            Assert.Equal(expectedValue, Day14.Day14_Part1(Day14.Day14_ReadInput(rawinput)));
        }

        [Theory]
        [InlineData("ABC", 0)]
        public static void Day14Part2Test(string rawinput, int expectedValue)
        {
            Assert.Equal(expectedValue, Day14.Day14_Part2(Day14.Day14_ReadInput(rawinput)));
        }
    }
}
