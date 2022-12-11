using AdventOfCode;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Day10
{
    internal interface ICpuOperation
    {
        int CyclesToComplete { get; set; }
        void Callback();
    }

    internal class NoopOperation : ICpuOperation
    {
        public int CyclesToComplete { get; set; } = 1;
        
        public void Callback()
        {
        }
    }

    internal class AddxOperation : ICpuOperation
    {
        private readonly Cpu cpu;
        private readonly int value;

        public AddxOperation(Cpu cpu, int value)
        {
            this.cpu = cpu;
            this.value = value;
        }

        public int CyclesToComplete { get; set; } = 2;

        public void Callback()
        {
            this.cpu.Register += this.value;
        }
    }

    internal class Cpu
    {
        private ICpuOperation currentOperation;

        public event Action<int> CycleStarted;

        public Queue<ICpuOperation> OperationsQueue { get; set; } = new();
        public int Register { get; set; } = 1;
        public int CycleNumber { get; set; } = 0;

        public void Run()
        {
            while(this.OperationsQueue.Count > 0)
            {
                this.CycleStarted?.Invoke(++this.CycleNumber);
                
                this.DrawPixel();
                
                if (currentOperation is null)
                {
                    this.currentOperation = this.OperationsQueue.Dequeue();
                }

                this.currentOperation.CyclesToComplete--;
                if (this.currentOperation.CyclesToComplete == 0)
                {
                    this.currentOperation.Callback();
                    this.currentOperation = null;
                }
            }
        }

        private void DrawPixel()
        {
            const int ScreenWidth = 40;
            const int ScreenHeight = 6;
            const int VerticalOffset = 0;
            const int HorizontalOffset = 20;

            var pixelsDrawn = this.CycleNumber - 1;

            var y = (pixelsDrawn / ScreenWidth) % ScreenHeight;
            var x = pixelsDrawn % ScreenWidth;

            var spriteCenterPosition = this.Register % ScreenWidth;

            Console.SetCursorPosition(HorizontalOffset + x, VerticalOffset + y);
            if (x >= spriteCenterPosition - 1 && x <= spriteCenterPosition + 1)
            {
                Console.BackgroundColor = ConsoleColor.White;
            }
            else
            {
                Console.BackgroundColor = ConsoleColor.DarkGray;
            }

            Console.Write(" ");
            Console.ResetColor();
        }
    }
    internal class Program
    {
        static void Main(string[] args)
        {
            var cpu = new Cpu();
            Input.Process(line =>
            {
                if (line == "noop")
                {
                    cpu.OperationsQueue.Enqueue(new NoopOperation());
                }
                else
                {
                    var value = int.Parse(line.Split(' ')[1]);
                    cpu.OperationsQueue.Enqueue(new AddxOperation(cpu, value));
                }
            });

            var strength = 0;
            var cyclesToCheck = new[] { 20, 60, 100, 140, 180, 220 };
            cpu.CycleStarted += (cycleNumber) =>
            {
                var index = cyclesToCheck.ToList().IndexOf(cycleNumber);
                if (index >= 0)
                {
                    Console.SetCursorPosition(0, index);
                    Console.WriteLine($"Cycle {cycleNumber}: {cpu.Register}");
                    strength += cpu.Register * cycleNumber;
                }
            };

            cpu.Run();

            // Part 1:
            Console.SetCursorPosition(0, 8);
            Console.WriteLine($"Part 1: {strength}");
        }
    }
}