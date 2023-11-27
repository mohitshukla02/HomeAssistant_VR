using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using SmoothMQTT.Core;
using UnityEngine;

namespace SmoothMQTT.Sending
{
    [HelpURL("https://smoothmqtt.schliesky.com/docs/next/user-guide/sending_publishing#publishtriggercollision")]
    [RequireComponent(typeof(Collider))]
    public class PublishTriggerCollision : MonoBehaviour
    {
        [Header("MQTT")]
        public string topic;
        [Tooltip("If true, %t, %n, and %a in topic string will be replaced by type (trigger/collision), name(gameobject's name), and action (enter/stay/exit) respectively")]
        public bool formatTopicString;
        [Tooltip("Should usually resemble the collider setting")]
        public bool isTrigger;

        [Header("Source and Target")]
        public Collider sourceCollider;
        public GameObject filterTargetGameObject;
        public String filterTargetTag;

        [Header("Actions to Publish")] 
        public bool publishEnter = true;
        public bool publishStay = false;
        public bool publishExit = true;

        [Header("OnStay events (Consult manual before use)")]
        public bool emitMQTTOnStay;

        [Tooltip("Interval in seconds between two MQTT messages send during OnCollision- or OnTriggerStay")]
        public float emitInterval = 0.5f;

        private float _emitTimer = 0;
        enum CollisionType
        {
            None,
            Enter,
            Stay,
            Exit
        }

        private CollisionType _collisionType;

        #region Trigger

        private void Reset()
        {
            if (sourceCollider == null) sourceCollider = GetComponent<Collider>();
        }
        private void OnTriggerEnter(Collider other)
        {
            if (!isTrigger) return;
            if (sourceCollider.isTrigger || other.isTrigger) Enter(other.gameObject);
        }

        private void OnTriggerExit(Collider other)
        {
            if (!isTrigger) return;
            if(sourceCollider.isTrigger || other.isTrigger) Exit(other.gameObject);
        }

        private void OnTriggerStay(Collider other)
        {
            if (!isTrigger) return;
            if ((sourceCollider.isTrigger || other.isTrigger) && emitMQTTOnStay) Stay(other.gameObject);
        }

        #endregion

        
        #region Collision

        private void OnCollisionEnter(Collision other)
        {
            if (isTrigger) return;
            if (!sourceCollider.isTrigger) Enter(other.gameObject);
        }

        private void OnCollisionExit(Collision other)
        {
            if (isTrigger) return;
            if (!sourceCollider.isTrigger) Exit(other.gameObject);
        }

        private void OnCollisionStay(Collision other)
        {
            if (isTrigger) return;
            if (!sourceCollider.isTrigger && emitMQTTOnStay) Stay(other.gameObject);
        }

        #endregion


        void Enter(GameObject other)
        {
            if (!publishEnter) return;
            
            bool validTarget = IsValidTarget(other);
            if (validTarget)
            {
                _collisionType = CollisionType.Enter;
                OnPublishTriggerCollision(other);
                _emitTimer = Time.time;
            }
        }

        void Stay(GameObject other)
        {
            if (!publishStay) return;
            
            bool validTarget = IsValidTarget(other.gameObject);
            if (validTarget && Time.time >= _emitTimer + emitInterval)
            {
                _collisionType = CollisionType.Stay;
                OnPublishTriggerCollision(other.gameObject);
                _emitTimer = Time.time;
            }
        }

        void Exit(GameObject other)
        {
            if (!publishExit) return;

            bool validTarget = IsValidTarget(other.gameObject);
            if (validTarget)
            {
                _collisionType = CollisionType.Exit;
                OnPublishTriggerCollision(other.gameObject);
            }
        }

        bool IsValidTarget(GameObject other)
        {
            var validTarget = true;
            if (filterTargetGameObject != null)
            {
                validTarget &= filterTargetGameObject == other;
            }

            if (!filterTargetTag.Equals(""))
            {
                validTarget &= other.CompareTag(filterTargetTag);
            }

            return validTarget;
        }

        public void OnPublishTriggerCollision(GameObject other)
        {
            var type = sourceCollider.isTrigger ? "Trigger" : "Collision";
            var currentTopic = topic;
            if (formatTopicString)
            {
                var topicReplacementParticlesAvailable = new bool [3];
                topicReplacementParticlesAvailable[0] = currentTopic.Contains("%t");
                topicReplacementParticlesAvailable[1] = currentTopic.Contains("%a");
                topicReplacementParticlesAvailable[2] = currentTopic.Contains("%n");
                currentTopic = currentTopic.Replace("%t", type.ToLower());
                currentTopic = currentTopic.Replace("%a", _collisionType.ToString().ToLower());
                currentTopic = currentTopic.Replace("%n", name.ToLower());
                var payload = "";
                payload += topicReplacementParticlesAvailable[0] ? "" : type + " ";
                payload += topicReplacementParticlesAvailable[2] ? "" : "\"" + name + "\" ";
                payload += topicReplacementParticlesAvailable[1] ? "" : _collisionType.ToString() + " ";

                _ = Publisher.Instance.OnSendMessage(currentTopic, $"{payload}\"{other.name}\"");
                return;
            }
            _ = Publisher.Instance.OnSendMessage(topic, $"{type} \"{name}\" {_collisionType.ToString()} \"{other.name}\"");
        }
    }
}