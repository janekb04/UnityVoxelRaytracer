using System;
using System.Collections;
using System.Collections.Generic;

public class Array3D<T>
{
    private T[] data;
    public int SizeX { get; private set; }
    public int SizeY { get; private set; }
    public int SizeZ { get; private set; }

    public Array3D(int sizeX, int sizeY, int sizeZ)
    {
        data = new T[sizeX * sizeY * sizeZ];
        SizeX = sizeX;
        SizeY = sizeY;
        SizeZ = sizeZ;
    }

    public T this[int i, int j, int k]
    {
        get
        {
            return data[ThreeDimensionalIndex(i, j, k)];
        }
        set
        {
            data[ThreeDimensionalIndex(i, j, k)] = value;
        }
    }

    private int ThreeDimensionalIndex(int x, int y, int z)
    {
        return (x * SizeX * SizeY) + (y * SizeY) + z;
    }

    public T[] AsOneDimensionalArray()
    {
        return data;
    }
}
