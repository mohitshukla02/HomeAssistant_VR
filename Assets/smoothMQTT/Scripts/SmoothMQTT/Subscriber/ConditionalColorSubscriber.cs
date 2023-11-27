using SmoothMQTT.Core;
using UnityEngine;
using UnityEngine.Events;

namespace SmoothMQTT.Subscribing
{
    public class ConditionalColorSubscriber : MonoBehaviour
    {
        [Header("Condition")] public Gradient gradient;
        public float threshold = 0.01f;

        [Header("Actions")] public ColorEvent trueAction;
        public ColorEvent falseAction;

        public void OnAction(string payload)
        {
            Color payloadColor;

            if (!ColorUtility.TryParseHtmlString(payload, out payloadColor))
            {
                if (Settings.Instance.debug)
                {
                    Debug.LogWarning("Could not parse color from payload: " + payload + ". Skipping.");
                }

                return;
            }

            var payloadColorVector = new Vector4(payloadColor.r, payloadColor.g, payloadColor.b, payloadColor.a);
            
            Debug.Log("Payload color vector: " + payloadColorVector);
            var numberOfKeyPairs = gradient.colorKeys.Length - 1;
            for (int i = 0; i < numberOfKeyPairs; i++)
            {
                var segmentStart = gradient.colorKeys[i];
                var segmentEnd = gradient.colorKeys[i + 1];
                if(segmentEnd.color == segmentStart.color) continue;
                var endColor = segmentEnd.color;
                var startColor = segmentStart.color;
                var lineEnd = new Vector4(endColor.r, endColor.g, endColor.b, gradient.alphaKeys[i+1].alpha);
                var lineStart = new Vector4(startColor.r, startColor.g, startColor.b, gradient.alphaKeys[i].alpha);
                var pa = lineStart - payloadColorVector;
                var pb = lineEnd - payloadColorVector;
                var cross = Vector3.Cross(pa, pb);
                var distance = cross.magnitude / (lineEnd - lineStart).magnitude;
                if (distance < threshold)
                {
                    trueAction.Invoke(payloadColor);
                    return;
                }
            }
            falseAction.Invoke(payloadColor);
        }
    }
}