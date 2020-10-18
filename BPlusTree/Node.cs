using System;
using System.Collections.Generic;
using System.Linq;

namespace BPlusTree
{
    public class Node
    {
        private readonly int n;
        private readonly int splitIdx;
        private readonly int?[] values;
        private readonly Node[] children;
        private int filled;
        private Node parent;

        internal Node(int n)
        {
            this.n = n;
            splitIdx = (int)Math.Ceiling((n - 1) / 2d);
            filled = 0;
            children = new Node[n];
            values = new int?[n - 1];
        }

        public IEnumerable<int?> Values => values.AsEnumerable();
        internal Node Root => IsRoot ? this : parent.Root;
        internal bool IsRoot => parent == null;
        private bool IsLeaf => children[0] == null;


        internal IEnumerable<Node> GetLevel(int x)
        {
            if (x == 0)
            {
                yield return this;
            }

            if (!IsLeaf)
            {
                for (var i = 0; i < filled + 1; i++)
                {
                    foreach (var node in children[i].GetLevel(x - 1))
                    {
                        yield return node;
                    }
                }
            }
        }

        internal Node Search(int value)
        {
            if (IsLeaf)
            {
                return this;
            }

            var i = 0;
            for (; i < n - 1 && values[i] <= value; i++) ;
            return children[i].Search(value);
        }

        internal void Insert(int value)
        {
            Search(value).Insert(value, null);
        }

        private Node Split(int comingValue, Node comingNode)
        {
            var newSibling = new Node(n);

            // Sort with the new value, keep the max value in a variable outside the array
            var overflowValue = comingValue;
            var overflowNode = comingNode;
            for (var i = 0; i < n - 1; i++)
            {
                var (tempValue, tempNode) = GetAt(i);
                if (overflowValue <= tempValue)
                {
                    SetAt(i, overflowValue, overflowNode);
                    (overflowValue, overflowNode) = (tempValue.Value, tempNode);
                }
            }

            // Copy values to the new node and reset values in the old node
            for (var i = splitIdx; i < n - 1; i++)
            {
                var (addValue, addNode) = GetAt(i);
                ResetAt(i);
                newSibling.SetAt(i - splitIdx, addValue.Value, addNode);
            }
            newSibling.SetAt(newSibling.filled, overflowValue, overflowNode);

            return newSibling;
        }

        private void Insert(int value, Node node)
        {
            // Check if there is a place for a new value
            if (filled < n - 1)
            {
                var insertIdx = 0;
                for (; values[insertIdx] <= value; insertIdx++) ;

                InsertAt(insertIdx, value, node);
            }
            else
            {
                var newSibling = Split(value, node);
                var midValueCopy = newSibling.GetAt(0).value.Value;

                if (!IsLeaf)
                {
                    newSibling.children[0] = newSibling.children[1];
                    newSibling.RemoveAt(0);
                }

                if (IsRoot)
                {
                    parent = new Node(n);
                    parent.children[0] = this;
                }

                parent.Insert(midValueCopy, newSibling);
            }
        }

        private void RemoveAt(int index)
        {
            for (var i = index; i < filled - 1; i++)
            {
                var (addValue, addNode) = GetAt(i + 1);
                ResetAt(i + 1);
                SetAt(i, addValue.Value, addNode);
            }
        }

        private void InsertAt(int index, int value, Node node)
        {
            for (var i = filled; i > index; i--)
            {
                var (addValue, addNode) = GetAt(i - 1);
                SetAt(i, addValue.Value, addNode);
            }
            SetAt(index, value, node);
        }

        private void SetAt(int index, int value, Node node)
        {
            // Check if value is added or updated
            if (values[index] == null)
            {
                filled++;
            }

            // Set
            values[index] = value;
            children[index + 1] = node;

            // Update node's parent
            if(node != null)
            {
                node.parent = this;
            }
        }

        private void ResetAt(int index)
        {
            if (values[index] != null)
            {
                values[index] = null;
                children[index + 1] = null;
                filled--;
            }
        }

        private (int? value, Node node) GetAt(int index)
        {
            return (values[index], children[index + 1]);
        }
    }
}
