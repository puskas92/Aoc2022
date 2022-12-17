using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.XPath;
using Xunit;
using static AoC_2022.Day17;

namespace AoC_2022
{
    public static class Day17
    {
        public class Day17_Input : List<bool> { } //true: > ; false: <        
        public static void Day17_Main()
        {

            var Day17Tiles = new List<Day17_Tile>()
        {
            new Day17_Tile("####"),
            new Day17_Tile(".#.\r\n###\r\n.#."),
            new Day17_Tile("..#\r\n..#\r\n###"),
            new Day17_Tile("#\r\n#\r\n#\r\n#"),
            new Day17_Tile("##\r\n##")
        };

            var input = Day17_ReadInput();
            Console.WriteLine($"Day17 Part1: {Day17_Part1(Day17Tiles, input)}");
            Console.WriteLine($"Day17 Part2: {Day17_Part2(Day17Tiles, input)}");
        }

        public static Day17_Input Day17_ReadInput(string rawinput = "")
        {
            if (rawinput == "")
            {
                rawinput = new StreamReader("Day17\\Day17_input.txt").ReadToEnd();
            }

            var result = new Day17_Input();

            result.AddRange(rawinput.Trim().Select(f => (f == '>')));

            return result;

    }


        public static Int64 Day17_Part1(List<Day17_Tile>  tiles, Day17_Input input)
        {
            return SimulateFalling(tiles, input, 2022);
        }

        private static Int64 SimulateFalling(List<Day17_Tile> tiles, Day17_Input input, Int64 rockCount)
        {
            var map = new Dictionary<int, Dictionary<int, bool>>();
            map.Add(-1, new Dictionary<int, bool>());
            for (var i = 1; i < 7; i++) { map[-1].Add(i, true); };


            int currentMaxHeight = -1;
            var time = 0;
            var inputLength = input.Count;
            var RepeatList = new Dictionary<int, (Int64, Int64)>();
            var Repeatint = -1;
            Int64 currentMaxHeightShift = 0;
            for (Int64 tilecount = 0; tilecount < rockCount; tilecount++)
            {
                //it is ugly but it works
                //idea: it should repeat itself. statuses are the current piece, and the jet command position (time%inputlength) and the top row
                //if the current piece is 0 (so the "----" it is likely sit on the top of the top row (it was checked visually, not checked by the program itself)
                //so the program saves all tilecount and currentmaxHeight for each jet command position when piece is 0
                //at the first time when we get the same status the difference between the tilecount and currentMaxHeight can be calculated
                //in this place it was checked manully if the next piece will sit on the top piece (both for test input and for real input)
                //from this a fast forward can be done until almost reaching the rock limit. After that the program runs normally forward.
                if ((tilecount % 5) == 0)
                {
                    if (Repeatint != -1 && Repeatint != -2)
                    {
                        if ((time % inputLength) == Repeatint)
                        {
                            //Debug.WriteLine((tilecount % 5) + " " + (time % inputLength) + " " + tilecount + " " + currentMaxHeight);
                            //PrintMap(map);
                            //Debug.WriteLine((tilecount - RepeatList[Repeatint].Item1) + " " + (currentMaxHeight - RepeatList[Repeatint].Item2));

                            var tiledelta = (tilecount - RepeatList[Repeatint].Item1);
                            var heightdelta = (currentMaxHeight - RepeatList[Repeatint].Item2);
                            Int64 Multiplier = (rockCount - tilecount) / tiledelta;
                            tilecount += tiledelta * Multiplier;
                            currentMaxHeightShift = heightdelta* Multiplier;
                            
                            Repeatint = -2;
                        }
                    }
                    else if (Repeatint == -1)
                    {
                        if (RepeatList.ContainsKey((time % inputLength)))
                        {
                            //Debug.WriteLine((tilecount % 5) + " " + (time % inputLength) + " " + tilecount + " " + currentMaxHeight);
                            //PrintMap(map);
                            Repeatint = (time % inputLength);
                        }
                        else RepeatList.Add((time % inputLength), (tilecount, currentMaxHeight));
                    }
                  
                }

                var currentTile = tiles[(int)(tilecount % 5)];
                var position = new Point((currentTile.Height - 1) + currentMaxHeight + 4, 2);
                var hitbottom = false;
                while (!hitbottom)
                {
                    //jetmovement
                    var jetcommand = (input[time % inputLength]) ? 1 : -1;
                    var jetposition = new Point(position.X, position.Y + jetcommand);


                    if (!TestHit(map, currentTile, jetposition)) position = jetposition;

                    //downstep
                    var downposition = new Point(position.X - 1, position.Y);
                    hitbottom = TestHit(map, currentTile, downposition);

                    if (hitbottom)
                    {
                        //update map
                        for (var i = 0; i < currentTile.Height; i++)
                        {
                            var testPosX = position.X - i;
                            if (!map.ContainsKey(testPosX)) map.Add(testPosX, new Dictionary<int, bool>());
                            for (var j = 0; j < currentTile.Width; j++)
                            {
                                var testPosY = position.Y + j;
                                if (!currentTile.Shape[currentTile.Height - 1 - i, j]) continue;
                                if (!map[testPosX].ContainsKey(testPosY)) map[testPosX].Add(testPosY, true);
                            }
                        }

                        //currentmaxHeight
                        currentMaxHeight = map.Keys.Max();

                        // PrintMap(map);
                    }
                    else
                    {
                        position = downposition;
                    }


                    time++;
                }

            }
            return currentMaxHeight + currentMaxHeightShift + 1;
        }

