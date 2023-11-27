using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class ToggleAlpha : MonoBehaviour
{
    [SerializeField] Color backgroundActiveColor, backgroundActiveColor2;

    Color radialbgdfc, fillbgdfC;

    Image radialImage, fillImage;

    // Start is called before the first frame update
    void Start()
    {
        radialImage = GetComponent<Image>();
        fillImage = radialImage.GetComponentInChildren<Image>();
        radialbgdfc = radialImage.color;
        fillbgdfC = fillImage.color;

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AlphaToggle(bool on)
    {
        if (on)
        {
            radialImage.color = backgroundActiveColor;
            fillImage.color = backgroundActiveColor2;
        }

        else
        {
            radialImage.color = radialbgdfc;
            fillImage.color = fillbgdfC;
        }
    }
}
