
namespace InnerSortingAlgorithm {
    internal class InnerSort {
        static void Swap( ref int[] arr, long i, long j ) {
            (arr[i], arr[j]) = (arr[j], arr[i]);
        }
        static long ChooseMedianPivot( ref int[] arr, long low, long high ) {
            long first = low;
            long last = high;
            long middle = low + ( high - low ) / 2;

            long pivotIndex;
            if ( arr[first] < arr[middle] ) {
                if ( arr[middle] < arr[last] )
                    pivotIndex = middle;
                else if ( arr[first] < arr[last] )
                    pivotIndex = last;
                else
                    pivotIndex = first;
            } else {
                if ( arr[first] < arr[last] )
                    pivotIndex = first;
                else if ( arr[middle] < arr[last] )
                    pivotIndex = last;
                else
                    pivotIndex = middle;
            }

            Swap(ref arr, pivotIndex, high);

            return Partition(ref arr, low, high);
        }
        static long Partition( ref int[] arr, long low, long high ) {
            long pivot = arr[high];
            long i = ( low - 1 );

            for ( long j = low; j <= high - 1; j++ ) {
                if ( arr[j] < pivot ) {
                    i++;
                    Swap(ref arr, i, j);
                }
            }
            Swap(ref arr, i + 1, high);
            return ( i + 1 );
        }
        public void QuickSort( ref int[] arr, long low, long high ) {
            if ( low < high ) {
                if ( high - low < 11 ) {
                    long i, j;
                    int key;
                    for ( i = low; i <= high; ++i ) {
                        key = arr[i];
                        j = i - 1;
                        while ( j >= 0 && arr[j] > key ) {
                            arr[j + 1] = arr[j];
                            j -= 1;
                        }
                        arr[j + 1] = key;
                    }
                    return;
                }
                long pi = ChooseMedianPivot(ref arr, low, high);
                QuickSort(ref arr, low, pi - 1);
                QuickSort(ref arr, pi + 1, high);
            }
        }
    }
}
