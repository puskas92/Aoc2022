using System.Diagnostics;
using Xunit;

namespace AoC_2022
{
    public static class Day21
    {
        public class Day21_Input : Dictionary<string, string> //Define input type
        {
        }
        public static void Day21_Main()
        {
            var input = Day21_ReadInput();
            Console.WriteLine($"Day21 Part1: {Day21_Part1(input)}");
            Console.WriteLine($"Day21 Part2: {Day21_Part2(input)}");
        }

        public static Day21_Input Day21_ReadInput(string rawinput = "")
        {
            if (rawinput == "")
            {
                rawinput = new StreamReader("Day21\\Day21_input.txt").ReadToEnd();
            }

            var result = new Day21_Input();

            foreach (string line in rawinput.Split(new string[] { "\r\n", "\r", "\n" }, StringSplitOptions.None).Select(s => s.Trim()))
            {
                var args = line.Split(':');
                result.Add(args[0], args[1].Trim());
            }

            return result;
        }


        public static Int64 Day21_Part1(Day21_Input input)
        {
            
            return Day21_GetValue(input, "root");
        }

        public static Int64 Day21_Part2(Day21_Input input)
        {
            Int64 humn;
            var args = input["root"].Split(" ");
            input["humn"] = "x";
            var Equation = Day21_GetPolinom(input, args[0]) - Day21_GetPolinom(input, args[2]);
            Equation.Reduce();
            if (Equation.Count() == 2 && Equation.ContainsKey(0) && Equation.ContainsKey(1))//so linear solution
            {
                humn = (Int64)(-Equation[0] / Equation[1]);
            }
            else throw new NotImplementedException(); //could be done also for second or third order, and treat also when all numbers are ok

            //test solutiuon
            input["humn"] = humn.ToString();
            Debug.Assert(Day21_GetValue(input, args[0]) == Day21_GetValue(input, args[2]));
            
            return humn;
        }

        public static Day21_Polinom Day21_GetPolinom(Day21_Input input, string address)
        {
            Double num;
            if (input[address] == "x") return new Day21_Polinom { [1] = 1 };
            else if (Double.TryParse(input[address], out num)) return new Day21_Polinom { [0] = num };
            else
            {
                var args = input[address].Split(' ');
                var num1 = Day21_GetPolinom(input, args[0]);
                var num2 = Day21_GetPolinom(input, args[2]);
                switch (args[1])
                {
                    case "+":
                        return num1 + num2;
                    case "-":
                        return num1 - num2;
                    case "*":
                        return num1 * num2;
                    case "/":
                        return num1 / num2;
                    default:
                        throw new Exception();
                }
            }
        }
        public static Int64 Day21_GetValue(Day21_Input input, string address)
        {
            Int64 num;
            if (Int64.TryParse(input[address], out num)) return num;
            else
            {
                var args = input[address].Split(' ');
                var num1 = Day21_GetValue(input, args[0]);
                var num2 = Day21_GetValue(input, args[2]);
                switch (args[1])
                {
                    case "+":
                        return num1 + num2;
                    case "-":
                        return num1 - num2;
                    case "*":
                        return num1 * num2;
                    case "/":
                        return num1 / num2;
                    default:
                        throw new Exception();
                }
            }
        }

        public class Day21_Polinom : Dictionary<int, Double>
        {
            public void SetElement(int Key, Double Value)
            {
                if (!this.ContainsKey(Key)) this.Add(Key, Value);
                else this[Key] = Value;
            }
            public Double GetElement(int Key)
            {
                if (!this.ContainsKey(Key)) return 0;
                else return this[Key];
            }
            public void AddElement(int Key, Double Value)
            {
                if (!this.ContainsKey(Key)) this.Add(Key, Value);
                else this[Key] += Value;
            }

            public void Reduce()
            {
               var ListToRemove = new List<int>();
                foreach(var KeyValuePair in this)
                {
                    if (KeyValuePair.Value == 0) ListToRemove.Add(KeyValuePair.Key);
                }
                foreach(var toRemoveKey in ListToRemove)
                {
                    this.Remove(toRemoveKey);
                }
            } 

            public static Day21_Polinom operator + (Day21_Polinom a, Day21_Polinom b)
            {
                var Result = new Day21_Polinom();
                for(var i = Math.Min(a.Keys.Min(), b.Keys.Min()); i <= Math.Max(a.Keys.Max(), b.Keys.Max()); i++) {
                    Result.SetElement(i, a.GetElement(i) + b.GetElement(i));
                }
                Result.Reduce();
                return Result;
            }

