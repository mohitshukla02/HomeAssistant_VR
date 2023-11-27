using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class LocalIP : MonoBehaviour
{
    private Text _text;


    private void OnValidate()
    {
        if (_text == null)
        {
            _text = GetComponent<Text>();
        }
        var ip = MyIP();
        if (ip != null)
        {
            _text.text = $"Connect your mqttclient to {ip.ToString()} and publish \na color like #50aa7f to topic /presentation/background";
        }
    }
    private IPAddress MyIP()
    {
        try
        {
            IPHostEntry host = Dns.GetHostEntry(Dns.GetHostName());

            return host.AddressList.FirstOrDefault(o => o.AddressFamily == AddressFamily.InterNetwork);    
        }
        catch
        {
            return null;
        }
        
    }
}
