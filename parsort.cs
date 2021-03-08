using System;
using System.IO;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

public class Example
{
    static int numberOfThreadsInUse = 1;
    public static void Main()
    {

        String[] args = Environment.GetCommandLineArgs();
        string fileName = args[2];
        int numberOfCoresWeCanUse = int.Parse(args[1]);

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

        long[] sorted = mergeSort(array, 0, array.Length - 1, numberOfCoresWeCanUse);
        sw.Stop();

        long microseconds = sw.ElapsedTicks / (Stopwatch.Frequency / (1000L * 1000L));
        Console.WriteLine("Merge sort: {0}", microseconds);

        printArrayToStandardOutput(sorted);
    }
    private static long[] mergeSort(long[] arr, int start, int end, int numberOfCoresWeCanUse)
    {
        
        int sizeOfTheArrayToSort = end - start + 1;
        long[] resultArray = new long[sizeOfTheArrayToSort];

        if (start == end)
        {
            //This is trivial - only 1 element to sort
            resultArray[0]=  arr[start];
            return resultArray;
        }

        if (end == start + 1)
        {
            //This is trivial - only 2 elements to sort
            resultArray[0] = Math.Min(arr[start], arr[end]);
            resultArray[1] = Math.Max(arr[start], arr[end]);
            return resultArray;
        }

        int middle = start + (end - start) / 2;
        long[] left = { };
        long[] right = { };
        
        //Declare that you need 2 more threads
        Interlocked.Add(ref numberOfThreadsInUse, 2);

        if (numberOfThreadsInUse > numberOfCoresWeCanUse)
        {
            //You cannot use these 2 threads, just run synchronousesly
            Interlocked.Add(ref numberOfThreadsInUse, -2);
            left = mergeSort(arr, start, middle - 1, 1);
            right = mergeSort(arr, middle, end, 1);
        }

        else
        {

            //Dividing the cores to ones that will work on the start of the array and ones that will work on the end
            int halfOfNodes = numberOfCoresWeCanUse / 2;
            Thread c  =  new Thread (()=> left = mergeSort(arr, start, middle - 1, halfOfNodes));
            Thread d  =  new Thread (()=> right = mergeSort(arr, middle, end, numberOfCoresWeCanUse - halfOfNodes));

            c.Start();
            d.Start();
            c.Join();
            d.Join();
            //They finihsed, so these threads are available now
            Interlocked.Add(ref numberOfThreadsInUse, -2);

        }

        int middleOfBoth = sizeOfTheArrayToSort / 2;
        //Now I want to parralely merge both sides
        //We will divide the merge to half from start to middle and half from end to middle

        //Declare that this thread wants to create 2 more
        Interlocked.Add(ref numberOfThreadsInUse, 2);

        if (numberOfThreadsInUse > numberOfCoresWeCanUse)
        {
            //You cannot use these 2 threads, just run synchronousesly
            Interlocked.Add(ref numberOfThreadsInUse, -2);
            mergeSortedStartToIndex(left, right, middleOfBoth, resultArray);
            mergeSortedEndToIndex(left, right, middleOfBoth, resultArray);
        }
        else
        {
            //You have threads to create this two
            Thread a = new Thread(() => mergeSortedStartToIndex(left, right, middleOfBoth, resultArray));
            Thread b = new Thread(() => mergeSortedEndToIndex(left, right, middleOfBoth, resultArray));
            a.Start();
            b.Start();

            a.Join();
            b.Join();
            //They finihsed, so these threads are available now
            Interlocked.Add(ref numberOfThreadsInUse, -2);

        }

        return resultArray;
    }

    private static void printArrayToStandardOutput(long[] a)
    {
        for (int i = 0; i < a.Length; i++)
        {
            Console.WriteLine(a[i]);
        }
    }

    private static Task mergeSortedEndToIndex(long[] list1, long[] list2, int maxIndex, long[] resultArray)
    {
        //This method will write to max index

        int indexInList1 = list1.Length - 1;
        int indexInList2 = list2.Length - 1;
        int indexInResult = resultArray.Length - 1;
        while (indexInResult >= maxIndex)
        {
            if (indexInList1 == -1)
            {
                //We need to put the value from list 2
                resultArray[indexInResult] = list2[indexInList2];
                indexInList2--;
                indexInResult--;
                continue;
            }
            if (indexInList2 == -1)
            {
                //We need to put the value from list 2
                resultArray[indexInResult] = list1[indexInList1];
                indexInList1--;
                indexInResult--;
                continue;
            }
            if (list1[indexInList1] > list2[indexInList2])
            {
                //We need to put the value from list 1
                resultArray[indexInResult] = list1[indexInList1];
                indexInList1--;
                indexInResult--;
            }
            else
            {
                //We need to put the value from list 2
                resultArray[indexInResult] = list2[indexInList2];
                indexInList2--;
                indexInResult--;
            }
        }
        return Task.FromResult(0);
    }

    /// <summary>
    /// This method merges the lists up until the index maxIndex of the merged one and put the result in resultArray
    /// </summary>
    /// <param name="list1"></param>
    /// <param name="list2"></param>
    /// <param name="maxIndex"></param>
    /// <param name="resultArray"></param>
    /// <returns></returns>
    private static Task mergeSortedStartToIndex(long[] list1, long[] list2, int maxIndex, long[] resultArray)
    {

        int indexInList1 = 0;
        int indexInList2 = 0;
        int indexInResult = 0;
        while (indexInResult < maxIndex)
        {
            //This method will not write to max index
            if (indexInList1 == list1.Length)
            {
                //We need to put the value from list 2
                resultArray[indexInResult] = list2[indexInList2];
                indexInList2++;
                indexInResult++;
                continue;
            }
            if (indexInList2 == list2.Length)
            {
                //We need to put the value from list 2
                resultArray[indexInResult] = list1[indexInList1];
                indexInList1++;
                indexInResult++;
                continue;
            }
            if (list1[indexInList1] < list2[indexInList2])
            {
                //We need to put the value from list 1
                resultArray[indexInResult] = list1[indexInList1];
                indexInList1++;
                indexInResult++;
            }
            else
            {
                //We need to put the value from list 2
                resultArray[indexInResult] = list2[indexInList2];
                indexInList2++;
                indexInResult++;
            }
        }
        return Task.FromResult(0);
    }

}
