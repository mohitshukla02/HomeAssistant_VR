using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using SmoothMQTT.Core;

namespace ExampleScripts
{
    /// <summary>
    /// Helper script to illustrate how to publish float values to a topic in a time-based fashion
    /// </summary>
    public class PublishDelayedValues : MonoBehaviour
    {
        public List<float> values;
        public string topic;
        public float delay;

        private void Awake()
        {
            Settings.OnConnect += StartSending;
        }

        public void StartSending()
        {
            InvokeRepeating(nameof(OnPublish), delay, delay);
        }

        private void OnPublish()
        {
            if (values.Count > 0)
            {
                _ = Publisher.Instance.OnSendMessage(topic, values[0].ToString(CultureInfo.InvariantCulture));
                values.RemoveAt(0);
            }
            else
            {
                CancelInvoke(nameof(OnPublish));
            }
        }
    }
}
