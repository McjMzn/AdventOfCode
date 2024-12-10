using AdventOfCode;
using System.Text;

namespace Day9
{



    internal class Program
    {
        static void Main(string[] args)
        {
            AdventOfCodeRunner.Run<Day9_2024>(1928, 2858);
        }
    }

    internal class Cell
    {
        public int? FileId { get; set; }

        public int Size { get; set; } = 1;

        public override string ToString()
        {
            if (FileId is not null)
            {
                return $"{FileId}[{Size}]";
            }

            return $"-[{Size}]";
        }
    }

    internal class Day9_2024 : IDailyChallenge<long>
    {
        public long Part1(IEnumerable<string> inputLines)
        {
            var disk = PrepareInputPart1(inputLines);

            var emptySpaceIndex = disk.IndexOf(disk.First(c => c.FileId is null));
            var processingIndex = disk.Count - 1;

            while (processingIndex > emptySpaceIndex)
            {
                var cellToMove = disk[processingIndex];
                if (cellToMove.FileId is not null)
                {
                    disk[emptySpaceIndex].FileId = cellToMove.FileId;
                    cellToMove.FileId = null;
                }

                while (disk[emptySpaceIndex].FileId is not null)
                {
                    emptySpaceIndex++;
                }

                processingIndex--;
            }

            long checksum = 0;
            Cell previousCell = null;

            return CalculateChecksum(disk);
        }

        public long Part2(IEnumerable<string> inputLines)
        {
            var disk = PrepareInputPart2(inputLines);
            
            var queue = new Queue<Cell>(disk.Where(cell => cell.FileId is not null).Reverse());
            
            while (queue.Count > 0)
            {
                var cellToMove = queue.Dequeue();
                var cellToMoveIndex = disk.IndexOf(cellToMove);

                var newLocation = disk.Where((cell, index) => cell.FileId is null && cell.Size >= cellToMove.Size && index < cellToMoveIndex).FirstOrDefault();
                if (newLocation is null)
                {
                    continue;
                }

                var leftoverSize = newLocation.Size - cellToMove.Size;
                newLocation.FileId = cellToMove.FileId;
                newLocation.Size = cellToMove.Size;
                cellToMove.FileId = null;
                if (leftoverSize > 0)
                {
                    var index = disk.IndexOf(newLocation);
                    disk.Insert(index + 1, new Cell { Size = leftoverSize });
                }
            }

            return CalculateChecksum(disk);
        }

        private List<Cell> PrepareInputPart1(IEnumerable<string> inputLines)
        {
            var disk = new List<Cell>();

            var isFile = true;
            var filesAdded = 0;

            foreach (var character in Input.LoadLines().First())
            {
                var digit = int.Parse(character.ToString());
                for (int i = 0; i < digit; i++)
                {
                    if (isFile)
                    {
                        disk.Add(new Cell { FileId = filesAdded });
                    }
                    else
                    {
                        disk.Add(new Cell());
                    }
                }

                if (isFile)
                {
                    filesAdded++;
                }

                isFile = !isFile;
            }

            return disk;
        }

        private List<Cell> PrepareInputPart2(IEnumerable<string> inputLines)
        {
            var disk = new List<Cell>();

            var isFile = true;
            var filesAdded = 0;

            foreach (var character in Input.LoadLines().First())
            {
                var digit = int.Parse(character.ToString());
                if (isFile)
                {
                    disk.Add(new Cell { FileId = filesAdded, Size = digit });
                }
                else
                {
                    disk.Add(new Cell { Size = digit });
                }

                if (isFile)
                {
                    filesAdded++;
                }

                isFile = !isFile;
            }

            return disk;
        }

        private long CalculateChecksum(List<Cell> disk)
        {
            long checksum = 0;
            var globalIndex = 0;

            foreach(var cell in disk)
            {
                for (var i = 0; i < cell.Size; i++)
                {
                    if (cell.FileId is not null)
                    {
                        checksum += cell.FileId.Value * globalIndex;
                    }

                    globalIndex++;
                }
            }

            return checksum;
        }
    }
}
