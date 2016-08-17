using UnityEngine;
using UnityEngine.UI;

public class IncrementSlider : MonoBehaviour, IFlowerEvent
{
    public int max;

    private float count;
    private Slider slider;

    void Awake()
    {
        slider = GetComponentInChildren<Slider>();	
	}

    public void Flower(IntVector3 pos)
    {
        slider.value = ++count / max;

        if ((int)count == max)
        {
            Text[] texts = GetComponentsInChildren<Text>();
            texts[texts.Length-1].enabled = true;
            Destroy(texts[texts.Length-1].gameObject, 5);
        }
    }
}
