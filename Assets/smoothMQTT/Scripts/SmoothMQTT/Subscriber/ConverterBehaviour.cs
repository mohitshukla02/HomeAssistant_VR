using SmoothMQTT.Core;
using UnityEngine;

public abstract class ConverterBehaviour : MonoBehaviour
{
    public Subscriber subscriber;
    void OnEnable()
    {
        if (subscriber == null)
        {
            subscriber = GetComponent<Subscriber>();
        }

        subscriber.action.AddListener(OnAction);
    }
    void OnDisable()
    {
        subscriber.action.RemoveListener(OnAction);
    }

    public abstract void OnAction(string payload);
}
