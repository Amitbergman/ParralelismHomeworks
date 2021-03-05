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
        //String[] files = Directory.GetFiles(args[1]);
        // Parallel.For(0, files.Length,
        //            index => {
        //              FileInfo fi = new FileInfo(files[index]);
        //          long size = fi.Length;
        //            Interlocked.Add(ref totalSize, size);
        //    });
        // Read the file and display it line by line.  
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


        long[] sorted =  mergeSort(array, 0, array.Length - 1);
        sw.Stop();

        long microseconds = sw.ElapsedTicks / (Stopwatch.Frequency / (1000L * 1000L));
        Console.WriteLine("Merge sort: {0}", microseconds);
        //printarray(sorted);
    }
    private static long[] mergeSort(long[] arr, int start, int end)
    {
        
        long[] a = { };
        if (start + 3 >= end)
        { //we have only 4 items to sort

            a = arr.Skip(start).Take(end - start + 1).ToArray();

            Array.Sort(a);
            
            return a;
        }

        int middle = start + (end - start) / 2;

        long[] left = mergeSort(arr, start, middle - 1);
        long[] right = mergeSort(arr, middle, end);


        return mergeSortedLists(left, right);

    }

    private static void printarray(long[] a)
    {
        for (int i = 0; i < a.Length; i++)
        {
            Console.WriteLine(a[i]);
        }
    }

    private static long[] mergeSortedLists(long[] left, long[] right)
    {
        int lengthOfNewArray = left.Length + right.Length;

        long[] mergedArray = new long[lengthOfNewArray];

        int indexNew = 0;
        int indexLeft = 0;
        int indexRight = 0;
        while (indexNew < lengthOfNewArray)
        {
            if (indexRight == right.Length)
            {
                mergedArray[indexNew] = left[indexLeft];
                indexLeft += 1;
                indexNew += 1;
                continue;
            }
            if (indexLeft == left.Length)
            {
                mergedArray[indexNew] = right[indexRight];
                indexRight += 1;
                indexNew += 1;
                continue;
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
