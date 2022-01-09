using System;

public static class ExtensionMethods
{
    public static int[] QuickSort(this int[] array)
    {
        QuickSortAlgorithm(array, 0, array.Length - 1);
        
        void QuickSortAlgorithm(int[] array, int left, int right)
        {
            if (left >= right)
            {
                return;
            }

            int pivot = array[(left + right) / 2];
            int partitionIndex = Partition(array, left, right, pivot);
            
            QuickSortAlgorithm(array, left, partitionIndex - 1);
            QuickSortAlgorithm(array, partitionIndex, right);
        }

        int Partition(int[] array, int left, int right, int pivot)
        {
            while (left <= right)
            {
                while (array[left] < pivot)
                {
                    left++;
                }

                while (array[right] > pivot)
                {
                    right--;
                }

                if (array[left] <= array[right])
                {
                    Swap(array, array[left], array[right]);
                    left++;
                    right--;
                }
            }

            return left;
        }
        
        void Swap(int[] array, int first, int second)
        {
            (array[first], array[second]) = (array[second], array[first]);
        }
        
        return array;
    }
}
