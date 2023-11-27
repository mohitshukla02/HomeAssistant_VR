using System.Collections;
using System.Collections.Generic;
using SmoothMQTT.Core;
using UnityEngine;
using UnityEngine.Events;

namespace SmoothMQTT.Subscribing
{
    [HelpURL("https://smoothmqtt.schliesky.com/docs/next/user-guide/converter#compare-string-conditionalsubscriber")]
    public class ConditionalSubscriber : MonoBehaviour
    {
        public string compareString;

        public StringEvent trueAction;
        public StringEvent falseAction;

        public void OnAction(string payload)
        {
            if (payload.Equals(compareString))
            {
                trueAction.Invoke(payload);
            }
            else
            {
                falseAction.Invoke(payload);
            }
        }
    }
}