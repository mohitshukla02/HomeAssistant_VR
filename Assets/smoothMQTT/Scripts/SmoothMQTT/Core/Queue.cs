using System;
using System.Collections.Generic;
using UnityEngine;

namespace SmoothMQTT.Core
{
    
    /// <summary>
    /// The Queue stores all triggered actions and executes them as soon as possible
    /// <typeparam name="maxActionsPerFrame"></typeparam>
    /// </summary>
    [HelpURL("https://smoothmqtt.schliesky.com/docs/next/user-guide/core-components#queue")]
    public class Queue : MonoBehaviour
    {
        public static Queue Instance;
        public int maxActionsPerFrame = 10;
        public struct MqttQueuedEvent
        {
            public StringEvent Action;
            public string Payload;
        }

        public Queue<MqttQueuedEvent> eventQueue;


        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            eventQueue = new Queue<MqttQueuedEvent>();
        }
        
        void Update()
        {
            lock (eventQueue)
            {
                int i = 0;
                while (eventQueue.Count > 0 && i < maxActionsPerFrame)
                {
                    MqttQueuedEvent qe = eventQueue.Dequeue();
                    if(Settings.Instance.debug)
                    {
                        try
                        {
                            Debug.Log($"Message {qe.Payload} received invoking action on {qe.Action.GetPersistentEventCount()} persistent listeners (and potentially runtime as well).");
                        }
                        catch
                        {
                            Debug.Log($"Message {qe.Payload} received invoking action for runtime listeners only.");
                        }
                    }

                    try
                    {
                        qe.Action.Invoke(qe.Payload);
                    }
                    catch (NullReferenceException)
                    {
                        Debug.LogWarning($"No Action defined. Skipping...");
                    }
                    i++;
                }
            }
        }
    }
}