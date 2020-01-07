using System.Collections.Generic;
namespace ConsoleAlgorithm
{
    public class BinaryTree
    {
        public Node Root = null;

        public Node FindNode(Node node, int key)
        {
            if (node == null)
            {
                return null;
            }
            if (node.Key == key)
            {
                return node;
            }
            else
            {
                Node left = FindNode(node.Left, key);
                Node right = FindNode(node.Right, key);
                if (left != null || right != null)
                {
                    return left != null ? left : right;
                }
                else
                {
                    return null;
                }
            }
        }

        public Node LowestCommonAncestor(int aKey, int bKey)
        {
            Node aNode = FindNode(Root, aKey);
            if (aNode == null)
            {
                return null;
            }
            Node bNode = FindNode(Root, bKey);
            if (bNode == null)
            {
                return null;
            }
            List<Node> aPath = new List<Node>();
            aPath.Add(aNode);
            Node temp = aNode.Parent;
            while (temp != null)
            {
                aPath.Add(temp);
                temp = temp.Parent;
            }

            List<Node> bPath = new List<Node>();
            bPath.Add(bNode);
            temp = bNode.Parent;
            while (temp != null)
            {
                bPath.Add(temp);
                temp = temp.Parent;
            }

            for (int i = 0;i < aPath.Count; i++)
            { 
                for(int j=0 ;j<bPath.Count; j++)
                {
                    if(aPath[i].Key == bPath[j].Key)
                    {
                        return aPath[i];
                    }
                }
            }
            return null;
        }

    }

    public class Node
    {
        public int Key;
        public Node Left;
        public Node Right;
        public Node Parent;
        public Node(int key, Node parent = null)
        {
            Key = key;
            Left = Right = null;
            Parent = parent;
        }
    }
}