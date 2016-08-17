using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public struct IntVector3
{
    public int x, y, z;

    public IntVector3(int x, int y, int z)
    {
        this.x = x;
        this.y = y;
        this.z = z;
    }

    public IntVector3(Vector3 v3) : this((int)v3.x, (int)v3.y, (int)v3.z) { }

    public IEnumerator GetEnumerator()
    {
        throw new NotImplementedException();
    }

    public Vector3 ToVector3()
    {
        return new Vector3(x, y, z);
    }

    public override String ToString()
    {
        return x + " " + y + " " + z;
    }
}
