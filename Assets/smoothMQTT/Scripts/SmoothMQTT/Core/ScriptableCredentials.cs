using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SmoothMQTT.Core
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "New user", menuName = "SmoothMQTT/User Credentials")]
    [HelpURL("https://smoothmqtt.schliesky.com/docs/next/user-guide/core-components#credentials")]
    public class ScriptableCredentials : ScriptableObject
    {
        public string username;
        public string password;

    }
}