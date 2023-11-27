using System.Collections;
using System.Collections.Generic;
using SmoothMQTT.Core;
using UnityEngine;

namespace SmoothMQTT.ExampleScripts
{


    public class SetCredentialsInSettings : MonoBehaviour
    {
        public void SetUser(string user)
        {
            Settings.Instance.user = user;
        }
        public void SetPassword(string pw)
        {
            Settings.Instance.password = pw;
        }
    }
}