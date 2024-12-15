using AdventOfCode;
using AdventOfCode.Grids;

namespace Day15
{
    internal class Program
    {
        static void Main(string[] args)
        {
            AdventOfCodeRunner.Run<Day15_2024>(10092, 9021);
        }
    }

    internal class Day15_2024 : IDailyChallenge<int>
    {
        public int Part1(IEnumerable<string> inputLines)
        {
            var (board, directives) = ProcessInputPart1(inputLines);
            var robot = board.First(kvp => kvp.Value == '@').Key;

            foreach (var directive in directives)
            {
                (int Y, int X) vector = directive switch
                {
                    '<' => (0, -1),
                    '>' => (0, 1),
                    '^' => (-1, 0),
                    'v' => (1, 0),
                    _ => throw new Exception($"Unexpected directive.")
                };

                var moved = Move1d(robot, vector, board);
                if (moved)
                {
                    robot = (robot.Y + vector.Y, robot.X + vector.X);
                }
            }

            return board.Where(kvp => kvp.Value == 'O').Select(kvp => 100 * kvp.Key.Y + kvp.Key.X).Sum();
        }


        public int Part2(IEnumerable<string> inputLines)
        {
            var (board, directives) = ProcessInputPart2(inputLines);
            var robot = board.First(kvp => kvp.Value == '@').Key;

            foreach (var directive in directives)
            {
                (int Y, int X) vector = directive switch
                {
                    '<' => (0, -1),
                    '>' => (0, 1),
                    '^' => (-1, 0),
                    'v' => (1, 0),
                    _ => throw new Exception($"Unexpected directive.")
                };

                var moved = Move2d(robot, vector, board);
                if (moved)
                {
                    robot = (robot.Y + vector.Y, robot.X + vector.X);
                }
            }

            return board.Where(kvp => kvp.Value == '[').Select(kvp => 100 * kvp.Key.Y + kvp.Key.X).Sum();
        }

        private bool Move2d(Coordinates coordinates, Vector2d vector, Dictionary<Coordinates, char> board)
        {
            if (!CanMove(coordinates, vector, board))
            {
                return false;
            }

            var next = coordinates.Translated(vector);

            switch (board[next])
            {
                case '.':
                    board[next] = board[coordinates];
                    board[coordinates] = '.';
                    return true;

                case '[':
                    Move2d(next, vector, board);
                    if (vector.X == 0)
                    {
                        Move2d((next.Y, next.X + 1), vector, board);
                    }

                    board[next] = board[coordinates];
                    board[coordinates] = '.';
                    return true;
                
                case ']':
                    Move2d(next, vector, board);
                    if (vector.X == 0)
                    {
                        Move2d((next.Y, next.X - 1), vector, board);
                    }

                    board[next] = board[coordinates];
                    board[coordinates] = '.';
                    return true;

                default:
                    throw new Exception("Invalid symbol.");
            }
        }

        private bool CanMove(Coordinates coordinates, Vector2d vector, Dictionary<Coordinates, char> board)
        {
            var next = coordinates.Translated(vector);
            if (!board.ContainsKey(next))
            {
                return false;
            }

            switch (board[next])
            {
                case '.':
                    return true;

                case '#':
                    return false;

                case '[':
                    return
                        vector.Y == 0 ?
                        CanMove(next, vector, board) :
                        CanMove(next, vector, board) && CanMove((next.Y, next.X + 1), vector, board);

                case ']':
                    return
                        vector.Y == 0 ?
                        CanMove(next, vector, board) :
                        CanMove(next, vector, board) && CanMove((next.Y, next.X - 1), vector, board);

                default:
                    throw new Exception("Invalid symbol.");
            }
        }

        private bool Move1d(Coordinates coordinates, Vector2d vector, Dictionary<Coordinates, char> board)
        {
            var next = coordinates.Translated(vector);
            if (!board.ContainsKey(next))
            {
                return false;
            }

            switch (board[next])
            {
                case '#':
                    return false;

                case '.':
                    board[next] = board[coordinates];
                    board[coordinates] = '.';
                    return true;

                default:
                    var moved = Move1d(next, vector, board);
                    if (moved)
                    {
                        board[next] = board[coordinates];
                        board[coordinates] = '.';
                    }

                    return moved;
            }
        }

        private void Print(Dictionary<Coordinates, char> board)
        {
            board.GroupBy(kvp => kvp.Key.Y).OrderBy(g => g.Key).ToList().ForEach(group =>
            {
                group.OrderBy(g => g.Key.X).ToList().ForEach(g => Console.Write(g.Value));
                Console.WriteLine();
            });

            Console.WriteLine();
        }

        private (Dictionary<Coordinates, char> Board, string Directives) ProcessInputPart1(IEnumerable<string> inputLines)
        {
            var input = inputLines.ToList();

            Dictionary<Coordinates, char> board = new();
            string directives;

            for (var y = 0; y < input.Count(); y++)
            {
                if (string.IsNullOrWhiteSpace(input[y]))
                {
                    directives = string.Join(string.Empty, inputLines.Skip(y).ToList());
                 
                    return(board, directives);
                }

                for (var x = 0; x < input.First().Length; x++)
                {
                    board[(y, x)] = input[y][x];
                }
            }

            throw new Exception("Invalid input data.");
        }

        private (Dictionary<Coordinates, char> Board, string Directives) ProcessInputPart2(IEnumerable<string> inputLines)
        {
            var input = inputLines.ToList();

            Dictionary<Coordinates, char> board = new();
            string directives;

            for (var y = 0; y < input.Count(); y++)
            {
                if (string.IsNullOrWhiteSpace(input[y]))
                {
                    directives = string.Join(string.Empty, inputLines.Skip(y).ToList());

                    return (board, directives);
                }

                for (var x = 0; x < input.First().Length; x++)
                {
                    switch (input[y][x])
                    {
                        case '#':
                            board[(y, 2 * x)] = '#';
                            board[(y, 2 * x + 1)] = '#';
                            break;

                        case 'O':
                            board[(y, 2 * x)] = '[';
                            board[(y, 2 * x + 1)] = ']';

                            break;
                        case '.':
                            board[(y, 2 * x)] = '.';
                            board[(y, 2 * x + 1)] = '.';
                            break;

                        case '@':
                            board[(y, 2 * x)] = '@';
                            board[(y, 2 * x + 1)] = '.';
                            break;
                    }

                }
            }

            throw new Exception("Invalid input data.");
        }
    }
}
