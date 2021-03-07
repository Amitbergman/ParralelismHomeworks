using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;

public class Example
{
    public static void Main()
    {

        String[] args = Environment.GetCommandLineArgs();
        string fileName = args[2];
        int numberOfCores = int.Parse(args[1]);

        string line;
        List<long> listOfNumbersToSort = new List<long>();

        StreamReader file =
            new StreamReader(fileName);
        while ((line = file.ReadLine()) != null)
        {
            long toAdd = long.Parse(line);
            listOfNumbersToSort.Add(toAdd);
        }

        file.Close();

        long[] array = listOfNumbersToSort.ToArray();
        Stopwatch sw = new Stopwatch();
        sw.Start();


        long[] sorted =  mergeSort(array, 0, array.Length - 1, numberOfCores).GetAwaiter().GetResult();
        sw.Stop();

        long microseconds = sw.ElapsedTicks / (Stopwatch.Frequency / (1000L * 1000L));
        Console.WriteLine("Merge sort: {0}", microseconds);
        
        
        //Testings 
        long lastObjectShouldBe = 9223355229966065229;
        long thirdObjectShouldBe = 65602082527067;
        if (lastObjectShouldBe == sorted[sorted.Length - 1] && sorted[2] == thirdObjectShouldBe)
        {
            Console.Write("No problems in the accuracy");
        }
        else
        {
            Console.Write("Algorithm is not accurate");
        }
        //printarray(sorted);
    }
    private static Task<long[]> mergeSort(long[] arr, int start, int end, int numberOfCoresWeCanUse)
    {
        
        long[] a = new long[end - start + 1];

        if (start == end)
        {
            a[0] = arr[start];
            return Task.FromResult(a);
        }

        if (end == start + 1)
        {
            a[0] = Math.Min(arr[start], arr[end]);
            a[1] = Math.Max(arr[start], arr[end]);
            return Task.FromResult(a);
        }

        int middle = start + (end - start) / 2;
        Task<long[]> right;
        Task<long[]> left;
        if (numberOfCoresWeCanUse == 1)
        {
            left = mergeSort(arr, start, middle - 1, 1);
            Task.WhenAll(left);
            right = mergeSort(arr, middle, end, 1);
        }

        else
        {
            int halfOfNodes = numberOfCoresWeCanUse / 2;
            left = mergeSort(arr, start, middle - 1, numberOfCoresWeCanUse - halfOfNodes);
            right = mergeSort(arr, middle, end, halfOfNodes);

        }

        Task.WhenAll(left, right);

        return Task.FromResult(mergeSortedLists(left.Result, right.Result, 0, left.Result.Length-1, 0, right.Result.Length-1));

    }

    private static void printarray(long[] a)
    {
        for (int i = 0; i < a.Length; i++)
        {
            Console.WriteLine(a[i]);
        }
    }

    private static long[] mergeSortedLists(long[] left, long[] right, int startLeft, int endLeft, int startRight, int endRight)
    {
        int lengthOfLeft = endLeft - startLeft + 1;
        int lengthOfRight = endRight - startRight + 1;
        int lengthOfNewArray = lengthOfLeft + lengthOfRight;
        long[] mergedArray = new long[lengthOfNewArray];
        int indexNew = 0;
        int indexLeft = 0;
        int indexRight = 0;
        while (indexNew < lengthOfNewArray)
        {
            if (indexRight == right.Length)
            {
                //We finished the right one, reading all from left
                while (indexLeft < left.Length)
                {
                    mergedArray[indexNew] = left[indexLeft];
                    indexLeft += 1;
                    indexNew += 1;
                }
                return mergedArray;

            }
            if (indexLeft == left.Length)
            {
                //We finished the left one, reading all from right
                while (indexRight < right.Length)
                {
                    mergedArray[indexNew] = right[indexRight];
                    indexRight += 1;
                    indexNew += 1;

                }
                return mergedArray;
            }

            if (left[indexLeft] <= right[indexRight])
            {
                //Left is smaller
                mergedArray[indexNew] = left[indexLeft];
                indexLeft += 1;
                indexNew += 1;
                continue;
            }
            else
            {
                //Right is smaller
                mergedArray[indexNew] = right[indexRight];
                indexRight += 1;
                indexNew += 1;
                continue;
            }
        }

        return mergedArray;

    }

}
