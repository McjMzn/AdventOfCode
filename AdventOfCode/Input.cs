using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode
{
    public class Input
    {
        public static void Process(Action<string> onLineRead)
        {
            var lines = File.ReadAllLines("input.txt");
            foreach(var line in lines)
            {
                onLineRead(line);
            }
        }
    }
}
