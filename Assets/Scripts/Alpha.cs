using UnityEngine;
using UnityEngine.UI;

public class Alpha : MonoBehaviour
{
    public Button button;
    private Color colorToChange = new Color(1, 0, 0);

    // Start is called before the first frame update
    void Start()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(ChangeColor); // Add listener to the button click event
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void ChangeColor()
    {
        ColorBlock buttonColors = button.colors;
        buttonColors.normalColor = colorToChange;
        button.colors = buttonColors;
    }
}