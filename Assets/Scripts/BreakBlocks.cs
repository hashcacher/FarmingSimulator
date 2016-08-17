using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class BreakBlocks : MonoBehaviour
{

    private WorldCreator world;
    private Inventory inventory;
    private float lastTimeBrokeBlock;

    void Awake()
    {
        world = GameObject.Find("_WorldCreator").GetComponent<WorldCreator>();
        inventory = GetComponent<Inventory>();
    }
	
	// Update is called once per frame
	void FixedUpdate ()
    {
        if (Input.GetMouseButton(0) && lastTimeBrokeBlock + .3f < Time.time)
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit, 2.3f) && hit.transform.tag == "block")
            {
                IntVector3 pos = new IntVector3(hit.transform.position);
                inventory.Add(world.blocks[pos.x,pos.y,pos.z].type);

                // Updater and creator need to know about this event.
                ExecuteEvents.Execute<IBlockEventTarget>(world.gameObject, null, 
                    (x, y) => x.Break(hit.transform));

                // So we don't break blocks too fast
                lastTimeBrokeBlock = Time.time;
            }
        }
    }
}
