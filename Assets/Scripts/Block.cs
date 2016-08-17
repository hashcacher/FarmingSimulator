using UnityEngine;
using System.Collections;

public struct Block
{
    public Utils.BlockTypes type;
    public bool visible;
    public bool created;
    public GameObject go;

    public Block(Utils.BlockTypes type, bool visible)
    {
        this.type = type;
        this.visible = visible;
        created = true;
        go = null;
    }
    public Block(Utils.BlockTypes type, bool visible, GameObject go)
    {
        this.type = type;
        this.visible = visible;
        created = true;
        this.go = go;
    }
}
