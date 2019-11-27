using System;


namespace RoadFlow.Utility
{
    /// <summary>
    /// 排序扩展
    /// </summary>
    public static class SortExtention
    {

        #region  希尔排序法
        /// <summary>
        ///  希尔排序法,高效Shell Sort
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array">数组</param>
        public static void ShellSort<T>(T[] array) where T : IComparable
        {

            int length = array.Length; //数组的长度   
            for (int h = length / 2; h > 0; h = h / 2) //2^n~2^n-1 就有n-1循环
            {
                //here is insert sort
                for (int i = h; i < length; i++)  //最终h从1到最大数组长度 
                {
                    T temp = array[i];
                    if (temp.CompareTo(array[i - h]) < 0) //对比h间距的值小于0 进行以下循环
                    {
                        for (int j = 0; j < i; j += h)  //按着h间距递增的循环
                        {
                            if (temp.CompareTo(array[j]) < 0) //对比小于i的值，按着h间距并且进行替换操作
                            {
                                temp = array[j];
                                array[j] = array[i];
                                array[i] = temp;
                            }
                        }
                    }
                }
            }
        }
        #endregion

        #region  选择排序
        public static void SelectSort<T>(T[] arr) where T : IComparable
        {
            T temp;
            for (int i = 0; i < arr.Length - 1; i++)
            {
                T minVal = arr[i]; //假设 i 下标就是最小的数
                int minIndex = i;  //记录我认为最小的数的下标

                for (int j = i + 1; j < arr.Length; j++)   //这里只是找出这一趟最小的数值并记录下它的下标
                {
                    //说明我们认为的最小值，不是最小
                    if (minVal.CompareTo(arr[j]) > 0)    //这里大于号是升序(大于是找出最小值) 小于是降序(小于是找出最大值)
                    {
                        minVal = arr[j];  //更新这趟最小(或最大)的值 (上面要拿这个数来跟后面的数继续做比较)
                        minIndex = j;    //记下它的下标
                    }
                }
                //最后把最小的数与第一的位置交换
                temp = arr[i];    //把第一个原先认为是最小值的数,临时保存起来
                arr[i] = arr[minIndex];   //把最终我们找到的最小值赋给这一趟的比较的第一个位置
                arr[minIndex] = temp;  //把原先保存好临时数值放回这个数组的空地方，  保证数组的完整性
            }

        }

        #endregion

        #region 冒泡排序

        //冒泡排序方法，从小到大排，虽然很多冒泡排序都是从大到小，
        //可是我就想这么排，你能怎么着我。
        /// <summary>
        /// 第一个数对比后面每一个数，如果前面大于后面，就进行交换处理
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        public static T[] PopSort<T>(T[] list) where T : IComparable
        {
            int i, j;
            T temp;  //先定义一下要用的变量
            for (i = 0; i < list.Length - 1; i++)
            {
                for (j = i + 1; j < list.Length; j++)
                {
                    if (list[i].CompareTo(list[j]) > 0) //如果第二个小于第一个数
                    {
                        //交换两个数的位置，在这里你也可以单独写一个交换方法，在此调用就行了
                        temp = list[i]; //把大的数放在一个临时存储位置
                        list[i] = list[j]; //然后把小的数赋给前一个，保证每趟排序前面的最小
                        list[j] = temp; //然后把临时位置的那个大数赋给后一个
                    }
                }
            }

            return list;
        }


        //冒泡排序方法，从小到大排，虽然很多冒泡排序都是从大到小，
        //可是我就想这么排，你能怎么着我。
        /// <summary>
        /// 第一个数对比后面每一个数，如果前面大于后面，就进行交换处理
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        public static T[] PopSortDesc<T>(T[] list) where T : IComparable
        {
            int i, j;
            T temp;  //先定义一下要用的变量
            for (i = 0; i < list.Length - 1; i++)
            {
                for (j = i + 1; j < list.Length; j++)
                {
                    if (list[i].CompareTo(list[j]) < 0) //如果第二个小于第一个数
                    {
                        //交换两个数的位置，在这里你也可以单独写一个交换方法，在此调用就行了
                        temp = list[i]; //把大的数放在一个临时存储位置
                        list[i] = list[j]; //然后把小的数赋给前一个，保证每趟排序前面的最小
                        list[j] = temp; //然后把临时位置的那个大数赋给后一个
                    }
                }
            }

            return list;
        }



        #endregion



        #region 快速排序

