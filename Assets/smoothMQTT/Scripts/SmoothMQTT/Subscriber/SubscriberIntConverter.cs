using System;
using UnityEngine;
using UnityEngine.Events;

namespace SmoothMQTT.Subscribing
{

    [Obsolete("Converter behaviours are deprecated and will be removed in v2.0! \nPlease use Tools->SmoothMQTT->Update Converter Components to replace all outdated components in the scene and in the respective prefabs (if any).")]
    [HelpURL("https://smoothmqtt.schliesky.com/docs/1.1.0/user-guide/converter#subscriberintconverter")]
    public class SubscriberIntConverter : ConverterBehaviour
    {
        public IntEvent action;

        public override void OnAction(string payload)
        {
            var value = int.Parse(payload);
            action.Invoke(value);
        }
    }

    [Serializable]
    public class IntEvent : UnityEvent<int>
    {
    }
}