        private static void PrintMap(Dictionary<int, Dictionary<int, bool>> map)
        {
            Debug.WriteLine("");
            var maxi = map.Keys.Max();
            for (int i = maxi; i >= Math.Max(0, maxi-20); i--)
            {
                var s = "";
                if (!map.ContainsKey(i))
                {
                    Debug.WriteLine(".......");
                    continue;
                }
                else
                {
                    for(var j= 0; j<7; j++)
                    {
                        s += (map[i].ContainsKey(j) && map[i][j]) ? '#' : '.';
                    }
                    Debug.WriteLine(s);
                }
            }
        }
        private static bool TestHit(Dictionary<int, Dictionary<int, bool>> map, Day17_Tile currentTile, Point position)
        {

            var hit = false;
            for (var i = 0; i < currentTile.Height; i++)
            {
                for (var j = 0; j < currentTile.Width; j++)
                {
                    if (!currentTile.Shape[currentTile.Height - 1 - i, j]) continue;
                    var testPos = new Point(position.X - i, position.Y + j);
                    if (testPos.Y < 0 || testPos.Y > 6)
                    {
                        hit = true;
                        break;
                    }
                    if (map.ContainsKey(testPos.X) && map[testPos.X].ContainsKey(testPos.Y) && map[testPos.X][testPos.Y])
                    {
                        hit = true;
                        break;
                    }
                }
                if (hit) break;
            }
            return hit;
        }

        public static Int64 Day17_Part2(List<Day17_Tile>  tiles, Day17_Input input)
        {
            return SimulateFalling(tiles, input, 1000000000000); ;
        }

        public struct Day17_Tile { 
            public int Width;
            public int Height;
            public bool[,] Shape;

            public Day17_Tile(string ShapeString)
            {
                var lines = ShapeString.Split("\r\n");
                var Shape = new bool[lines.Count(), lines[0].Length];
                for (var i = 0; i < lines.Count(); i++)
                {
                    for(var j = 0; j < lines[i].Length; j++)
                    {
                        Shape[(lines.Count()-1)-i, j] = (lines[i][j] == '#');
                    }
                }
                this.Shape = Shape;
                Width = lines[0].Length;
                Height = lines.Count();
            }
        }
    }



    public class Day17_Test
    {
        [Theory]
        [InlineData(">>><<><>><<<>><>>><<<>>><<<><<<>><>><<>>", 3068)]
        public static void Day17Part1Test(string rawinput, int expectedValue)
        {
            var Day17Tiles = new List<Day17_Tile>()
        {
            new Day17_Tile("####"),
            new Day17_Tile(".#.\r\n###\r\n.#."),
            new Day17_Tile("..#\r\n..#\r\n###"),
            new Day17_Tile("#\r\n#\r\n#\r\n#"),
            new Day17_Tile("##\r\n##")
        };

            Assert.Equal(expectedValue, Day17.Day17_Part1(Day17Tiles, Day17.Day17_ReadInput(rawinput)));
        }

        [Theory]
        [InlineData(">>><<><>><<<>><>>><<<>>><<<><<<>><>><<>>", 1514285714288)]
        public static void Day17Part2Test(string rawinput, Int64 expectedValue)
        {
            var Day17Tiles = new List<Day17_Tile>()
        {
            new Day17_Tile("####"),
            new Day17_Tile(".#.\r\n###\r\n.#."),
            new Day17_Tile("..#\r\n..#\r\n###"),
            new Day17_Tile("#\r\n#\r\n#\r\n#"),
            new Day17_Tile("##\r\n##")
        };
            Assert.Equal(expectedValue, Day17.Day17_Part2(Day17Tiles, Day17.Day17_ReadInput(rawinput)));
        }
    }
}