            public static Day21_Polinom operator -(Day21_Polinom a, Day21_Polinom b)
            {
                var Result = new Day21_Polinom();
                for (var i = Math.Min(a.Keys.Min(), b.Keys.Min()); i <= Math.Max(a.Keys.Max(), b.Keys.Max()); i++)
                {
                    Result.SetElement(i, a.GetElement(i) - b.GetElement(i));
                }
                Result.Reduce();
                return Result;
            }

            public static Day21_Polinom operator *(Day21_Polinom a, Day21_Polinom b)
            {
                var Result = new Day21_Polinom();
                for (var i = b.Keys.Min(); i <= b.Keys.Max(); i++)
                {
                    for (var j = a.Keys.Min(); j <= a.Keys.Max(); j++)
                    {
                        Result.AddElement(i+j, a.GetElement(j) * b.GetElement(i));
                    }
                }
                Result.Reduce();
                return Result;
            }

            public static Day21_Polinom operator /(Day21_Polinom a, Day21_Polinom b)
            {
                if (b.Keys.Count() > 1 || !b.Keys.Contains(0)) throw new NotImplementedException();
                var Result = new Day21_Polinom();
                for (var i = a.Keys.Min(); i <= a.Keys.Max(); i++)
                {
                  //  if(a.GetElement(i) %  b.GetElement(0) != 0) throw new NotImplementedException();
                    Result.SetElement(i, a.GetElement(i) / b.GetElement(0));
                }
                Result.Reduce();
                return Result;
            }
        }
    }
    public class Day21_Test
    {
        [Theory]
        [InlineData("root: pppw + sjmn\r\ndbpl: 5\r\ncczh: sllz + lgvd\r\nzczc: 2\r\nptdq: humn - dvpt\r\ndvpt: 3\r\nlfqf: 4\r\nhumn: 5\r\nljgn: 2\r\nsjmn: drzm * dbpl\r\nsllz: 4\r\npppw: cczh / lfqf\r\nlgvd: ljgn * ptdq\r\ndrzm: hmdt - zczc\r\nhmdt: 32", 152)]
        public static void Day21Part1Test(string rawinput, int expectedValue)
        {
            Assert.Equal(expectedValue, Day21.Day21_Part1(Day21.Day21_ReadInput(rawinput)));
        }

        [Theory]
        [InlineData("root: pppw + sjmn\r\ndbpl: 5\r\ncczh: sllz + lgvd\r\nzczc: 2\r\nptdq: humn - dvpt\r\ndvpt: 3\r\nlfqf: 4\r\nhumn: 5\r\nljgn: 2\r\nsjmn: drzm * dbpl\r\nsllz: 4\r\npppw: cczh / lfqf\r\nlgvd: ljgn * ptdq\r\ndrzm: hmdt - zczc\r\nhmdt: 32", 301)]
        public static void Day21Part2Test(string rawinput, Int64 expectedValue)
        {
            Assert.Equal(expectedValue, Day21.Day21_Part2(Day21.Day21_ReadInput(rawinput)));
        }

        [Theory]
        [MemberData(nameof(IntersectData))]
        public static void Day21PolynomOperatonTests(Day21.Day21_Polinom input, Day21.Day21_Polinom expectedValue)
        {
            Assert.Equal(expectedValue, input);
        }

        public static IEnumerable<object[]> IntersectData()
        {
            yield return new object[] { new Day21.Day21_Polinom { [1] = 2, [0] = 1 } + new Day21.Day21_Polinom { [1] = 3, [0] = 4 }, new Day21.Day21_Polinom { [1] = 5, [0] = 5 } }; // (2x + 1) + (3x + 4) = (5x + 5) 
            yield return new object[] { new Day21.Day21_Polinom { [1] = 2, [0] = 1 } - new Day21.Day21_Polinom { [1] = 3, [0] = 4 }, new Day21.Day21_Polinom { [1] = -1, [0] = -3 } }; // (2x + 1) - (3x + 4) = (-x - 3) 
            yield return new object[] { new Day21.Day21_Polinom { [1] = 2, [0] = 1 } * new Day21.Day21_Polinom { [1] = 3, [0] = 4 }, new Day21.Day21_Polinom { [2] = 6,  [1] = 11, [0] = 4 } }; // (2x + 1) * (3x + 4) = (6x^2 + 11x + 4) 
            yield return new object[] { new Day21.Day21_Polinom { [1] = 2, [0] = 1 } * new Day21.Day21_Polinom { [0] = 2 }, new Day21.Day21_Polinom { [1] = 4, [0] = 2 } }; // (2x + 1) * (2) = (4x+2) 
            yield return new object[] { new Day21.Day21_Polinom { [1] = 4, [0] = 2 } / new Day21.Day21_Polinom { [0] = 2 }, new Day21.Day21_Polinom { [1] = 2, [0] = 1 } }; // (4x + 2) / (2) = (2x + 1)
        }
    }
}
