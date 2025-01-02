using AdventOfCode;
using AdventOfCode.Grids;

namespace Day21
{
    internal class Program
    {
        static void Main(string[] args)
        {
            AdventOfCodeRunner.Run<Day21_2024>(126384, 154115708116294);
        }
    }

    internal class Keypad
    {
        private  static IReadOnlyDictionary<char, Coordinates> _codeKeyboardMapping = new Dictionary<char, Coordinates>
        {
            ['7'] = (0, 0),
            ['8'] = (0, 1),
            ['9'] = (0, 2),

            ['4'] = (1, 0),
            ['5'] = (1, 1),
            ['6'] = (1, 2),

            ['1'] = (2, 0),
            ['2'] = (2, 1),
            ['3'] = (2, 2),

            ['-'] = (3, 0),
            ['0'] = (3, 1),
            ['A'] = (3, 2),
        };

        private static IReadOnlyDictionary<char, Coordinates> _directionalKeyboardMapping = new Dictionary<char, Coordinates>
        {
            ['-'] = (0, 0),
            ['^'] = (0, 1),
            ['A'] = (0, 2),

            ['<'] = (1, 0),
            ['v'] = (1, 1),
            ['>'] = (1, 2),
        };

        private IReadOnlyDictionary<char, Coordinates> _mapping;

        private char _currentKey = 'A';

        private Keypad()
        {
        }

        public List<string> GetPressCombinations(string sequence)
        {
            var branches = new List<List<char>>();
            foreach (var c in sequence)
            {
                var subBranches = GetRequiredPresses(c);
                if (branches.Count == 0)
                {
                    branches.AddRange(subBranches);
                }
                else
                {
                    var updated = new List<List<char>>();
                    foreach (var subBranch in subBranches)
                    {
                        branches.ForEach(b => updated.Add(b.Concat(subBranch).ToList()));
                    }

                    branches = updated;
                }

                _currentKey = c;
            }

            return branches.Select(b => new string(b.ToArray())).ToList();
        }

        private List<List<char>> GetRequiredPresses(char targetKey)
        {
            var presses = new List<char>();

            var targetPosition = _mapping[targetKey];
            var currentPosition = _mapping[_currentKey];
            var gapPosition = _mapping['-'];
            
            var branches = GetBranches(currentPosition, targetPosition);

            if (branches.Count == 0)
            {
                branches.Add(['A']);
            }
            else
            {
                branches.ForEach(b => b.Add('A'));
            }

            return branches;
        }

        private List<List<char>> GetBranches(Coordinates from,  Coordinates to)
        {
            if (from == to)
            {
                return new List<List<char>>();
            }

            var deltaX = to.X - from.X;
            var deltaY = to.Y - from.Y;
            
            var xDirection =
                deltaX == 0 ? 0 :
                deltaX < 0 ? -1 :
                1;

            var yDirection =
                deltaY == 0 ? 0 :
                deltaY < 0 ? -1 :
                1;

            var xSteps = xDirection == 0 ? 0 : deltaX / xDirection;
            var ySteps = yDirection == 0 ? 0 : deltaY / yDirection;


            var movements = new Dictionary<Vector2d, int>
            {
                [(yDirection, 0)] = ySteps,
                [(0, xDirection)] = xSteps,
            };

            Func<Vector2d, List<char>> createBranch = primaryVector =>
            {
                var branch = new List<char>();
                
                var primarySteps = movements[primaryVector];
                var secondaryVector = movements.First(kvp => kvp.Key != primaryVector).Key;
                var secondarySteps = movements[secondaryVector];

                var tPosition = from;

                var usePrimary = true;
                while (primarySteps + secondarySteps > 0)
                {
                    var vector = usePrimary ? primaryVector : secondaryVector;
                    var otherVector = vector == primaryVector ? secondaryVector : primaryVector;

                    if (usePrimary && primarySteps == 0)
                    {
                        vector = secondaryVector;
                        otherVector = primaryVector;
                    }

                    if (!usePrimary && secondarySteps == 0)
                    {
                        vector = primaryVector;
                        otherVector = secondaryVector;
                    }

                    // Take a turn in case of a gap.
                    if (tPosition.Translated(vector) == _mapping['-'])
                    {
                        branch.Add(GetSymbolForVector(otherVector));
                        tPosition = tPosition.Translated(otherVector);
                        secondarySteps--;
                        usePrimary = !usePrimary;
                        continue;
                    }

                    branch.Add(GetSymbolForVector(vector));
                    tPosition = tPosition.Translated(vector);
                    if (vector == primaryVector)
                    {
                        primarySteps--;
                    }
                    else
                    {
                        secondarySteps--;
                    }
                }

                return branch;
            };

            var xBranch = createBranch((0, xDirection));
            var yBranch = createBranch((yDirection, 0));

            // Return
            if (Enumerable.SequenceEqual(xBranch, yBranch))
            {
                return [ xBranch ];
            }

            return [ xBranch, yBranch ];
        }

        private char GetSymbolForVector(Vector2d vector)
        {
            return vector switch
            {
                (-1, 0) => '^',
                (1, 0) => 'v',
                (0, 1) => '>',
                (0, -1) => '<',
                _ => throw new NotSupportedException()
            };
        }

