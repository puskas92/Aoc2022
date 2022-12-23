using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Xml.XPath;
using Xunit;

namespace AoC_2022
{
    public static class Day23
    {
        public class Day23_Input
        {
            //public Dictionary<int, Dictionary<int, bool>> Map;
            public Dictionary<int, HashSet<int>> Elfes;

            public int timeStep;

            public Day23_Input()
            {
                Elfes = new Dictionary<int, HashSet<int>>();
                timeStep = 0;
            }
            //public Day23_Input()
            //{
            //    Map = new Dictionary<int, Dictionary<int, bool>>();
            //    timeStep = 0;
            //}

            //public void Reduce()
            //{
            //    while (true)
            //    {
            //        if (!Map[Map.Keys.Min()].Any(f => f.Value)) Map.Remove(Map.Keys.Min());
            //        else break;
            //    }

            //    while (true)
            //    {
            //        if (!Map[Map.Keys.Max()].Any(f => f.Value)) Map.Remove(Map.Keys.Max());
            //        else break;
            //    }

            //    while (true)
            //    {
            //        if (!Map.Any(f => f.Value[f.Value.Keys.Min()]))
            //        {
            //            foreach (var row in Map)
            //            {
            //                row.Value.Remove(row.Value.Keys.Min());
            //            }
            //        }
            //        else break;
            //    }


            //    while (true)
            //    {
            //        if (!Map.Any(f => f.Value[f.Value.Keys.Max()]))
            //        {
            //            foreach (var row in Map)
            //            {
            //                row.Value.Remove(row.Value.Keys.Max());
            //            }
            //        }
            //        else break;
            //    }
            //}

            //public int Score()
            //{
            //    Reduce();
            //    return Map.Sum(f => f.Value.Count(g => !g.Value));
            //}
            public int Score()
            {
                var MinRow = Elfes.Keys.Min();
                var MaxRow = Elfes.Keys.Max();
                var MinColumn = Elfes.Min(f => f.Value.Min());
                var MaxColumn = Elfes.Max(f => f.Value.Max());

                return ((MaxRow-MinRow+1)*(MaxColumn-MinColumn+1))-Elfes.Sum(f=> f.Value.Count());
            }

            //public bool DoATimeStep()
            //{
            //    var IsChanged = false;
            //    timeStep += 1;

            //    var NewState = new Dictionary<int, Dictionary<int, List<(int, int)>>>();
            //    var mincolumn = Map[Map.Keys.Min()].Keys.Min();
            //    var maxcolumn = Map[Map.Keys.Min()].Keys.Max();

            //    //create new state object
            //    for (var i = Map.Keys.Min()-1; i <= Map.Keys.Max() +1; i++)
            //    {
            //        NewState.Add(i, new Dictionary<int, List<(int, int)>>());
            //        for(var j = mincolumn - 1; j<= maxcolumn +1; i++)
            //        {
            //            //if (Map.ContainsKey(i) && Map[i].ContainsKey(j))
            //            NewState[i].Add(j, new List<(int, int)>());
            //        }
            //    }

            //    //Do the proposal


            //    //Update the map based on the proposals

            //    return IsChanged;
            //}


            public bool DoATimeStep()
            {
             
                var directions = new Dictionary<int, Point>()
                {
                    [0] = new Point(-1, 0),
                    [1] = new Point(1, 0),
                    [2] = new Point(0, -1),
                    [3] = new Point(0, 1)
                };
                var CountOfProposalToStay = 0;
                var Proposal = new Dictionary<int, Dictionary<int, List<(int, int)>>>();
                foreach(var row in Elfes)
                {
                    foreach(var column in row.Value)
                    {
                        if(!TestPosition(row.Key - 1, column - 1) && !TestPosition(row.Key - 1, column) && !TestPosition(row.Key - 1, column + 1) && !TestPosition(row.Key , column - 1) &&  !TestPosition(row.Key, column + 1) && !TestPosition(row.Key + 1, column - 1) && !TestPosition(row.Key + 1, column) && !TestPosition(row.Key + 1, column + 1) )
                        {
                            SetToProposal(Proposal, row.Key, column, row.Key, column);
                            CountOfProposalToStay += 1;
                            continue;
                        }
                        var ToContinue = false;
                        for (int i= 0; i< 4; i++)
                        {
                           
                            var dir = directions[(timeStep + i) % 4];
                            if (!TestPosition(row.Key + dir.X - dir.Y, column + dir.Y - dir.X) && !TestPosition(row.Key + dir.X, column + dir.Y) && !TestPosition(row.Key + dir.X + dir.Y, column + dir.Y + dir.X))
                            {
                                SetToProposal(Proposal, row.Key + dir.X, column + dir.Y, row.Key, column);
                                ToContinue = true;
                                break;
                            }
                        }
                        if (ToContinue) continue;
                        //if (!TestPosition(row.Key-1, column-1) && !TestPosition(row.Key - 1, column) && !TestPosition(row.Key - 1, column  + 1))
                        //{
                        //    SetToProposal(Proposal, row.Key - 1, column, row.Key, column);
                        //}
                        //else if (!TestPosition(row.Key + 1, column - 1) && !TestPosition(row.Key + 1, column) && !TestPosition(row.Key + 1, column + 1))
                        //{
                        //    SetToProposal(Proposal, row.Key + 1, column, row.Key, column);
                        //}
                        //else if (!TestPosition(row.Key-1, column - 1) && !TestPosition(row.Key , column-1) && !TestPosition(row.Key + 1, column - 1))
                        //{
                        //    SetToProposal(Proposal, row.Key, column -1, row.Key, column);
                        //}
                        //else if (!TestPosition(row.Key - 1, column + 1) && !TestPosition(row.Key, column + 1) && !TestPosition(row.Key + 1, column + 1))
                        //{
                        //    SetToProposal(Proposal, row.Key, column+1, row.Key, column);
                        //}
                        //else
                        //{
                        SetToProposal(Proposal, row.Key, column, row.Key, column);
                            CountOfProposalToStay += 1;
                        //}
                    }
                }

                var NewState = new Dictionary<int, HashSet<int>>();
                foreach (var row in Proposal)
                {
                    foreach (var column in row.Value)
                    {
                        if (column.Value.Count() == 1)
                        {
                            SetToNewState(NewState, row.Key, column.Key);
                            CountOfProposalToStay -= 1;
                        }
                        else
                        {
                            foreach(var pair in column.Value)
                            {
                                SetToNewState(NewState, pair.Item1, pair.Item2);
                            }
                        }
                    }
                }

                Elfes = NewState;
                timeStep += 1;
                return (CountOfProposalToStay!=0);
            }

