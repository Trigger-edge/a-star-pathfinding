using UnityEngine;
using System.Collections;
using System;

// Heap structure used by A* 
public class Heap<T> where T : IHeapItem<T> {

	T[] items;
	int currentItemCount;

	// Constructor
	public Heap(int maxHeapSize) {

		items = new T[maxHeapSize];

	}

	// Add item to heap
	public void Add(T item) {

		item.HeapIndex = currentItemCount;
		items [currentItemCount] = item;

		SortUp (item);
		currentItemCount++;

	}

	// Remove top element from heap
	public T RemoveFirst() {
		
		T firstItem = items [0];
		currentItemCount--;

		items [0] = items[currentItemCount];
		items [0].HeapIndex = 0;

		SortDown (items [0]);

		return firstItem;

	}

	// Update item position
	public void UpdateItem(T item) {
		SortUp (item);
	}

	// Get number of items in heap
	public int Count {

		get {
			return currentItemCount;
		}

	}

	// Check if an item exists in heap
	public bool Contains(T item) {

		return Equals (items [item.HeapIndex], item);

	}

	// Sort up 
	void SortUp(T item) {
		int parentIndex = (item.HeapIndex - 1) / 2;

		while (true) {
			T parentItem = items [parentIndex];

			// If item has a higher priority than parent, swap it with parent
			if (item.CompareTo (parentItem) > 0) {
				Swap (item, parentItem);
			} else {
				break;
			}

			parentIndex = (item.HeapIndex - 1) / 2;
		}
	}

	// Sort down
	void SortDown(T item) {

		while (true) {
			int leftChildIndex = item.HeapIndex * 2 + 1;
			int rightChildIndex = item.HeapIndex * 2 + 2;
			int swapIndex = 0;

			if (leftChildIndex < currentItemCount) {
				swapIndex = leftChildIndex;

				if (rightChildIndex < currentItemCount) {
					// if left child has lower priority, set swap index to right child index
					if (items[leftChildIndex].CompareTo(items[rightChildIndex]) < 0) {
						swapIndex = rightChildIndex;
					}
				}

				// Check if parent needs to be swapped
				if (item.CompareTo (items [swapIndex]) < 0) {
					Swap (item, items [swapIndex]);
				} else {
					return;
				}
			} else {
				return;
			}
		}

	}

	// Swap two items in heap
	void Swap(T itemA, T itemB) {
		items [itemA.HeapIndex] = itemB;
		items [itemB.HeapIndex] = itemA;

		int itemAIndex = itemA.HeapIndex;
		itemA.HeapIndex = itemB.HeapIndex;
		itemB.HeapIndex = itemAIndex;
	}

}

public interface IHeapItem<T> : IComparable<T> {
	int HeapIndex {
		get;
		set;
	}
}
