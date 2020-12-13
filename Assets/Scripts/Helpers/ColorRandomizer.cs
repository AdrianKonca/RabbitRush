using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorRandomizer : MonoBehaviour
{
    public float hueMinValue;
    public float hueMaxValue;
    public float saturationMinValue;
    public float saturationMaxValue;
    public float colorMinValue;
    public float colorMaxValue;

    public Color PickRandomColor()
    {
        var hue = Random.Range(hueMinValue, hueMaxValue);
        var saturation = Random.Range(saturationMinValue, saturationMaxValue);
        var brigthness = Random.Range(colorMinValue, colorMaxValue);

        return Color.HSVToRGB(hue, saturation, brigthness);
    }

    //TODO: Add more generic solution
    void Start()
    {
        transform.GetChild(0).GetComponent<Renderer>().material.SetColor("_Color", PickRandomColor());
    }
}
