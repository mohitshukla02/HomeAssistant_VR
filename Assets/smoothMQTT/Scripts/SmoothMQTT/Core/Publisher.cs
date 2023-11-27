using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;
using MQTTnet.Client;
using MQTTnet.Client.Connecting;
using MQTTnet.Client.Options;
using UnityEngine;


namespace SmoothMQTT.Core
{
    /// <summary>
    /// Sender provides an autoconnecting MQTT client responsible for publishing all messages needed.
    /// </summary>
    [HelpURL("https://smoothmqtt.schliesky.com/docs/next/user-guide/core-components#publisher")]
    public class Publisher : MonoBehaviour
    {
        public static Publisher Instance;
        [Tooltip("Timeout in seconds")] public float connectionTimeout = 3f;
        public string clientId;
        private MqttClient _client;
        private bool _isReconnecting = false;
        private float _reconnectionTimer = 1f;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }

            Settings.OnConnect += StartConnect;
            Settings.OnReconnect += TryTriggerReconnect;
        }

        void StartConnect()
        {
            StartCoroutine(Connect());
        }

        /// <summary>
        /// Publish a message to the MQTT broker
        /// </summary>
        /// <param name="topic">Topic to publish on (usually something like "/group/unit"</param>
        /// <param name="payload">Data to publish on the specified topic. Needs to be String, but can be serialized value of any type</param>
        /// <param name="retain">Bool determining whether the message should stay published for Clients subscribing later to the topic.</param>
        /// <returns>Task. Can be used for await statements or ignored by assigning to _</returns>
        /// <example>_ = OnSendMessage("/sensors/temp/living_room", "25.7", false);</example>
        public async Task OnSendMessage(string topic, string payload, bool retain = false)
        {
            if (!_client.IsConnected)
            {
                Debug.LogWarning("Tried to send MQTT message, but client is not connected");
                return;
            }

            if (Settings.Instance.debug)
            {
                Debug.Log($"Publish message \"{payload}\" to topic {topic}{(retain ? " (retained)" : "")}");
            }

            await _client.PublishAsync(topic, payload, retain);
        }

        void TryTriggerReconnect()
        {
            if (!_isReconnecting)
            {
                _isReconnecting = true;
                _reconnectionTimer = _client.Options.CommunicationTimeout.Seconds;
                if (Settings.Instance.debug)
                {
                    Debug.Log("Trying to reconnect");
                }

                if (_client.IsConnected)
                {
                    _ = _client.ReconnectAsync();
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
                    Debug.LogWarning($"Reconnection timed out for publisher {clientId}. Trying again.");
                    _isReconnecting = false;
                    if (!_client.IsConnected && Settings.Instance.autoReconnect)
                    {
                        TryTriggerReconnect();
                    }
                }
            }
        }

        IEnumerator Connect()
        {
            _client = (MqttClient)Settings.mqttFactory.CreateMqttClient();
            if (Settings.Instance.autoReconnect)
            {
                _client.UseDisconnectedHandler(args => { TryTriggerReconnect(); });
            }

            var connectionTime = 0f;
            var options = new MqttClientOptionsBuilder()
                    .WithClientId(clientId)
                    .WithTcpServer(Settings.Instance.host, Settings.Instance.port)
                    .WithCleanSession()
                    .WithCommunicationTimeout(TimeSpan.FromSeconds(connectionTimeout))
                ;
            if (!string.IsNullOrEmpty(Settings.Instance.user) || !string.IsNullOrEmpty(Settings.Instance.password))
            {
                options = options.WithCredentials(Settings.Instance.user, Settings.Instance.password);
            }

            if (Settings.Instance.useSSL)
            {
                options = options.WithTls(
                    new MqttClientOptionsBuilderTlsParameters
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

            var ctsource = new CancellationTokenSource();

            var result = _client.ConnectAsync(options.Build(), ctsource.Token);
            _client.UseConnectedHandler((args) =>
            {
                if (Settings.Instance.debug)
                {
                    _client.SubscribeAsync("$SYS/");
                    Debug.Log($"Connected to MQTT broker {Settings.Instance.host}:{Settings.Instance.port}");
                }
            });
            while (!result.IsCompleted)
            {
                connectionTime += Time.deltaTime;
                if (connectionTime >= connectionTimeout)
                {
                    ctsource.Cancel(false);
                    if (Settings.Instance.debug)
                    {
                        Debug.LogWarning("Connection timed out.", gameObject);
                    }
                    yield break;
                }

                yield return null;
            }

            if (result.Result.ResultCode == MqttClientConnectResultCode.BadUserNameOrPassword)
            {
                Debug.LogError("Sender cannot connect. Invalid username or password.");
                yield break;
            }

            if (result.Result.ResultCode == MqttClientConnectResultCode.NotAuthorized)
            {
                Debug.LogError("Sender cannot connect. User is not authorized.");
                yield break;
            }

            connectionTime += Time.deltaTime;
            if (connectionTime >= connectionTimeout)
            {
                Debug.LogError($"Connection timed out after {connectionTime:F1} seconds.");
                yield break;
            }

            yield return null;
        }

        public void ForceReconnect()
        {
            if (_client.IsConnected)
            {
                _client.UseDisconnectedHandler(args =>
                {
                    _client.UseDisconnectedHandler(args2 => { });
                    StartConnect();
                });
                _client.DisconnectAsync();
            }
            else
            {
                StartConnect();
            }
            
        }
    }
}