using System;
using System.Globalization;
using UnityEngine;
using UnityEngine.Events;
using SmoothMQTT.Core;
namespace SmoothMQTT.Subscribing
{

    [Obsolete("Converter behaviours are deprecated and will be removed in v2.0! \nPlease use Tools->SmoothMQTT->Update Converter Components to replace all outdated components in the scene and in the respective prefabs (if any).")]
    [HelpURL("https://smoothmqtt.schliesky.com/docs/1.1.0/user-guide/converter#subscribercolorconverter")]
    public class SubscriberColorConverter : ConverterBehaviour
    {
        [Header("Color Events (Materials are changed permanently!)")]
        public ColorEvent action;
        
        public override void OnAction(string payload)
        {
            if (payload.StartsWith("#"))
            {
                var r = int.Parse(payload.Substring(1, 2),NumberStyles.HexNumber);
                var g = int.Parse(payload.Substring(3, 2),NumberStyles.HexNumber);
                var b = int.Parse(payload.Substring(5, 2),NumberStyles.HexNumber);
                action.Invoke(new Color(r/255f, g/255f, b/255f));
            }
        }
    }
    
    [Serializable]
    public class ColorEvent : UnityEvent<Color>
    {
        
    }
}