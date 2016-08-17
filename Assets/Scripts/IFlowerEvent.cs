using UnityEngine.EventSystems;
public interface IFlowerEvent : IEventSystemHandler
{
    void Flower(IntVector3 pos);
}