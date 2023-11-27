using UnityEngine;
using UnityEngine.UI;

public class ToggleAnimation : MonoBehaviour
{
    [SerializeField] RectTransform uiHandleRectTransform;
    [SerializeField] Color backgroundActiveColor;
    [SerializeField] Color handleActiveColor;

    Image backgroundImage, handleImage;

    Color backgroundDefaultColor, handleDefaultColor;

    Toggle toggle;

    Vector2 handlePosition;

    private bool isOn = false;

    void Awake()
    {
        toggle = GetComponent<Toggle>();

        handlePosition = uiHandleRectTransform.anchoredPosition;

        backgroundImage = uiHandleRectTransform.parent.GetComponent<Image>();
        handleImage = uiHandleRectTransform.GetComponent<Image>();

        backgroundDefaultColor = backgroundImage.color;
        handleDefaultColor = handleImage.color;

        toggle.onValueChanged.AddListener(OnSwitch);

        if (toggle.isOn)
            OnSwitch(true);
    }

    public void OnSwitch(bool on)
    {
        if (on)
        {
            uiHandleRectTransform.anchoredPosition = handlePosition * -1;
            backgroundImage.color = backgroundActiveColor;
            handleImage.color = handleActiveColor;
            Debug.Log(toggle.isOn);
        }

        else
        {
            uiHandleRectTransform.anchoredPosition = handlePosition;
            backgroundImage.color = backgroundDefaultColor;
            handleImage.color = handleDefaultColor;
            Debug.Log(toggle.isOn);
        }
           
    }

    void OnDestroy()
    {
        toggle.onValueChanged.RemoveListener(OnSwitch);
    }
}