using System;
using System.IO;
using SmoothMQTT.Core;
using UnityEngine;
using UnityEngine.Events;

namespace SmoothMQTT.Subscribing
{
    [Obsolete("Converter behaviours are deprecated and will be removed in v2.0! \nPlease use Tools->SmoothMQTT->Update Converter Components to replace all outdated components in the scene and in the respective prefabs (if any).")]
    [HelpURL("https://smoothmqtt.schliesky.com/docs/1.1.0/user-guide/converter#subscriberquaternionconverter")]
    public class SubscriberQuaternionConverter : ConverterBehaviour
    {

        public QuaternionEvent action;
 
        private char[] trimchars = {'(', ')'};

        Quaternion QuaternionFromString(string input)
        {
            var data = input.Trim(trimchars).Split(',');
            float x, y, z, w;
            if (float.TryParse(data[0], out x) &&
                float.TryParse(data[1], out y) &&
                float.TryParse(data[2], out z) &&
                float.TryParse(data[3], out w)
            )
            {
                return new Quaternion(x, y, z, w);
            }

            throw new InvalidDataException($"Payload \"{input}\" could not be parsed to Quaternion!");
        }

        public override void OnAction(string payload)
        {
            var value = QuaternionFromString(payload);
            action.Invoke(value);
        }
    }

    [Serializable]
    public class QuaternionEvent : UnityEvent<Quaternion>
    {
        
    }
}