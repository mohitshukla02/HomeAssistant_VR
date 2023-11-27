using System;
using System.IO;
using SmoothMQTT.Core;
using UnityEngine;
using UnityEngine.Events;

namespace SmoothMQTT.Subscribing
{
    [Obsolete("Converter behaviours are deprecated and will be removed in v2.0! \nPlease use Tools->SmoothMQTT->Update Converter Components to replace all outdated components in the scene and in the respective prefabs (if any).")]
    [HelpURL("https://smoothmqtt.schliesky.com/docs/1.1.0/user-guide/converter#subscribervector3converter")]
    public class SubscriberVector3Converter : ConverterBehaviour
    {
        public VectorEvent action;

        private char[] trimchars = {'(', ')'};
        private char splitchar = ',';

        public override void OnAction(string payload)
        {
            var value = Vector3FromString(payload);
            action.Invoke(value);
        }

        private Vector3 Vector3FromString(string input)
        {
            var data = input.Trim(trimchars).Split(splitchar);
            float x, y, z;
            if (float.TryParse(data[0], out x) &&
                float.TryParse(data[1], out y) &&
                float.TryParse(data[2], out z)
            )
            {
                return new Vector3(x, y, z);
            }

            throw new InvalidDataException($"Payload \"{input}\" could not be parsed to Vector3!");
        }

    }

    [Serializable]
    public class VectorEvent : UnityEvent<Vector3>
    {
        
    }
}