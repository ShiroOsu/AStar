using System;

namespace AStar
{
    public interface IHeapItem<in T> : IComparable<T>
    {
        public int HeapIndex { get; set; }
    }
    
    public class Heap<T> where T : IHeapItem<T>
    {
        private readonly T[] m_Items;

        public Heap(int maxHeapSize)
        {
            m_Items = new T[maxHeapSize];
        }

        public void Add(T item)
        {
            item.HeapIndex = Count;
            m_Items[Count] = item;
            SortUp(item);
            Count++;
        }

        public T RemoveFirstItem()
        {
            var firstItem = m_Items[0];
            Count--;
            m_Items[0] = m_Items[Count];
            m_Items[0].HeapIndex = 0;
            SortDown(m_Items[0]);
            
            return firstItem;
        }

        public bool Contains(T item)
        {
            return Equals(m_Items[item.HeapIndex], item);
        }

        public int Count { get; private set; }

        public void UpdateItem(T item) => SortUp(item);
        
        private void SortDown(T item)
        {
            while (true)
            {
                var childIndexLeft = item.HeapIndex * 2 + 1;
                var childIndexRight = item.HeapIndex * 2 + 2;

                if (childIndexLeft < Count)
                {
                    var swapIndex = childIndexLeft;

                    if (childIndexRight < Count)
                    {
                        if (m_Items[childIndexLeft].CompareTo(m_Items[childIndexRight]) < 0)
                        {
                            swapIndex = childIndexRight;
                        }
                    }

                    if (item.CompareTo(m_Items[swapIndex]) < 0)
                    {
                        Swap(item, m_Items[swapIndex]);
                    }
                    else return;
                }
                else return;
            }
        }

        private void SortUp(T item)
        {
            var parentIndex = (item.HeapIndex - 1) / 2;

            while (true)
            {
                var parentItem = m_Items[parentIndex];
                if (item.CompareTo(parentItem) > 0)
                {
                    Swap(item, parentItem);
                }
                else break;

                parentIndex = (item.HeapIndex - 1) / 2;
            }
        }

        private void Swap(T a, T b)
        {
            m_Items[a.HeapIndex] = b;
            m_Items[b.HeapIndex] = a;
            (a.HeapIndex, b.HeapIndex) = (b.HeapIndex, a.HeapIndex);
        }
    }
}