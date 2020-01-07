using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace ConsoleAppSample
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine(Enumerable.Range(0, 10).Sum());
            //Sort Algorithm
            SortAlgorithm.BubbleSort();
            SortAlgorithm.SelectionSort();

            int[] iAry = { 2, 3, -7, -9, 4, 1, 6 };
            //Greatest sum of a coniguous sub array 
            ArrayAlgorithm.GreatestSum(iAry);
            //Greatest sum of NON-CONTIGUOUS elements in int array 
            ArrayAlgorithm.GreatestSumOfNonAdjacentElements(iAry);
            //Greatest sum of a coniguous N-element sub array
            ArrayAlgorithm.GreatestSumOfNElementsRecur(iAry, 0, 3);

            //Longest comomon substring
            ArrayAlgorithm.LongestCommonSubstring();
            //Longest comomon sequence
            ArrayAlgorithm.LongestCommonSequence();

            //Tree Traversal 
            //Inorder, Preorder, Postorder, BST build from preorder sequence array
            new BinaryTree().BinaryTreeTraveral();
        }

    }

    
}
