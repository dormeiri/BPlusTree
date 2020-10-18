using System.Collections.Generic;

namespace BPlusTree
{
    public class Tree
    {
        private Node root;


        public Tree(int n)
        {
            root = new Node(n);
        }


        public IEnumerable<Node> GetLevel(int level)
        {
            return root.GetLevel(level);
        }

        public Node Search(int value)
        {
            return root.Search(value);
        }

        public void Insert(int value)
        {
            root.Insert(value);
            root = root.Root;
        }
    }
}
