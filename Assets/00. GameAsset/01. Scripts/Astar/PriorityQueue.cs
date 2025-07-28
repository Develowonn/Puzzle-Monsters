using System;

public class PriorityQueue<T> where T : IComparable<T>
{
    private T[]     data;
    public  int     Count    { get; private set; }
    public  int     Capacity { get; private set; }

    public PriorityQueue()
    {
        Count    = 0;
        Capacity = 1;
        data     = new T[Capacity];
    }

    public PriorityQueue(int capacity)
    {
        Count    = 0;
        Capacity = capacity;
        data     = new T[Capacity];
    }

    private void Expand()
    {
        T[] newData = new T[Capacity * 2];

        for(int i = 0; i < Count; i++)
        {
            newData[i] = data[i];
        }
        data      = newData;
        Capacity *= 2;
    }

    private void Swap(ref T left, ref T right)
    {
        T temp = left;
        left   = right;
        right  = temp;
    }

    public void Enqueue(T value)
    {
        // �迭�� �� á�� ��� Ȯ�� 
        if(Count >= Capacity)
        {
            Expand();
        }

        // ������ �߰�
        data[Count] = value;
        Count++;

        // ������ ����(�� Ʈ�� ���� ����)
        int now = Count - 1;
        while(now > 0)
        {
            int parent = (now - 1) / 2;
            if(data[now].CompareTo(data[parent]) < 0)
            {
                break;
            }

            Swap(ref data[now], ref data[parent]);
            now = parent;
        }
    }

    public T Dequeue()
    {
        if(Count == 0)
        {
            throw new IndexOutOfRangeException();
        }

        // ��Ʈ ��� �� ����, ������ ���� ��ȯ �� ���� 
        T result = data[0];
        data[0]  = data[Count - 1];
        data[Count - 1] = default(T);
        Count--;

        // ������ ����(�� Ʈ�� ���� ����)
        int now = 0;
        while(now < Count)
        {
            // ��Ʈ�� ������ �ڽ� ��� �ε��� ��Ģ
            int left  = now * 2 + 1;
            int right = now * 2 + 2;

            int next = now;

            if(left < Count && data[next].CompareTo(data[left]) < 0)
            {
                next = left;
            }

            if (right < Count && data[next].CompareTo(data[right]) < 0)
            {
                next = left;
            }

            if (next == now)
            {
                break;
            }

            Swap(ref data[now], ref data[next]);
            now = next;
        }
        return result;
    }

    public T Peek()
    {
        if(Count == 0)
        {
            throw new IndexOutOfRangeException();
        }

        return data[0];
    }
}
