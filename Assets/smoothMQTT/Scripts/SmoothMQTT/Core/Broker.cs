using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using MQTTnet.Diagnostics;
using MQTTnet.Protocol;
using MQTTnet.Server;
using SmoothMQTT.Util;
using UnityEngine;

namespace SmoothMQTT.Core
{
    [HelpURL("https://smoothmqtt.schliesky.com/docs/next/user-guide/core-components#broker")]
    public class Broker : Singleton<Broker>
    {
        private IMqttServer broker;

        public List<ScriptableCredentials> credentials;
        private void Awake()
        {
            Settings.OnConnect += StartBroker;
        }
        
        private void OnDestroy()
        {
            if (broker != null && broker.IsStarted)
            {
                broker.StopAsync();
            }
        }

        private void StartBroker()
        {
            MqttNetLogger logger = null;
            if (Settings.Instance.debug)
            {
                logger = new MqttNetLogger("debug_logger");
                logger.LogMessagePublished += (s, e) =>
                {
                    if (e.LogMessage.Level == MqttNetLogLevel.Verbose)
                    {
                        return;
                    }
                    var trace =
                        $">> [{e.LogMessage.Timestamp:O}] [{e.LogMessage.ThreadId}] [{e.LogMessage.Source}] [{e.LogMessage.Level}]: {e.LogMessage.Message}";
                    if (e.LogMessage.Exception != null)
                    {
                        trace += Environment.NewLine + e.LogMessage.Exception.ToString();
                    }

                    if (e.LogMessage.Level == MqttNetLogLevel.Error)
                    {
                        Debug.LogError(trace);
                    }else if (e.LogMessage.Level == MqttNetLogLevel.Warning)
                    {
                        Debug.LogWarning(trace);    
                    }
                    
                };
            }

            var optionsBuilder = new MqttServerOptionsBuilder();

            if (Settings.Instance.validateCredentials)
            {
                optionsBuilder = optionsBuilder
                        .WithConnectionValidator(connectionContext =>
                        {
                            foreach (var userpw in credentials)
                            {
                                if (userpw.username == connectionContext.Username && userpw.password == connectionContext.Password)
                                {
                                    connectionContext.ReasonCode = MqttConnectReasonCode.Success;
                                    return;
                                }

                                connectionContext.ReasonCode = MqttConnectReasonCode.BadUserNameOrPassword;
                                return;
                            }

                            connectionContext.ReasonCode = MqttConnectReasonCode.NotAuthorized;
                        });
            }

            if (Settings.Instance.useSSL)
            {
                if (Settings.Instance.brokerCertAsset?.GetRelativePath() != null)
                {
                    var certPath = Application.streamingAssetsPath;
                    optionsBuilder = optionsBuilder
                        .WithoutDefaultEndpoint();
                    var serverCert = new X509Certificate2(
                        Path.Combine(certPath, Settings.Instance.brokerCertAsset.GetRelativePath()),
                        Settings.Instance.brokerCertAsset.password,
                        X509KeyStorageFlags.Exportable);

                    if (serverCert.HasPrivateKey)
                    {
                        optionsBuilder = optionsBuilder.WithEncryptionCertificate(
                                serverCert.Export(X509ContentType.Pkcs12, Settings.Instance.brokerCertAsset.password),
                                new MqttServerCertificateCredentials()
                                    {Password = Settings.Instance.brokerCertAsset.password})
                            .WithEncryptedEndpoint()
                            .WithEncryptionSslProtocol(SslProtocols.Tls12)
                            .WithEncryptedEndpointBoundIPAddress(IPAddress.Parse(Settings.Instance.host));
                        ;
                        Settings.Instance.serverCertificate = serverCert;
                    }

                    optionsBuilder = optionsBuilder.WithClientCertificate(
                        (sender, certificate, chain, sslPolicyErrors) =>
                        {

                            // TODO: Add client certificate validation
                            return true;
                        });
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                    ServicePointManager.ServerCertificateValidationCallback += (sender, certificate, chain, errors) =>
                    {
                        //TODO: Certificate Validation as soon as Mono supports it.
                        return true;
                    };
                }
                else
                {
                    Debug.LogError("Improperly configured path for broker/server cert.");
                    optionsBuilder.WithDefaultEndpoint();
                }
            }

            if (Settings.Instance.debug)
            {
                optionsBuilder.WithApplicationMessageInterceptor(context =>
                {
                    Debug.Log(
                        $"Intercepted {context.ApplicationMessage.Topic}: {Encoding.UTF8.GetString(context.ApplicationMessage.Payload)}");
                });
                broker = Settings.mqttFactory.CreateMqttServer(logger);
            }
            else
            {
                broker = Settings.mqttFactory.CreateMqttServer();
            }
            broker.StartedHandler = new MqttServerStartedHandlerDelegate((args) =>
            {
                return;
            });
            broker.StartAsync(optionsBuilder.Build());
        }
    }
}