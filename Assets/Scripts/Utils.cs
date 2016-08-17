using System.Collections.Generic;
using UnityEngine;

public class Utils
{
    public static IEnumerable<IntVector3> GetNeighbors(IntVector3 pos)
    {
        foreach (IntVector3 neighbor in GetNeighbors(pos.x, pos.y, pos.z))
        {
            yield return neighbor;
        }
    }

    // These should be in the same order as the Block Prefabs in World Creator
    public enum BlockTypes
    {
        Gravel, Dirt, DirtGrass, Grass, Stone, Water, Poop, Flower1, Flower2, Flower3, Flower4
    }

    public static BlockTypes RandomFlower()
    {
        return (BlockTypes)Random.Range((int)BlockTypes.Flower1, (int)BlockTypes.Flower4 + 1);
    }

    public static IEnumerable<IntVector3> GetNeighbors(int tx, int ty, int tz)
    {
        for (int x = -1; x <= 1; x++)
        {
            for (int z = -1; z <= 1; z++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    if (!(x == 0 && y == 0 && z == 0))
                    {
                        yield return new IntVector3(tx + x, ty + y, tz + z);
                    }
                }
            }
        }
    }

    public static bool InBounds(IntVector3 pos, IntVector3 size)
    {
        int x = pos.x;
        int y = pos.y;
        int z = pos.z;
        return !(x >= size.x || x < 0 || y >= size.y || y < 0 || z >= size.z || z < 0);
    }

    public static IEnumerable<IntVector3> GetNeighborsInBounds(IntVector3 pos, IntVector3 size)
    {
        foreach (IntVector3 neighbor in GetNeighbors(pos.x, pos.y, pos.z))
        {
            if (InBounds(neighbor, size)) yield return neighbor;
        }
    }

    internal static int AbsMaxIndex(float[] sides)
    {
        int maxIndex = -1;
        float maxValue = float.MinValue; // Immediately overwritten anyway

        int index = 0;
        foreach (float value in sides)
        {
            if (Mathf.Abs(value) > maxValue || maxIndex == -1)
            {
                maxIndex = index;
                maxValue = Mathf.Abs(value);
            }
            index++;
        }
        return maxIndex;
    }
}
