using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace AdventOfCode.TwentyTwentyTwo.Day7;
internal abstract class FileSystemMember
{
    public string Name { get; init; }
    public bool IsDirectory { get; init; }
    public Directory Parent { get; init; }

    public abstract int Size { get; }
}

internal class File : FileSystemMember
{
    private int size;

    public File(string name, int size, Directory parent)
    {
        this.Name = name;
        this.size = size;
    }

    public override int Size => this.size;
}

internal class Directory : FileSystemMember
{
    public Directory(string name, Directory parent)
    {
        this.Name = name;
        this.Parent = parent;
        this.IsDirectory = true;
    }

    public List<FileSystemMember> Children { get; set; } = new();

    public override int Size => this.GetSize();

    private int GetSize()
    {
        if (this.Children.Count == 0) return 0;

        return this.Children.Select(c => c.Size).Sum();
    }

    public IEnumerable<FileSystemMember> GetMembers(FileSystemMember parent)
    {
        if (parent is not Directory directory)
        {
            yield break;
        }

        foreach (var child in directory.Children)
        {
            yield return child;
            foreach (var grandchild in this.GetMembers(child))
            {
                yield return grandchild;
            }
        }
    }
}

internal class FileSystem
{
    public Directory Root { get; set; } = new Directory("/", null);
    public Directory CurrentDirectory { get; set; }

    public IEnumerable<FileSystemMember> GetAllMembers()
    {
        return this.Root.GetMembers(this.Root);
    }

    public void ParseLog(string line)
    {
        if (line == "$ cd /")
        {
            this.CurrentDirectory = this.Root;
            return;
        }

        if (line == "$ cd ..")
        {
            this.CurrentDirectory = this.CurrentDirectory.Parent;
            return;
        }

        if (line.StartsWith("$ cd "))
        {
            var dirName = line.Substring("$ cd ".Length);
            this.CurrentDirectory = this.CurrentDirectory.Children.OfType<Directory>().Single(m => m.Name == dirName);
            return;
        }

        var fileMatch = Regex.Match(line, @"(?<size>\d+) (?<name>.*)");
        if (fileMatch.Success)
        {
            var name = fileMatch.Groups["name"].Value;
            var size = int.Parse(fileMatch.Groups["size"].Value);

            this.CurrentDirectory.Children.Add(new File(name, size, this.CurrentDirectory));
            return;
        }

        var directoryMatch = Regex.Match(line, @"dir (?<name>.*)");
        if (directoryMatch.Success)
        {
            this.CurrentDirectory.Children.Add(new Directory(directoryMatch.Groups["name"].Value, this.CurrentDirectory));
            return;
        }
    }
}

internal class Program
{
    static void Main(string[] args)
    {
        var fileSystem = new FileSystem();
        Input.Process(line => fileSystem.ParseLog(line));

        // Part 1
        var part1 = fileSystem.GetAllMembers().Where(m => m.IsDirectory && m.Size < 100000).Select(d => d.Size).Sum();
        Console.WriteLine($"Part 1: {part1}");

        // Part 2
        var diskSize = 70000000;
        var requiredSpace = 30000000;
        var usedSpace = fileSystem.Root.Size;
        var spaceToFree = requiredSpace - (diskSize - usedSpace);

        var part2 = fileSystem.GetAllMembers().OfType<Directory>().OrderBy(m => m.Size).SkipWhile(m => m.Size <= spaceToFree).First().Size;
        Console.WriteLine($"Part 2: {part2}");
    }


}
