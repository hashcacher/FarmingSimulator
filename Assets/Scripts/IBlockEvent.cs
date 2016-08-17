using UnityEngine;
using UnityEngine.EventSystems;

public interface IBlockEventTarget : IEventSystemHandler
{
    // functions that can be called via the messaging system
    void Break(Transform trans);
    void Build(IntVector3 pos, Utils.BlockTypes type);
    void Poop(IntVector3 pos);
}