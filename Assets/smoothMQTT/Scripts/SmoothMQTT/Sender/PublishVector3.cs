using System.Collections;
using System.Collections.Generic;
using SmoothMQTT.Core;
using UnityEngine;

namespace SmoothMQTT.Sending
{
    [HelpURL("https://smoothmqtt.schliesky.com/docs/next/user-guide/sending_publishing#publishvector3")]
    public class PublishVector3 : MonoBehaviour
    {
        public string topic;
        public Vector3 value;
        public void OnPublishVector3()
        {
            _ = Publisher.Instance.OnSendMessage(topic, $"({value.x}, {value.y}, {value.z})");
        }
    }
}