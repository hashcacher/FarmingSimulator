using System.Linq;
using UnityEngine;
using UnityEngine.UI;

using System.Collections;

public class Tutorial : MonoBehaviour
{
    private Text text;
    private int step;

    private string[] instructions =
    {
        "Use W A S D to move around",
        "Look at a nearby block and Left click to collect blocks",
        "Press 1-9 to hold other tools/blocks",
        "Right click to place a block",
        "To win, complete the instructions in the top left corner. Left click to begin"
    };

    private KeyCode[][] codes =
    {
        new KeyCode[] { KeyCode.W, KeyCode.A, KeyCode.S, KeyCode.D },
        new KeyCode[] { KeyCode.Mouse0 },
        new KeyCode[] { KeyCode.Alpha1, KeyCode.Alpha2, KeyCode.Alpha3, KeyCode.Alpha4, KeyCode.Alpha5,
            KeyCode.Alpha6, KeyCode.Alpha7, KeyCode.Alpha8, KeyCode.Alpha9},
        new KeyCode[] { KeyCode.Mouse1 },
        new KeyCode[] { KeyCode.Mouse0 }
    };

	void Awake ()
    {
        text = GetComponent<Text>();
        step = 0;	    
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (step < instructions.Length)
        {
            text.text = instructions[step];
            if (codes[step].Any(item => Input.GetKey(item))) step++;
        }
        else Destroy(this.gameObject);
	}
}
