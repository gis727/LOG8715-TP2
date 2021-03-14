using System;
using UnityEngine;

public class Array<T> where T: IComponent
{
    private T[] elements {get; set;}

    public Array(int length)
    {
        elements = new T[length];
    }

    // O(1)
    public T this[uint i]
    {  
        get { return elements[i]; }
        set { elements[i] = value; }
    }

    // O(1)
    public int Length
    {
        get { return elements.Length; }
    }

    // O(1)
    public void Remove(uint index)
    {
        elements[index] = default(T);
    }

    // O(1)
    public T Get(uint index)
    {
        return elements[index];
    }

    // O(1)
    public void Set(uint index, T element)
    {
        elements[index] = element;
    }

    // O(1)
    public bool Contains(uint index)
    {
        return elements[index] != null;
    }

    // O(N)
    public void Clear()
    {
        for(int i = 0; i < elements.Length; i++)
        {
            elements[i] = default(T);
        }
    }
}