        public static Keypad CreateNumerical()
        {
            var keypad = new Keypad
            {
                _mapping = _codeKeyboardMapping,
            };

            return keypad;
        }

        public static Keypad CreateDirectional()
        {
            var keypad = new Keypad
            {
                _mapping = _directionalKeyboardMapping,
            };

            return keypad;
        }
    }


    internal class Day21_2024 : IDailyChallenge<long>
    {
        private Dictionary<(char, char), string> _directionalMoves = new();
        private Dictionary<(char, char, int), long> _cache = new();

        public Day21_2024()
        {
            // Good old manual setup.
            // Marked * records require in-practice checking as they produce same lenght next layer.
            _directionalMoves = new()
            {
                [('A', 'A')] = "A",
                [('A', '^')] = "<A",
                [('A', '>')] = "vA",
                [('A', 'v')] = "<vA", //* (<vA) or (v<A) => (<v<A>A^>A) or (<VA<A>^>A)
                [('A', '<')] = "v<<A",// (<v<A) or (v<<A) => (<v<A>A<A>^>A) or (<vA<AA>^>A)

                [('^', 'A')] = ">A",
                [('^', '^')] = "A",
                [('^', '>')] = "v>A", //* (>vA) or (v>A) => (vA<A^>A) or (v<A>A^A)
                [('^', 'v')] = "vA",
                [('^', '<')] = "v<A",

                [('>', 'A')] = "^A",
                [('>', '^')] = "<^A", //* (^<A) or (<^A) => (<Av<A>>^A) or (<v<A>^A>A)
                [('>', '>')] = "A",
                [('>', 'v')] = "<A",
                [('>', '<')] = "<<A",

                [('v', 'A')] = "^>A", //* (^>A) or (>^A) => (<Av>A^A) or (vA<^A>A)
                [('v', '^')] = "^A",
                [('v', '>')] = ">A",
                [('v', 'v')] = "A",
                [('v', '<')] = "<A",

                [('<', 'A')] = ">>^A", // (>^>A) or (>>^A) => (vA<^Av>A^A) or (vAA<^A>A)
                [('<', '^')] = ">^A",
                [('<', '>')] = ">>A",
                [('<', 'v')] = ">A",
                [('<', '<')] = "A",
            };
        }

        public int Part1_OriginalBruteforce(IEnumerable<string> inputLines)
        {
            var result = 0;

            foreach (var line in inputLines)
            {
                var combinations = new List<string>();

                for (var i = 0; i < 3; i++)
                {
                    if (i == 0)
                    {
                        var keypad = Keypad.CreateNumerical();
                        combinations.AddRange(keypad.GetPressCombinations(line));
                    }
                    else
                    {
                        var updated = new List<string>();
                        foreach (var combination in combinations)
                        {
                            var keypad = Keypad.CreateDirectional();
                            var directionalCombinations = keypad.GetPressCombinations(combination);
                            updated.AddRange(directionalCombinations);
                        }

                        combinations = updated;
                    }
                }

                var shortest = combinations.OrderBy(c => c.Length).First();

                var code = int.Parse(line.Substring(0, 3));
                var length = shortest.Length;
                result += code * length;

                Console.WriteLine($"{length} * {code} = {code * length}");
            }

            return result;
        }

        public long Part1(IEnumerable<string> inputLines)
        {
            return GetSumOfComplexities(inputLines, 4);
        }

        public long Part2(IEnumerable<string> inputLines)
        {
            return GetSumOfComplexities(inputLines, 27);
        }

        private long GetSumOfComplexities(IEnumerable<string> inputLines, int numberOfLayers)
        {
            var result = 0L;

            foreach (var line in inputLines)
            {
                var keypad = Keypad.CreateNumerical();
                var combinations = keypad.GetPressCombinations(line);

                var code = int.Parse(line.Substring(0, 3));

                var minimalLength = combinations.Select(firstRobotDirections =>
                {
                    var length = 0L;
                    for (var i = 0; i < firstRobotDirections.Length; i++)
                    {
                        length += CalculateInstructionsLength(firstRobotDirections[i], i == 0 ? 'A' : firstRobotDirections[i - 1], numberOfLayers - 2);
                    }

                    return length * code;
                }).Min();

                result += minimalLength;
            }

            return result;
        }

        private long CalculateInstructionsLength(char currentCharacter, char previousCharacter, int numberOfRobots)
        {
            if (_cache.ContainsKey((currentCharacter, previousCharacter, numberOfRobots)))
            {
                return _cache[(currentCharacter, previousCharacter, numberOfRobots)];
            }

            var translated = _directionalMoves[(previousCharacter, currentCharacter)];
            if (numberOfRobots == 0)
            {

                return 1;
            }

            var count = 0L;

            for (var i = 0; i < translated.Length; i++)
            {
                count += CalculateInstructionsLength(translated[i], i == 0 ? 'A' : translated[i - 1], numberOfRobots - 1);
            }

            _cache[(currentCharacter, previousCharacter, numberOfRobots)] = count;
            return count;
        }
    }
}
