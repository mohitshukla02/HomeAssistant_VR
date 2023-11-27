using System.Collections;
using System.Collections.Generic;
using SmoothMQTT.Core;
using UnityEngine;

namespace SmoothMQTT.Sending
{
    [HelpURL("https://smoothmqtt.schliesky.com/docs/next/user-guide/sending_publishing#publishcolor")]
   public class PublishColor : MonoBehaviour
   {
       public string topic;
       public Color color;
       public string prefix = "#";
       public void OnPublishColorHex()
       {
            var hexstring = $"{prefix}{(int) Mathf.Lerp(0, 255, color.r):X2}" +
                            $"{(int) Mathf.Lerp(0, 255, color.g):X2}" +
                            $"{(int) Mathf.Lerp(0, 255, color.b):X2}";
            _ = Publisher.Instance.OnSendMessage(topic, hexstring);
       }
    }
}