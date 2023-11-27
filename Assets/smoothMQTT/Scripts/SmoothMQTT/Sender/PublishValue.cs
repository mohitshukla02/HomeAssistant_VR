using System.Globalization;
using UnityEngine;
using SmoothMQTT.Core;

namespace SmoothMQTT.Sending
{
    /// <summary>
    /// Collection of wrapper methods accepting different types and forwarding strings to the Sender for publishing.
    /// Thus, these methods can be used with Unity Actions returning a value (e.g. UI->Slider->OnValueChanged) on objects.
    /// </summary>
    [HelpURL("https://smoothmqtt.schliesky.com/docs/next/user-guide/sending_publishing#publishvalue")]
    public class PublishValue : MonoBehaviour
    {
        public string topic;
        public void OnPublishFloat(float value)
        {
            _ = Publisher.Instance.OnSendMessage(topic, value.ToString(CultureInfo.InvariantCulture));
        }

        public void OnPublishInt(int value)
        {
            _ = Publisher.Instance.OnSendMessage(topic, value.ToString());
        }

        public void OnPublishString(string value)
        {
            _ = Publisher.Instance.OnSendMessage(topic, value);
        }

        public void OnPublishDouble(double value)
        {
            _ = Publisher.Instance.OnSendMessage(topic, value.ToString(CultureInfo.InvariantCulture));
        }

        public void OnPublishBool(bool value)
        {
            _ = Publisher.Instance.OnSendMessage(topic, value.ToString());
        }
    }
}