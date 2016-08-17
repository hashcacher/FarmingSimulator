using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class DestroyOrFlower : MonoBehaviour
{
    private GameObject objectivesUI, world;

    void Start()
    {
        world = GameObject.Find("_WorldCreator");
        objectivesUI = GameObject.Find("/Canvas/ObjectivesPanel/FlowerObjective");
        StartCoroutine(SelfDestructAndFlower(Random.Range(10, 30)));
    }

    private IEnumerator SelfDestructAndFlower(int seconds)
    {
        yield return new WaitForSeconds(seconds);
        RaycastHit hit;
        if(Physics.Raycast(transform.position + Vector3.up*.1f, Vector3.down, out hit, 1f))
        {
            if(hit.transform.name == "Grass(Clone)")
            {
                ExecuteEvents.Execute<IFlowerEvent>(world, null,
                    (x, y) => x.Flower(new IntVector3(transform.position + Vector3.up)));
                ExecuteEvents.Execute<IFlowerEvent>(objectivesUI, null,
                    (x, y) => x.Flower(new IntVector3(transform.position + Vector3.up)));
            }
        }
        Destroy(this.gameObject);
    }
}