            private static void SetToProposal(Dictionary<int, Dictionary<int, List<(int, int)>>> Proposal, int DestRow, int DestColumn, int ValueRow, int ValueColumn)
            {
                if (!Proposal.ContainsKey(DestRow)) Proposal.Add(DestRow, new Dictionary<int, List<(int, int)>>());
                if (!Proposal[DestRow].ContainsKey(DestColumn)) Proposal[DestRow].Add(DestColumn, new List<(int, int)>());
                Proposal[DestRow][DestColumn].Add((ValueRow, ValueColumn));
            }

            private static void SetToNewState(Dictionary<int, HashSet<int>> NewState, int Row, int Column)
            {
                if (!NewState.ContainsKey(Row)) NewState.Add(Row, new HashSet<int>());
                if (!NewState[Row].Contains(Column)) NewState[Row].Add(Column);
                else throw new Exception();
            }

            public bool TestPosition(int row, int column)
            {
                return (Elfes.ContainsKey(row) && Elfes[row].Contains(column));
            }
        }
        public static void Day23_Main()
        {
            var input = Day23_ReadInput();
            Console.WriteLine($"Day23 Part1: {Day23_Part1(input)}");
            Console.WriteLine($"Day23 Part2: {Day23_Part2(input)}");
        }

        public static Day23_Input Day23_ReadInput(string rawinput = "")
        {
            if (rawinput == "")
            {
                rawinput = new StreamReader("Day23\\Day23_input.txt").ReadToEnd();
            }

            var result = new Day23_Input();

            var row = 0;
            foreach (string line in rawinput.Split(new string[] { "\r\n", "\r", "\n" }, StringSplitOptions.None).Select(s => s.Trim()))
            {
                for(var i = 0; i< line.Length; i++)
                {
                    if (line[i] == '#')
                    {
                        if (!result.Elfes.ContainsKey(row)) result.Elfes.Add(row, new HashSet<int>());
                        result.Elfes[row].Add(i);
                    }
                }
                row += 1;
            }

            return result;
        }


        public static int Day23_Part1(Day23_Input input)
        {
            for(var i= 1; i<= 10; i++)
            {
                if (!input.DoATimeStep()) break;
            }
            return input.Score();
        }

        public static int Day23_Part2(Day23_Input input)
        {
            while (input.DoATimeStep()) { };
            return input.timeStep;
        }


    }
    public class Day23_Test
    {
        [Theory]
        [InlineData("....#..\r\n..###.#\r\n#...#.#\r\n.#...##\r\n#.###..\r\n##.#.##\r\n.#..#..", 110)]
        [InlineData(".....\r\n..##.\r\n..#..\r\n.....\r\n..##.\r\n.....", 25)]
        public static void Day23Part1Test(string rawinput, int expectedValue)
        {
            Assert.Equal(expectedValue, Day23.Day23_Part1(Day23.Day23_ReadInput(rawinput)));
        }

        [Theory]
        [InlineData("....#..\r\n..###.#\r\n#...#.#\r\n.#...##\r\n#.###..\r\n##.#.##\r\n.#..#..", 20)]
        [InlineData(".....\r\n..##.\r\n..#..\r\n.....\r\n..##.\r\n.....", 4)]
        public static void Day23Part2Test(string rawinput, int expectedValue)
        {
            Assert.Equal(expectedValue, Day23.Day23_Part2(Day23.Day23_ReadInput(rawinput)));
        }
    }
}
