using System;
using System.Collections.Generic;
using System.Linq;
using SmoothMQTT.Core;
using UnityEditor;
using UnityEngine;

namespace SmoothMQTT.Sending
{

    #if TextMeshPro
    using TMPro;
    [HelpURL("https://smoothmqtt.schliesky.com/docs/next/user-guide/sending_publishing#publishfromtmpinputfield")]

    [RequireComponent(typeof(TMP_InputField))]
    public class PublishFromTMPInputField : MonoBehaviour
    {
        public string topic;
        public TMP_InputField inputField
        {
            get
            {
                if (_inputField == null)
                {
                    _inputField = GetComponent<TMP_InputField>();
                }

                return _inputField;
            }
        }

        private PublishValue _publisher;
        private TMP_InputField _inputField;

        public void OnSendInputField()
        {
            _ = Publisher.Instance.OnSendMessage(topic, _inputField.text);
        }
    }
#else
    [HelpURL("https://smoothmqtt.schliesky.com/docs/next/user-guide/sending_publishing#publishfromtmpinputfield")]

    public class PublishFromTMPInputField : MonoBehaviour
    {
        // Empty class if TextMeshPro is not installed
    }
#endif
}