using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using MQTTnet;
using MQTTnet.Client;
using UnityEngine;
using UnityEngine.Events;
using MQTTnet.Client.Options;
using MQTTnet.Protocol;
using SmoothMQTT.Util;

namespace SmoothMQTT.Core
{
    [HelpURL("https://smoothmqtt.schliesky.com/docs/next/user-guide/core-components#settings")]
    public class Settings : Singleton<Settings>
    {
        [Header("Connection Parameters")] public string host = "localhost";
        public int port = 1883;
        public string user;
        public string password;
        public bool connectOnStart = true;

        [Tooltip("Auto reconnect components if connection is lost")]
        public bool autoReconnect = true;
        public bool debug;

        public MqttQualityOfServiceLevel QOSLevel = (MqttQualityOfServiceLevel)1;

        [Header("For use with internal broker only")]
        public bool validateCredentials;

        [Header("SSL certificates")] public bool useSSL = false; // Will be available in future version

        public ScriptableCertificate caCertAsset;
        public ScriptableCertificate brokerCertAsset;
        public ScriptableCertificate clientCertAsset;

        public Queue<UnityEvent> queue;
        public static MqttFactory mqttFactory = new MqttFactory();
        public List<X509Certificate2> publicCertChain;
        public X509Certificate2 serverCertificate;
        public X509Certificate2 clientCertificate;
        public bool allowUnknownCA;

        public static event Action OnConnect;
        public static event Action OnReconnect;

        void Awake()
        {
            if (host.ToLower().Equals("localhost"))
            {
                host = "127.0.0.1";
            }

            host = host.Trim();
            queue = new Queue<UnityEvent>();
            if (useSSL)
            {
                var certPath = Application.streamingAssetsPath;
                if (caCertAsset?.GetRelativePath() != null)
                {
                    var ca = new X509Certificate2(
                        Path.Combine(certPath, caCertAsset.GetRelativePath()), caCertAsset.password);
                    publicCertChain = new List<X509Certificate2>();
                    publicCertChain.Add(serverCertificate);
                    publicCertChain.Add(ca);
                }
                else
                {
                    Debug.LogError($"Improperly configured path for CA cert.");
                }

                if (clientCertAsset?.GetRelativePath() != null)
                {
                    clientCertificate = new X509Certificate2(Path.Combine(certPath, clientCertAsset.GetRelativePath()),
                        clientCertAsset.password);
                }
                else
                {
                    Debug.LogError($"Improperly configured path for client cert.");
                }
            }
        }

        void Start()
        {
            if (debug)
            {
                OnConnect += StartDebugClient;
            }
            if (connectOnStart)
            {
                OnConnect?.Invoke();
            }
        }

        void Update()
        {
            lock (queue)
            {
                while (queue.Count > 0)
                {
                    UnityEvent e = queue.Dequeue();
                    Debug.Log($"{e.GetPersistentEventCount()} listeners: {e.GetPersistentMethodName(1)}");
                    e.Invoke();
                }
            }
        }

        private void StartDebugClient()
        {
            var options = new MqttClientOptionsBuilder()
                .WithClientId("DebugClient")
                .WithTcpServer(host, port)
                .WithCleanSession();
            if (useSSL)
            {
                options = options.WithTls(new MqttClientOptionsBuilderTlsParameters
                {
                    Certificates = publicCertChain,
                    UseTls = true,
                    SslProtocol = SslProtocols.Tls12,
                    IgnoreCertificateChainErrors = true,
                    IgnoreCertificateRevocationErrors = true,
                    AllowUntrustedCertificates = true,
                    CertificateValidationHandler = (context => true)
                });
            }

            var mqttClient = mqttFactory.CreateMqttClient();
            mqttClient.UseConnectedHandler(async e =>
            {
                await mqttClient.SubscribeAsync(new MqttTopicFilter() { Topic = "#" });
                Debug.Log("Debug Client connected");
            });
            mqttClient.UseApplicationMessageReceivedHandler(e =>
            {
                Debug.Log(
                    $"{e.ApplicationMessage.Topic}: {Encoding.UTF8.GetString(e.ApplicationMessage.Payload)}");
            });

            mqttClient.ConnectAsync(options.Build(), CancellationToken.None);
            Debug.Log("Debug Client started");
        }
        
        public void Connect()
        {
            OnConnect?.Invoke();
        }
        
        public void Reconnect()
        {
            OnReconnect?.Invoke();
        }
    }
}