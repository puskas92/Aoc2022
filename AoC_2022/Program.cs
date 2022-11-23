// See https://aka.ms/new-console-template for more information
using System.Diagnostics;

Console.WriteLine("AoC 2022");
var sw = new Stopwatch();
sw.Start();
//DayXX.DayXX_Main();

sw.Stop();
Console.WriteLine($"Code run under {sw.ElapsedMilliseconds}ms");
Console.ReadLine();