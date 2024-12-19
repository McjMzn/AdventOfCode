using AdventOfCode;
using System.Diagnostics;

namespace Day17
{
    internal class Program
    {
        static void Main(string[] args)
        {
            AdventOfCodeRunner.Run<Day17_2024>(4635635210, runDemo: false);
        }
    }

    internal class Day17_2024 : IDailyChallenge<string>
    {
        public string Part1(IEnumerable<string> inputLines)
        {
            var (regA, regB, regC, intructions) = ProcessInput(inputLines);

            var cpu = new Processor();
            cpu.RegisterA = regA;
            cpu.RegisterB = regB;
            cpu.RegisterC = regC;

            var output = cpu.RunProgram(intructions);
            
            return output;
        }

        public string Part2(IEnumerable<string> inputLines)
        {
            var (regA, regB, regC, instructions) = ProcessInput(inputLines);

            /*
                2,4 // Bst(4) - RegB = RegA % 8

                1,3 // Bxl(3) - RegB = RegB ^ 3

                7,5 // Cdv(5) - RegC = RegA / (int)Math.Pow(2, RegB)

                0,3 // Adv(3) - RegA = RegA / 8

                1,5 // Bxl(5) - RegB = RegB ^ 5

                4,4 // Bxc(4) - RegB = RegB ^ RegC;

                5,5 // Out(5) - Output(RegB % 8)

                3,0 // Jnz - if RegA not 0, jump to 0
            */

            var found = new List<long>();
            FindInitialRegisterAValue(0, 0, instructions, found);

            return found.Min().ToString();
        }

        private void Emulate(long a)
        {
            while (a != 0)
            {
                long b = a % 8;
                b = b ^ 3;

                long c = a / (long)(Math.Pow(2, b));

                a = a / 8;

                b = b ^ 5;
                b = b ^ c;

                Console.WriteLine($"RA: {a} Out: {b % 8}");
            }
        }

        private void ListAllOffsets(long startingA, long target)
        {
            Console.WriteLine($"-------- {startingA} --------");
            for (var i = 0; i < 8; i++)
            {
                long a = startingA + i;
                long b = a % 8;
                b = b ^ 3;

                long c = a / (long)(Math.Pow(2, b));

                a = a / 8;

                b = b ^ 5;
                b = b ^ c;

                var result = b % 8;

                if (result == target)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                }

                Console.WriteLine($"+{i} | {startingA + i} => {result}");
                Console.ResetColor();
            }
        }

        private void FindInitialRegisterAValue(long value, int step, List<int> instructions, List<long> found)
        {
            if (step == instructions.Count)
            {
                found.Add(value);
                return;
            }

            for (var i = 0; i < 8; i++)
            {
                long a = value + i;
                long b = a % 8;
                b = b ^ 3;

                long c = a / (long)(Math.Pow(2, b));

                a = a / 8;

                b = b ^ 5;
                b = b ^ c;

                var result = b % 8;

                if (result != instructions[instructions.Count - 1 - step])
                {
                    continue;
                }

                Console.WriteLine($"{new string(' ', step * 2)} {value + i}");
                if (step + 1 == instructions.Count)
                {
                    found.Add(value + i);
                    return;
                }

                FindInitialRegisterAValue((value + i) * 8, step + 1, instructions, found);
            }
        }



        private (int RegA, int RegB, int RegC, List<int> instructions) ProcessInput(IEnumerable<string> inputLines)
        {
            var lines = inputLines.ToList();

            var registerA = int.Parse(lines[0].Split(':', StringSplitOptions.TrimEntries)[1]);
            var registerB = int.Parse(lines[1].Split(':', StringSplitOptions.TrimEntries)[1]);
            var registerC = int.Parse(lines[2].Split(':', StringSplitOptions.TrimEntries)[1]);

            var instructions = lines[4].Split(':', StringSplitOptions.TrimEntries)[1].Split(',').Select(int.Parse).ToList();
        
            return (registerA, registerB, registerC, instructions);
        }
    }

    internal class Processor
    {
        private Dictionary<int, Action> _interpreter;

        private int _pointer;
        private int[] _program;

        private List<int> _outputBuffer;

        public Processor()
        {
            _interpreter = new()
            {
                [0] = Adv,
                [1] = Bxl,
                [2] = Bst,
                [3] = Jnz,
                [4] = Bxc,
                [5] = Out,
                [6] = Bdv,
                [7] = Cdv,
            };
        }

        public long RegisterA { get; set; }
        
        public long RegisterB { get; set; }
        
        public long RegisterC { get; set; }

        public string RunProgram(IEnumerable<int> program)
        {
            _program = program.ToArray();
            _pointer = 0;
            _outputBuffer = new List<int>();

            while (true)
            {
                try
                {
                    var opcode = _program[_pointer];
                    _pointer++;

                    _interpreter[opcode]();
                }
                catch (IndexOutOfRangeException)
                {
                    return string.Join(',', _outputBuffer);
                }
            }
        }

        private long Combo(int operand)
        {
            return operand switch
            {
                0 => 0,
                1 => 1,
                2 => 2,
                3 => 3,
                4 => RegisterA,
                5 => RegisterB,
                6 => RegisterC,
                7 => throw new Exception("Operand not allowed")
            };
        }

        private void Adv()
        {
            var operand = _program[_pointer++];

            RegisterA = RegisterA / (int)Math.Pow(2, Combo(operand));
        }

        private void Bxl()
        {
            var operand = _program[_pointer++];
            RegisterB = RegisterB ^ operand;
        }

        private void Bst()
        {
            var operand = _program[_pointer++];
            RegisterB = Combo(operand) % 8;
        }

        private void Jnz()
        {
            if (RegisterA == 0)
            {
                return;
            }

            var operand = _program[_pointer++];
            _pointer = operand;
        }

        private void Bxc()
        {
            var operand = _program[_pointer++];
            RegisterB = RegisterB ^ RegisterC;
        }

        private void Out()
        {
            var operand = _program[_pointer++];
            _outputBuffer.Add((int)Combo(operand) % 8);
        }

        private void Bdv()
        {
            var operand = _program[_pointer++];

            RegisterB = RegisterA / (int)Math.Pow(2, Combo(operand));
        }

        private void Cdv()
        {
            var operand = _program[_pointer++];

            RegisterC = RegisterA / (int)Math.Pow(2, Combo(operand));
        }
    }

}
