using AdventOfCode;
using System.Text.RegularExpressions;

namespace Day8
{
    internal class Node
    {
        private static Dictionary<string, Node> _cache = new();

        public string Name { get; set; }
        public int Id { get; set; }
        
        public Node Left { get; set; }

        public Node Right { get; set; }

        public static Node Get(string nodeName, int id = 0)
        {
            if (!_cache.ContainsKey(nodeName))
            {
                _cache.Add(nodeName, new Node { Name = nodeName });
            }

            var node = _cache[nodeName];
            if (id > 0)
            {
                node.Id = id;
            }

            return _cache[nodeName];
        }

        public override string ToString()
        {
            return $"{Left.Name} <-- {Name} --> {Right.Name}";
        }
    }

    internal class Program
    {
        static void Main(string[] args)
        {
            var nodes = new List<Node>();
            var counter = 1;

            string movePattern = null;

            Input.Process(line =>
            {
                if (Regex.IsMatch(line, "^[LR]+$"))
                {
                    movePattern = line;
                    return;
                }

                if (line.Length == 0)
                {
                    return;
                }

                var match = Regex.Match(line, @"(?<name>.*) = \((?<left>.*), (?<right>.*)\)");
                
                var name = match.Groups["name"].Value;
                var left = match.Groups["left"].Value;
                var right = match.Groups["right"].Value;

                var node = Node.Get(name, counter++);
                node.Left = Node.Get(left);
                node.Right = Node.Get(right);

                nodes.Add(node);
            });

            var startNode = Node.Get("AAA");
            var finishNode = Node.Get("ZZZ");

            long steps = 0;
            Node currentNode = startNode;
            
            while (currentNode != finishNode)
            {
                int index = (int)(steps++ % movePattern.Length);
                var direction = movePattern[index];
                
                if (direction == 'L')
                {
                    Console.WriteLine($"Moving LEFT from {currentNode.Name} to {currentNode.Left.Name}");
                    currentNode = currentNode.Left;
                }
                else
                {
                    Console.WriteLine($"Moving RIGHT from {currentNode.Name} to {currentNode.Right.Name}");
                    currentNode = currentNode.Right;
                }
            }

            Console.WriteLine($"Part 1: {steps}");

            steps = 0;

            Func<Node, int> stepsToZ = node =>
            {
                var stepsMade = 0;
                var current = node;
                while (!current.Name.EndsWith("Z"))
                {
                    int index = (int)(stepsMade++ % movePattern.Length);
                    var direction = movePattern[index];
                    if (direction == 'L')
                    {
                        current = current.Left;
                    }
                    else
                    {
                        current = current.Right;
                    }
                }

                return stepsMade;
            };

            var ghostNodes = nodes.Where(n => n.Name.EndsWith("A")).ToList();
            var nodesSteps = ghostNodes.Select(stepsToZ).ToList();

            // Calculate LCM for values in nodeSteps. I jused used an online solver for this.
            Console.WriteLine($"Part 2: <computed online>");
        }
    }
}