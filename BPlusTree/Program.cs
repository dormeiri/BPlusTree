using System;
using System.Collections.Generic;
using System.Linq;

namespace BPlusTree
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Write("Insert n: ");
            var n = int.Parse(Console.ReadLine());
            var tree = new Tree(n);

            while(true)
            {
                Console.Write("Insert value: ");
                var value = int.Parse(Console.ReadLine());
                tree.Insert(value);
                PrintTree(tree);
            }
        }

        private static void PrintTree(Tree tree)
        {
            int i = 0;
            IEnumerable<Node> nodes;
            while((nodes = tree.GetLevel(i))?.Any() == true)
            {
                Console.Write(i + ":\t");
                foreach(var node in nodes)
                {
                    Console.Write(string.Join('|', node.Values) + "   ");
                }
                Console.WriteLine();
                i++;
            }
        }
    }
}
