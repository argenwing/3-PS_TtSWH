using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A sorted linked list
/// </summary>
public class SortedLinkedList<T> : LinkedList<T> where T:IComparable
{

    #region Constructor

    public SortedLinkedList() : base()
    {
    }

    #endregion

    #region Methods

    /// <summary>
    /// Adds the given item to the list
    /// </summary>
    /// <param name="item">item to add to list</param>
    public void Add(T item)
    {
        // add your code here
        if (Count == 0)
        {
            // add to empty list
            AddFirst(item);
        }
        else if (First.Value.CompareTo(item) > 0)
        {
            // adding before head
            AddFirst(item);
        }
        else
        {
            LinkedListNode<T> previousNode = First;
            LinkedListNode<T> currentNode = First.Next;
            while (currentNode != null &&
                currentNode.Value.CompareTo(item) < 0)
            {
                previousNode = currentNode;
                currentNode = currentNode.Next;
            }

            if (currentNode == null)
            {
                LinkedListNode<T> tempItem = new LinkedListNode<T>(item);
                AddAfter(previousNode, tempItem);
            }
            else
            {
                LinkedListNode<T> tempItem = new LinkedListNode<T>(item);
                AddAfter(previousNode, tempItem);
            }

        }
    }

    /// <summary>
    /// Repositions the given item in the list by moving it
    /// forward in the list until it's in the correct
    /// position. This is necessary to keep the list sorted
    /// when the value of the item changes
    /// </summary>
    public void Reposition(T item)
    {
        // add your code here

        LinkedListNode<T> reposItem = Find(item);
        while (reposItem.Previous != null &&
            reposItem.Value.CompareTo(reposItem.Previous.Value) < 0)
        {
            reposItem.Value = reposItem.Previous.Value;
            reposItem.Previous.Value = item;
            reposItem = reposItem.Previous;
        }
    }
    #endregion
}
