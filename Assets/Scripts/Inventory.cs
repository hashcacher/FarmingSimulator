using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System;
using UnityEngine.EventSystems;
using System.Collections;

public class Inventory : MonoBehaviour
{
    public NamedImage[] images;
    public NamedPrefab[] prefabs;

    private GameObject uiPanel;
    private Dictionary<string, int> amounts;

    // The UI mostly stems from this slots array.
    private List<string> slots;

    private GameObject equipment;
    private int currentEquipSlot;
    private Vector3 shovelPos;

    private WorldCreator world;

    void Awake()
    {
        uiPanel = GameObject.Find("/Canvas/InventoryPanel");
        world = GameObject.Find("_WorldCreator").GetComponent<WorldCreator>();
    }
    void Start ()
    {
        slots = new List<string>(10);
        slots.Add("Shovel");

        amounts = new Dictionary<string, int>();
        amounts["Shovel"] = 1;

        UpdateUI();

        // So we can replace the shovel with blocks later
        equipment = GetComponentInChildren<Swing>().gameObject;
        shovelPos = equipment.transform.localPosition;
    }

    // Update is called once per frame
    void Update ()
    {
        for (int i = (int)KeyCode.Alpha1; i < (int)KeyCode.Alpha9; i++)
        {
            if (Input.GetKeyDown((KeyCode)i) && slots.Count > i - (int)KeyCode.Alpha1)
            {
                Equip(i - (int)KeyCode.Alpha1);
            }
        }
        if(Input.GetAxis("Mouse ScrollWheel") > 0f)
        {
            Equip((currentEquipSlot - 1) % slots.Count);
        }
        else if (Input.GetAxis("Mouse ScrollWheel") < 0f)
        {
            Equip((currentEquipSlot + 1) % slots.Count);
        }

        if(Input.GetMouseButtonDown(1))
        {
            try
            {
                Utils.BlockTypes type = (Utils.BlockTypes)Enum.Parse(typeof(Utils.BlockTypes), slots[currentEquipSlot]);

                // If we get past that last line, it means we have a valid block equipped and can build
                Build(type);
            }
            catch (ArgumentException e) { } // We're ok. Non-buildable thing was equipped
        }
    }

    public void Build(Utils.BlockTypes type)
    {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit, 3.3f) && hit.transform.tag == "block")
        {
            Vector3 hitPos = hit.transform.position;

            // Figure out which side to build on
            Vector3 diff = this.transform.position - hitPos;
            float[] sides = { diff.x, diff.y, diff.z };
            int maxDiffIndex = Utils.AbsMaxIndex(sides);
            int adj = sides[maxDiffIndex] > 0 ? 1 : -1;

            IntVector3 buildPos;
            if (maxDiffIndex == 0)
            {
                buildPos = new IntVector3((int)hitPos.x + adj, (int)hitPos.y, (int)hitPos.z);
            }
            else if (maxDiffIndex == 1)
            {
                buildPos = new IntVector3((int)hitPos.x, (int)hitPos.y + adj, (int)hitPos.z);
            }
            else
            {
                buildPos = new IntVector3((int)hitPos.x, (int)hitPos.y, (int)hitPos.z + adj);
            }

            // Tell the world
            ExecuteEvents.Execute<IBlockEventTarget>(world.gameObject, null,
                    (x, y) => x.Build(buildPos, type));

            if (--amounts[slots[currentEquipSlot]] <= 0)
            {
                Destroy(uiPanel.transform.GetChild(currentEquipSlot).gameObject);
                slots.RemoveAt(currentEquipSlot);
                StartCoroutine(UpdateUINextFrame()); // Need to wait for the Destroy to go through
                Equip((currentEquipSlot - 1) % slots.Count); // Make sure we're not stuck equipping spent blocks
            }
            else UpdateSpecificItem(currentEquipSlot);
        }
    }

    private IEnumerator UpdateUINextFrame()
    {
        yield return new WaitForEndOfFrame();
        UpdateUI();
    }

    private void Equip(int slot)
    {
        if (slot == currentEquipSlot) return;
        GameObject newEquipment = Instantiate(
            GetPrefab(slots[slot]), 
            equipment.transform.position, 
            equipment.transform.rotation) as GameObject;
        Destroy(equipment);
        equipment = newEquipment;

        equipment.transform.SetParent(this.transform);

        // Cubes need to be more forward and Swing
        if (slots[slot] != "Shovel")
        {
            equipment.transform.localPosition = shovelPos + new Vector3(0, -.6f, 1.3f);
            equipment.AddComponent<Swing>();
        }
        else
        {
            equipment.transform.localPosition = shovelPos;

        }
        currentEquipSlot = slot;
    }

    public void Add(Utils.BlockTypes type)
    {
        string typeStr = type.ToString();

        if (amounts.ContainsKey(typeStr)) amounts[typeStr]++;
        else amounts[typeStr] = 1;

        int index = slots.IndexOf(typeStr);
        if (index == -1)
        {
            slots.Add(typeStr);
            index = slots.Count - 1;
        }

        UpdateSpecificItem(index);
    }

    // Updates the inventory UI based on the slots array.
    private void UpdateUI()
    {
        int i = 0;
        foreach (string item in slots)
        {
            if (amounts.ContainsKey(item))
            {
                UpdateSpecificItem(i);
                i++;
            }
            else
            {
                Debug.Log("item not in order list!");
            }
        }
    }

    private void UpdateSpecificItem(int index)
    {
        string item = slots[index];
        int nSlots = uiPanel.transform.childCount;
        int amount = amounts[item];

        RectTransform rt;

        // Create new gameobject if needed
        if (index >= nSlots)
        {
            rt = Instantiate(GetPrefab("uiItem")).GetComponent<RectTransform>();
            rt.SetParent(uiPanel.transform);
        }
        else
        {
            rt = uiPanel.transform.GetChild(index).GetComponent<RectTransform>();
        }

        rt.GetComponentInChildren<Text>().text = amount.ToString();
        rt.anchoredPosition = new Vector2(-200 + 50 * index, 0);
        rt.GetComponentInChildren<Image>().sprite = GetTexture(item);
        // Set active image
    }

    private Sprite GetTexture(string name)
    {
        foreach(NamedImage img in images)
        {
            if (img.name == name) return img.sprite;
        }
        return null;
    }

    private GameObject GetPrefab(string name)
    {
        foreach (NamedPrefab prefab in prefabs)
        {
            if (prefab.name == name) return prefab.prefab;
        }
        return null;
    }
}