        /// <summary>
        /// 方法的调用快速排序
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="numbers"></param>
        public static void KuaiSort<T>(T[] numbers) where T : IComparable
        {
            QuickSort<T>(numbers, 0, numbers.Length - 1);
        }
        /// <summary>
        /// 快速排序的方法：则中对半，对比左右两边，把左边小于右边的值
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="numbers">数组</param>
        /// <param name="left">左边的位置</param>
        /// <param name="right">右边的位置</param>
        private static void QuickSort<T>(T[] numbers, int left, int right) where T : IComparable
        {
            if (left < right)
            {
                T middle = numbers[(left + right) / 2]; //中间值
                int i = left - 1;
                int j = right + 1;
                while (true)
                {
                    while (numbers[++i].CompareTo(middle) < 0) ; //左边的值小于中间值
                    while (numbers[--j].CompareTo(middle) > 0) ;//右边的值大于中间值
                    if (i >= j)  //对比左边大于右边退出循环
                    {
                        break;
                    }
                    Swap<T>(numbers, i, j); //否则交换左右两边的值
                }

                QuickSort<T>(numbers, left, i - 1); //递归左边的快速排序
                QuickSort<T>(numbers, j + 1, right);//递归右边的快速排序
            }
        }

        /// <summary>
        /// 交替两个数值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="numbers"></param>
        /// <param name="i"></param>
        /// <param name="j"></param>
        private static void Swap<T>(T[] numbers, int i, int j)
        {
            T number = numbers[i];
            numbers[i] = numbers[j];
            numbers[j] = number;
        }


        #endregion



        #region 归并排序
        public static void MergeSort<T>(T[] array) where T : IComparable
        {
            MergeSortFunction<T>(array, 0, array.Length - 1);
        }


        //归并排序（目标数组，子表的起始位置，子表的终止位置）
        private static void MergeSortFunction<T>(T[] array, int first, int last) where T : IComparable
        {
            try
            {
                if (first < last)   //子表的长度大于1，则进入下面的递归处理
                {
                    int mid = (first + last) / 2;   //子表划分的位置
                    MergeSortFunction<T>(array, first, mid);   //对划分出来的左侧子表进行递归划分
                    MergeSortFunction<T>(array, mid + 1, last);    //对划分出来的右侧子表进行递归划分
                    MergeSortCore<T>(array, first, mid, last); //对左右子表进行有序的整合（归并排序的核心部分）
                }
            }
            catch  // Exception ex
            {

            }
        }

        //归并排序的核心部分：将两个有序的左右子表（以mid区分），合并成一个有序的表
        private static void MergeSortCore<T>(T[] array, int first, int mid, int last) where T : IComparable
        {
            try
            {
                int indexA = first; //左侧子表的起始位置
                int indexB = mid + 1;   //右侧子表的起始位置
                T[] temp = new T[last + 1]; //声明数组（暂存左右子表的所有有序数列）：长度等于左右子表的长度之和。
                int tempIndex = 0;
                while (indexA <= mid && indexB <= last) //进行左右子表的遍历，如果其中有一个子表遍历完，则跳出循环
                {
                    if (array[indexA].CompareTo(array[indexB]) <= 0) //此时左子表的数 <= 右子表的数
                    {
                        temp[tempIndex++] = array[indexA++];    //将左子表的数放入暂存数组中，遍历左子表下标++
                    }
                    else//此时左子表的数 > 右子表的数
                    {
                        temp[tempIndex++] = array[indexB++];    //将右子表的数放入暂存数组中，遍历右子表下标++
                    }
                }
                //有一侧子表遍历完后，跳出循环，将另外一侧子表剩下的数一次放入暂存数组中（有序）
                while (indexA <= mid)
                {
                    temp[tempIndex++] = array[indexA++];
                }
                while (indexB <= last)
                {
                    temp[tempIndex++] = array[indexB++];
                }

                //将暂存数组中有序的数列写入目标数组的制定位置，使进行归并的数组段有序
                tempIndex = 0;
                for (int i = first; i <= last; i++)
                {
                    array[i] = temp[tempIndex++];
                }
            }
            catch //(Exception ex)
            {

            }
        }

        #endregion




        #region 插入排序
        public static void insertSort<T>(T[] data) where T : IComparable
        {
            insertOnSort<T>(data, 1);
        }
        private static void insertOnSort<T>(T[] data, int index) where T : IComparable
        {
            if (index < data.Length)
            {
                T t = data[index];
                for (int i = index - 1; i >= 0; i--)
                {
                    if (data[i].CompareTo(t) > 0)
                    {
                        data[i + 1] = data[i];
                        data[i] = t;
                    }
                    else
                    {
                        data[i + 1] = t;
                        break;
                    }
                }
                insertOnSort<T>(data, index + 1);
            }
        }
        #endregion
    }
}
