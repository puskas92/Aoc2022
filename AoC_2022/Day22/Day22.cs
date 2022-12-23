using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Xml.XPath;
using Xunit;

namespace AoC_2022
{
    public static class Day22
    {
        public class Day22_Input //Define input type
        {
            public Dictionary<int, Dictionary<int, (char, int)>> Map;

            public List<string> CommandList;

            public Day22_Input()
            {
                Map = new Dictionary<int, Dictionary<int, (char, int)>>();
                CommandList = new List<string>();
            }
        }
        public static void Day22_Main()
        {
            var input = Day22_ReadInput();
            Console.WriteLine($"Day22 Part1: {Day22_Part1(input)}");
            Console.WriteLine($"Day22 Part2: {Day22_Part2(input)}");
        }

        public static Day22_Input Day22_ReadInput(string rawinput = "")
        {
            if (rawinput == "")
            {
                rawinput = new StreamReader("Day22\\Day22_input.txt").ReadToEnd();
            }

            var result = new Day22_Input();

            var row = 1;
            foreach (string line in rawinput.Split(new string[] { "\r\n", "\r", "\n" }, StringSplitOptions.None))
            {
                if (line == "") break;
                result.Map.Add(row, new Dictionary<int, (char, int)>());
                for(var i = 1; i<= line.Length; i++)
                {
                    int side;
                    //this section is input dependent:
                    if (row <= 50)
                    {
                        if (i <= 100) side = 1;
                        else side = 2;
                    }
                    else if (row > 50 && row <= 100) side = 3;
                    else if (row > 100 && row <= 150)
                    {
                        if (i <= 50) side = 4;
                        else side = 5;
                    }
                    else side = 6;

                    if (line[i-1] != ' ') result.Map[row].Add(i, (line[i-1], side));
                }
                row++;
            }

            string s = "";
            foreach(var character in rawinput.Split(new string[] { "\r\n", "\r", "\n" }, StringSplitOptions.None).Last())
            {
                int num;
                if(int.TryParse(character.ToString(), out num))
                {
                    s+= character;
                }
                else
                {
                    if (s != "") result.CommandList.Add(s);
                    s = "";
                    result.CommandList.Add(character.ToString());
                }
            }
            if (s != "") result.CommandList.Add(s);

            return result;
        }


        public static int Day22_Part1(Day22_Input input)
        {
            var directions = new Dictionary<int, Point>() {
                [0] = new Point(0,1), 
                [1] = new Point(1,0), 
                [2] = new Point(0,-1), 
                [3] = new Point(-1,0)
            };
            var StartX = input.Map.Keys.Min();
            var Position = new Point(StartX, input.Map[StartX].Keys.Min());
            var Direction = 0;

            foreach(var command in input.CommandList)
            {
                int num;
                if (int.TryParse(command, out num))
                {
                    for(var i = 1; i<= num; i++)
                    {
                        var TestPos = new Point(Position.X + directions[Direction].X, Position.Y + directions[Direction].Y);
                        if (!input.Map.ContainsKey(TestPos.X) || !input.Map[TestPos.X].ContainsKey(TestPos.Y))//  || input.Map[TestPos.X][TestPos.Y] == ' ')
                        {
                            switch (Direction)
                            {
                                case 0:
                                    TestPos= new Point(TestPos.X,  input.Map[TestPos.X].Keys.Min());
                                    break;
                                case 1:
                                    TestPos = new Point(input.Map.Where(f=> f.Value.ContainsKey(TestPos.Y)).Select(f=> f.Key).Min(), TestPos.Y);
                                    break;
                                case 2:
                                    TestPos = new Point(TestPos.X, input.Map[TestPos.X].Keys.Max());
                                    break;
                                case 3:
                                    TestPos = new Point(input.Map.Where(f => f.Value.ContainsKey(TestPos.Y)).Select(f => f.Key).Max(), TestPos.Y);
                                    break;
                                default:
                                    throw new Exception();
                            }
                        }

                        if (input.Map[TestPos.X][TestPos.Y].Item1 == '#') break;

                        Position = TestPos;
                    }
                }
                else
                {
                    if (command == "R") Direction += 1;
                    else if (command == "L") Direction -= 1;
                    else throw new Exception();
                    Direction = (Direction+4) % 4;
                } 
            }

            return 1000 * Position.X + 4* Position.Y + Direction;
        }

