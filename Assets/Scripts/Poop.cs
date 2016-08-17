using UnityEngine;
using UnityEngine.EventSystems;

public class Poop : MonoBehaviour
{
    public GameObject world;
    private int seed;

    void Start()
    {
        seed = Random.Range(int.MinValue, int.MaxValue);
        world = GameObject.Find("_WorldCreator");
    }

	// Update is called once per frame
	void Update ()
    {
        if ((seed + (int)(Time.time * 1000)) % 2111 == 0) // Poop not too often
        {
            ExecuteEvents.Execute<IBlockEventTarget>(world, null,
                    (x, y) => x.Poop(new IntVector3(transform.position + Vector3.up)));
        }
	}
}
