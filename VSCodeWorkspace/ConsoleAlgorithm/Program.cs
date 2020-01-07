using System;

namespace ConsoleAlgorithm
{
    class Program
    {
        static void Main(string[] args)
        {
            //SortAlgorithm.BubbleSort();
            //SortAlgorithm.SelectionSort();            
            ArrayAlgorithm.GreatestSumOfContiguousElementsInArray();
            int[] iAry = { 2, 3, -7, 9, -9, 1, 6 };
            Console.WriteLine($"Array:[{String.Join(',', iAry)}]");
            int max = ArrayAlgorithm.GreatestSumOfNonContiguousElementsInArray(iAry, iAry.Length-1);
            Console.WriteLine($"The greatest sum of non contiguous elements is: {max}");
            int min = ArrayAlgorithm.SmallestSumOfNonContiguousElementsInArray(iAry, iAry.Length-1);
            Console.WriteLine($"The smallest sum of non contiguous elements is: {min}");
            string sourceStr = "faabcdsbcdeff"; 
            string targetStr = "aaaabcdssbcdedd";
            Console.WriteLine($"Source string: {sourceStr} Target string: {targetStr}");
            Console.WriteLine($"The longest common string: {ArrayAlgorithm.LongestCommmonString(sourceStr,targetStr)}");
            Console.WriteLine($"The longest common sequence: {ArrayAlgorithm.LongestCommonSequence(sourceStr,targetStr)}");

            BinaryTree tree = new BinaryTree(); 
            tree.Root = new Node(20); 
            tree.Root.Left = new Node(8, tree.Root); 
            tree.Root.Right = new Node(22,tree.Root); 
            tree.Root.Left.Left = new Node(4,tree.Root.Left); 
            tree.Root.Left.Right = new Node(12,tree.Root.Left); 
            tree.Root.Left.Right.Left = new Node(10,tree.Root.Left.Right); 
            tree.Root.Left.Right.Right = new Node(14,tree.Root.Left.Right); 
            Node node = tree.LowestCommonAncestor(10,14);
            Console.WriteLine($"LCA of 10 and 14 is: {node.Key}");
            node = tree.LowestCommonAncestor(8,14);
            Console.WriteLine($"LCA of 8 and 14 is: {node.Key}");
            node = tree.LowestCommonAncestor(10,22);
            Console.WriteLine($"LCA of 10 and 22 is: {node.Key}");
        }
    }
}
