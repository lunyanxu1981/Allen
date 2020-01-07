using System;

namespace ConsoleAlgorithm
{
    public static class SortAlgorithm
    {
        public static void BubbleSort()
        {
            int[] iAry = { 5, 3, 2, 1, 55, 3, 9 };
            Console.WriteLine($"Before BubbleSort: [{string.Join(',', iAry)}]");
            bool swapped = false;
            for (int i = 0; i < iAry.Length - 1; i++)
            {
                for (int j = 0; j < iAry.Length - i - 1; j++)
                {
                    if (iAry[j] > iAry[j + 1])
                    {
                        iAry[j] = iAry[j] + iAry[j + 1];
                        iAry[j + 1] = iAry[j] - iAry[j + 1];
                        iAry[j] = iAry[j] - iAry[j + 1];
                        swapped = true;
                    }
                }
                if (swapped == false)
                    break;
            }
            Console.WriteLine($"After BubbleSort: [{string.Join(',', iAry)}]");
        }

        public static void SelectionSort()
        {
            int[] iAry = { 5, 3, 2, 1, 55, 3, 9 };
            Console.WriteLine($"Before SelectionSort: [{string.Join(',', iAry)}]");

            for (int i = 0; i < iAry.Length - 1; i++)
            {
                int minIdx = i;
                for (int j = i + 1; j < iAry.Length; j++)
                {
                    if (iAry[minIdx] > iAry[j])
                    {
                        
                        minIdx = j;
                    }
                }
                if(minIdx != i)
                {
                    iAry[minIdx] = iAry[minIdx] + iAry[i];
                    iAry[i] = iAry[minIdx] - iAry[i];
                    iAry[minIdx] = iAry[minIdx] - iAry[i];
                }
            }
            Console.WriteLine($"After SelectionSort: [{string.Join(',', iAry)}]");
        }
    }
}