        public static int Day22_Part2(Day22_Input input)
        {
            var directions = new Dictionary<int, Point>()
            {
                [0] = new Point(0, 1),
                [1] = new Point(1, 0),
                [2] = new Point(0, -1),
                [3] = new Point(-1, 0)
            };
            var StartX = input.Map.Keys.Min();
            var Position = new Point(StartX, input.Map[StartX].Keys.Min());
            var Direction = 0;

            foreach (var command in input.CommandList)
            {
                int num;
                if (int.TryParse(command, out num))
                {
                    for (var i = 1; i <= num; i++)
                    {
                        var newDir = Direction;
                        var TestPos = new Point(Position.X + directions[Direction].X, Position.Y + directions[Direction].Y);
                        if (!input.Map.ContainsKey(TestPos.X) || !input.Map[TestPos.X].ContainsKey(TestPos.Y))//  || input.Map[TestPos.X][TestPos.Y] == ' ')
                        {
                            var SideX = Position.X % 50;
                            if (SideX == 0) SideX = 50;
                            var SideY = Position.Y % 50;
                            if (SideY == 0) SideY = 50;

                            switch (Direction)
                            {
                                case 0: // right 2, 3, 5, 6
                                    switch (input.Map[Position.X][Position.Y].Item2)
                                    {
                                        case 2:  // 5 right
                                            TestPos = new Point(151-SideX, 100);
                                            newDir = 2;
                                            break;
                                        case 3:  // 2 bottom
                                            TestPos = new Point(50, 100+SideX);
                                            newDir = 3;
                                            break;
                                        case 5: // 2 right
                                            TestPos = new Point(51 - SideX, 150);
                                            newDir = 2;
                                            break;
                                        case 6:  //5 bottom
                                            TestPos = new Point(150, 50 + SideX);
                                            newDir = 3;
                                            break;
                                        default:
                                            throw new Exception();
                                    }
                                    break;
                                case 1: // bottom 2, 5, 6
                                    switch (input.Map[Position.X][Position.Y].Item2)
                                    {
                                        case 2: // 3 right
                                            TestPos = new Point(50 + SideY, 100);
                                            newDir = 2;
                                            break;
                                        case 5: //6 right
                                            TestPos = new Point(150 + SideY, 50);
                                            newDir = 2;
                                            break;
                                        case 6: //2 top
                                            TestPos = new Point( 1, 100 + SideY);
                                            newDir = 1;
                                            break;
                                        default:
                                            throw new Exception();
                                    }
                                    break;
                                case 2: // left 1, 3, 4, 6
                                    switch (input.Map[Position.X][Position.Y].Item2)
                                    {
                                        case 1:  //4 left
                                            TestPos = new Point(151 - SideX, 1);
                                            newDir = 0;
                                            break;
                                        case 3:  //4 top
                                            TestPos = new Point(101, SideX);
                                            newDir = 1;
                                            break;
                                        case 4:  //1 left
                                            TestPos = new Point(51 - SideX, 51);
                                            newDir = 0;
                                            break;
                                        case 6:  //1 top
                                            TestPos = new Point(1, 50 + SideX);
                                            newDir = 1;
                                            break;
                                        default:
                                            throw new Exception();
                                    }
                                    break;
                                case 3: // top 1, 2, 4
                                    switch (input.Map[Position.X][Position.Y].Item2)
                                    {
                                        case 1: // 6 left
                                            TestPos = new Point(150 + SideY, 1);
                                            newDir = 0;
                                            break;
                                        case 2:  // 6 bottom
                                            TestPos = new Point(200, SideY);
                                            newDir = 3;
                                            break;
                                        case 4: // 3 left
                                            TestPos = new Point(50 + SideY, 51);
                                            newDir = 0;
                                            break;
                                        default:
                                            throw new Exception();
                                    }
                                    break;
                                default:
                                    throw new Exception();
                            }
                        }

                        //newDir = (newDir + 4) % 4;
                        if (input.Map[TestPos.X][TestPos.Y].Item1 == '#') break;

                        Direction = newDir;
                        Position = TestPos;
                    }
                }
                else
                {
                    if (command == "R") Direction += 1;
                    else if (command == "L") Direction -= 1;
                    else throw new Exception();
                    Direction = (Direction + 4) % 4;
                }
            }

            return 1000 * Position.X + 4 * Position.Y + Direction;  //117138 too high
        }


    }
    public class Day22_Test
    {
        [Theory]
        [InlineData("        ...#\r\n        .#..\r\n        #...\r\n        ....\r\n...#.......#\r\n........#...\r\n..#....#....\r\n..........#.\r\n        ...#....\r\n        .....#..\r\n        .#......\r\n        ......#.\r\n\r\n10R5L5R10L4R5L5", 6032)]
        public static void Day22Part1Test(string rawinput, int expectedValue)
        {
            Assert.Equal(expectedValue, Day22.Day22_Part1(Day22.Day22_ReadInput(rawinput)));
        }

        //[Theory]
        //[InlineData("        ...#\r\n        .#..\r\n        #...\r\n        ....\r\n...#.......#\r\n........#...\r\n..#....#....\r\n..........#.\r\n        ...#....\r\n        .....#..\r\n        .#......\r\n        ......#.\r\n\r\n10R5L5R10L4R5L5", 5031)]
        //public static void Day22Part2Test(string rawinput, int expectedValue)
        //{
        //    Assert.Equal(expectedValue, Day22.Day22_Part2(Day22.Day22_ReadInput(rawinput)));
        //}
    }
}
