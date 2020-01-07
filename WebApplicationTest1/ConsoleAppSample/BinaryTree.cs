using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleAppSample
{
    public class BinaryTree
    {
        public Node Root;

        public BinaryTree ()
        {
            Root = null;
        }

        public void InorderTraversal()
        {
            if (Root != null)
            {
                Console.WriteLine("Inorder Traversal");
                InorderTraversal(Root);
                Console.WriteLine();
            }
            else
                Console.WriteLine("Empty Tree");
        }
        /// <summary>
        /// Inorder (Left, Root, Right)
        /// </summary>
        /// <param name="node"></param>
        private void InorderTraversal(Node node)
        {
            if (node == null)
                return;

            InorderTraversal(node.Left);

            Console.Write($"{node.Key} ");

            InorderTraversal(node.Right);
        }

        public void PreorderTraversal()
        {
            if (Root != null)
            {
                Console.WriteLine("Pretorder Traversal");
                PreorderTraversal(Root);
                Console.WriteLine();
            }
            else
                Console.WriteLine("Empty Tree");
        }
        /// <summary>
        /// Preorder (Root, Left, Right)
        /// </summary>
        /// <param name="node"></param>
        private void PreorderTraversal(Node node)
        {
            if (node == null)
                return;

            Console.Write($"{node.Key} ");

            PreorderTraversal(node.Left);

            PreorderTraversal(node.Right);

        }

        public void PostorderTraversal()
        {

            if (Root != null)
            {
                Console.WriteLine("Postorder Traversal");
                PostorderTraversal(Root);
                Console.WriteLine();
            }
            else
                Console.WriteLine("Empty tree!");
        }
        /// <summary>
        /// Postorder (Left, Right, Root)
        /// </summary>
        /// <param name="node"></param>
        private void PostorderTraversal(Node node)
        {
            if (node == null)
                return;

            PostorderTraversal(node.Left);

            PostorderTraversal(node.Right);

            Console.Write($"{node.Key} ");
        }


        public void LevelOrderTraversal()
        {
            if (Root != null)
            {
                Console.WriteLine("Level Order Traversal");
                
                int height = GetTreeHeight(Root);
                
                Console.WriteLine($"Tree height: {height}");

                for (int i = 1; i <= height; i++)
                {
                    TraverseTreeLevel(Root, i);
                    Console.WriteLine();
                }
                Console.WriteLine();
            }
            else
                Console.WriteLine("Empty Tree");
        }

        private void TraverseTreeLevel(Node root, int level)
        {
            if (root == null)
                return;
            if (level == 1)
                Console.Write($"{root.Key} ");
            else
            {
                TraverseTreeLevel(root.Left, level - 1);
                TraverseTreeLevel(root.Right, level - 1);
            }
        }

        private int GetTreeHeight(Node root)
        {
            if (root == null)
                return 0;
            else
            {
                int leftTreeHeight = GetTreeHeight(root.Left);
                int rightTreeHeight = GetTreeHeight(root.Right);
                return Math.Max(leftTreeHeight, rightTreeHeight) + 1;
            }
        }

        public void BinaryTreeTraveral()
        {
            Node root = new Node(3);
            root.Left = new Node(9);
            root.Right = new Node(20);
            root.Right.Left = new Node(15);
            root.Right.Right = new Node(7);
            this.Root = root;
            InorderTraversal();
            Console.WriteLine();

            PreorderTraversal();
            Console.WriteLine();

            PostorderTraversal();
            Console.WriteLine();

            
            LevelOrderTraversal();

            int[] pre = new int[] { 10, 5, 1, 7, 40, 50 };
            int size = pre.Length;
            Index idx = new Index();
            Node bstNode = BuildBSTFromPreOrderTree(pre, idx, pre[0], Int32.MinValue, Int32.MaxValue, pre.Length);
            InorderTraversal(bstNode);

        }

        private Node BuildBSTFromPreOrderTree(int[] iAry, Index idx, int key, int min, int max, int length)
        {
            if (idx.index >= length)
                return null;
            Node node = null;
            if (key > min && key < max)
            {
                node = new Node(key);
                Console.WriteLine($"New node:{node.Key} Current Index: {idx.index} min:{min} max:{max}");
                idx.index++;
                if (idx.index < length)
                {
                    node.Left = BuildBSTFromPreOrderTree(iAry, idx, iAry[idx.index], min, key, length);
                    if (node.Left != null)
                        Console.WriteLine($"Left:{node.Left.Key} of Root {node.Key} Current Index: {idx.index}");
                    node.Right = BuildBSTFromPreOrderTree(iAry, idx, iAry[idx.index], key, max, length);
                    if (node.Right != null)
                        Console.WriteLine($"Right:{node.Right.Key} of Root {node.Key} Current Index: {idx.index}");


                }
            }
            return node;
        }

    }

    public class Index
    {
        public int index = 0;
    }

    public class Node
    {
        public int Key;
        public Node Left;
        public Node Right;

        public Node() { }

        public Node(int key)
        {
            Left = Right = null;
            Key = key;
        }
    }
}
