using System;

public class PriorityQueue<T> where T : IComparable<T>
{
	private T[] data;
	public int Count { get; private set; }
	public int Capacity { get; private set; }

	public PriorityQueue(int capacity)
	{
		Count = 0;
		Capacity = capacity;
		data = new T[Capacity];
	}

	private void Expand()
	{
		T[] newData = new T[Capacity * 2];

		for (int i = 0; i < Count; i++)
		{
			newData[i] = data[i];
		}
		data = newData;
		Capacity *= 2;
	}

	private void Swap(ref T left, ref T right)
	{
		T temp = left;
		left = right;
		right = temp;
	}

	public void Enqueue(T value)
	{
		// 배열이 꽉 찼을 경우 확장 
		if (Count >= Capacity)
		{
			Expand();
		}

		// 데이터 추가
		data[Count] = value;
		Count++;

		// 데이터 정렬(힙 트리 구조 유지)
		int now = Count - 1;
		while (now > 0)
		{
			int parent = (now - 1) / 2;
			if (data[now].CompareTo(data[parent]) < 0)
				break;

			Swap(ref data[now], ref data[parent]);
			now = parent;
		}
	}

	public T Dequeue()
	{
		if (Count == 0)
		{
			throw new IndexOutOfRangeException();
		}

		// 루트 노드 값 추출, 마지막 노드와 교환 후 제거 
		T result = data[0];
		data[0] = data[Count - 1];
		data[Count - 1] = default(T);
		Count--;

		// 데이터 정렬(힙 트리 구조 유지)
		int now = 0;
		while (now < Count)
		{
			// 힙트리 구조의 자식 노드 인덱스 규칙
			int left = now * 2 + 1;
			int right = now * 2 + 2;
			int next = now;

			if (left < Count && data[next].CompareTo(data[left]) < 0)
				next = left;

			if (right < Count && data[next].CompareTo(data[right]) < 0)
				next = right;

			if (next == now)
				break;

			Swap(ref data[now], ref data[next]);
			now = next;
		}
		return result;
	}

	public T Peek()
	{
		if (Count == 0)
		{
			throw new IndexOutOfRangeException();
		}

		return data[0];
	}
}
