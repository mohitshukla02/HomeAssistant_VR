using System;
using System.Collections;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Client.Options;
using UnityEngine;
using UnityEngine.Events;

namespace SmoothMQTT.Core
{
    [HelpURL("https://smoothmqtt.schliesky.com/docs/next/user-guide/core-components#subscriber-component")]
    public class Subscriber : MonoBehaviour
    {
        public string topic;
        public StringEvent action;
        public string clientID;
        private MqttClient client;
        bool _isReconnecting = false;
        private float _reconnectionTimer = 1f;

        private void Awake()
        {
            Settings.OnConnect += StartConnect;
            Settings.OnReconnect += TryTriggerReconnect;
        }

        void StartConnect()
        {
            StartCoroutine(ConnectSubscriber());
        }
        
        private IEnumerator ConnectSubscriber()
        {
            if (clientID == null || clientID.Equals(string.Empty))
            {
                throw new ArgumentException("Value may not be null or empty", nameof(clientID));
            }

            if (action != null)
            {
                client = (MqttClient)Settings.mqttFactory.CreateMqttClient();
                var options = new MqttClientOptionsBuilder()
                    .WithClientId(clientID)
                    .WithTcpServer(Settings.Instance.host, Settings.Instance.port)
                    .WithCleanSession();
                if (!string.IsNullOrEmpty(Settings.Instance.user) || !string.IsNullOrEmpty(Settings.Instance.password))
                {
                    options = options.WithCredentials(Settings.Instance.user, Settings.Instance.password);
                }

                if (Settings.Instance.useSSL)
                {
                    options = options.WithTls(new MqttClientOptionsBuilderTlsParameters
                    {
                        Certificates = new[] { Settings.Instance.clientCertificate },
                        UseTls = true,
                        SslProtocol = SslProtocols.Tls12,
                        IgnoreCertificateChainErrors = true,
                        IgnoreCertificateRevocationErrors = true,
                        AllowUntrustedCertificates = true,
                        CertificateValidationHandler = context =>
                        {
                            context.Chain.ChainPolicy.VerificationFlags = Settings.Instance.allowUnknownCA
                                ? X509VerificationFlags.AllowUnknownCertificateAuthority
                                : X509VerificationFlags.NoFlag;

                            return true;
                        }
                    });
                }

                if (Settings.Instance.autoReconnect)
                {
                    client.UseDisconnectedHandler(args => { TryTriggerReconnect(); });    
                }
                

                client.UseApplicationMessageReceivedHandler(e =>
                {
                    if (Settings.Instance.debug)
                    {
                        Debug.Log("Message received");
                    }

                    var qe = new Queue.MqttQueuedEvent
                    {
                        Action = action,
                        Payload = System.Text.Encoding.UTF8.GetString(e.ApplicationMessage.Payload)
                    };
                    Queue.Instance.eventQueue.Enqueue(qe);
                });
                int tries = 3;
                float timeout = 1f;
                var ctsource = new CancellationTokenSource();
                _ = client.ConnectAsync(options.Build(), ctsource.Token);
                client.UseConnectedHandler((args =>
                {
                    _ = client.SubscribeAsync(new MqttTopicFilter
                        { Topic = topic, QualityOfServiceLevel = Settings.Instance.QOSLevel });
                    if(_isReconnecting){
                        _isReconnecting = false;
                        Debug.Log("Reconnected to MQTT Broker");
                    }
                }));
                while (!client.IsConnected)
                {
                    if (tries <= 0)
                    {
                        Debug.LogError($"Cannot connect subscriber {name}.");
                        yield break;
                    }

                    timeout -= Time.deltaTime;
                    if (timeout <= 0)
                    {
                        ctsource.Cancel(false);
                        yield return null;
                        _ = client.ConnectAsync(options.Build(), ctsource.Token);
                        timeout = 1f;
                        tries--;
                    }

                    yield return null;
                }

                
            }
        }

        void TryTriggerReconnect()
        {
            if (!_isReconnecting)
            {
                _isReconnecting = true;
                _reconnectionTimer = client.Options.CommunicationTimeout.Seconds;
                if (Settings.Instance.debug)
                {
                    Debug.Log("Trying to reconnect");
                }

                if (client.IsConnected)
                {
                    _ = client.ReconnectAsync();    
                }
                else
                {
                    StartConnect();
                }
            }
        }

        private void Update()
        {
            if (_isReconnecting)
            {
                _reconnectionTimer -= Time.deltaTime;
                if (_reconnectionTimer <= 0)
                {
                    Debug.LogWarning($"Reconnection timed out for subscriber {clientID}. Trying again.");
                    _isReconnecting = false;
                    if (!client.IsConnected && Settings.Instance.autoReconnect)
                    {
                        TryTriggerReconnect();
                    }
                }
            }
        }

        private void OnEnable()
        {
            action.AddListener(DoNothing);
        }

        private void OnDisable()
        {
            action.RemoveListener(DoNothing);
        }

        void DoNothing(string s)
        {
            return;
        }
        public void ForceReconnect()
        {
            if (client.IsConnected)
            {
                client.UseDisconnectedHandler(args =>
                {
                    client.UseDisconnectedHandler(args2 => { });
                    StartConnect();
                });
                client.DisconnectAsync();
            }
            else
            {
                StartConnect();
            }
            
        }
    }

    [Serializable]
    public class StringEvent : UnityEvent<string>
    {
    }
}