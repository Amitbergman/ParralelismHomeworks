using System;
using System.IO;
using System.Threading.Tasks;
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
        int sizeOfTheArrayToSort = end - start + 1;
        long[] resultArray = new long[sizeOfTheArrayToSort];

        if (start == end)
        {
            //This is trivial - only 1 element to sort
            resultArray[0]=  arr[start];
            return Task.FromResult(resultArray);
        }

        if (end == start + 1)
        {
            //This is trivial - only 2 elements to sort
            resultArray[0] = Math.Min(arr[start], arr[end]);
            resultArray[1] = Math.Max(arr[start], arr[end]);
            return Task.FromResult(resultArray);
        }

        int middle = start + (end - start) / 2;
        Task<long[]> left;
        Task<long[]> right;
        if (numberOfCoresWeCanUse == 1)
        {
            left = mergeSort(arr, start, middle - 1, 1);
            Task.WhenAll(left);
            right = mergeSort(arr, middle, end, 1);
        }

        else
        {
            //Dividing the cores to ones that will work on the start of the array and ones that will work on the end
            int halfOfNodes = numberOfCoresWeCanUse / 2;
            left = mergeSort(arr, start, middle - 1, numberOfCoresWeCanUse - halfOfNodes);
            right = mergeSort(arr, middle, end, halfOfNodes);

        }

        Task.WhenAll(left, right);

        int middleOfBoth = sizeOfTheArrayToSort / 2;
        //Now I want to parralely merge both sides
        //We will divide the merge to half from start to middle and half from end to middle

        Task leftMerge = mergeSortedStartToIndex(left.Result, right.Result, middleOfBoth, resultArray);
        Task rightMerge = mergeSortedEndToIndex(left.Result, right.Result, middleOfBoth, resultArray);

        Task.WhenAll(leftMerge, rightMerge);
        return Task.FromResult(resultArray);

    }

    private static void printarray(long[] a)
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
        return Task.CompletedTask;
    }

    private static Task mergeSortedStartToIndex(long[] list1, long[] list2, int maxIndex, long[] resultArray)
    {
        int indexInList1 = 0;
        int indexInList2 = 0;
        int indexInResult = 0;
        while (indexInResult < maxIndex)
        {
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
            //This method will not write to max index
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
        return Task.CompletedTask;
    }

}
