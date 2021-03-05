using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;

public class Example
{
    public static void Main()
    {

        //String[] args = Environment.GetCommandLineArgs();

        //String[] files = Directory.GetFiles(args[1]);
        // Parallel.For(0, files.Length,
        //            index => {
        //              FileInfo fi = new FileInfo(files[index]);
        //          long size = fi.Length;
        //            Interlocked.Add(ref totalSize, size);
        //    });
        int[] arr = { 1, 9, 4, 6 , 2};

        int[] sorted =  calc(arr, 0, arr.Length - 1);

        printarray(sorted);

    }


    private static int[] calc(int[] arr, int start, int end)
    {
        Console.WriteLine("Directory '{0}': {1}", start, end);


        int[] a = { };
        if (start + 3 >= end)
        { //we have only 4 items to sort

            a = arr.Skip(start).Take(end - start + 1).ToArray();

            Array.Sort(a);
            
            return a;
        }

        int middle = start + (end - start) / 2;

        int[] left = calc(arr, start, middle - 1);
        int[] right = calc(arr, middle, end);


        return merge(left, right);

    }

    private static void printarray(int[] a)
    {
        for (int i = 0; i < a.Length; i++)
        {
            Console.WriteLine(a[i]);
        }
    }

    private static int[] merge(int[] left, int[] right)
    {
        int lengthOfNewArray = left.Length + right.Length;

        int[] mergedArray = new int[lengthOfNewArray];

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

            if ( left[indexLeft] <= right[indexRight])
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
