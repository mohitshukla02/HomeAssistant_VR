using System;
using System.Collections;
using System.Collections.Generic;
using SmoothMQTT.Core;
using UnityEngine;

namespace SmoothMQTT.Sending
{
    [HelpURL("https://smoothmqtt.schliesky.com/docs/next/user-guide/sending_publishing#publishquaternion")]
   public class PublishQuaternion : MonoBehaviour
    {
        public enum Source
        {
            ReferenceTransform,
            Quaternion,
            EulerAngles
        }
        public string topic;

        public Source type;
        public Transform copyRotationFrom;
        public Quaternion quaternion;
        public Vector3 eulerRotation;
        
        // Start is called before the first frame update

        public void OnPublishQuaternion()
        {
            Quaternion value;
            switch (type)
            {
                case Source.ReferenceTransform:
                    value = copyRotationFrom.rotation;
                    break;
                case Source.Quaternion:
                    value = quaternion;
                    break;
                case Source.EulerAngles:
                    value = Quaternion.Euler(eulerRotation);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            _ = Publisher.Instance.OnSendMessage(topic, $"({value.x}, {value.y}, {value.z}, {value.w})");
        }
    }
}