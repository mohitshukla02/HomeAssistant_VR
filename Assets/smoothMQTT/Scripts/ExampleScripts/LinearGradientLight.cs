using System.Collections.Generic;
using UnityEngine;

namespace ExampleScripts
{
    /// <summary>
    /// Helper script to illustrate setting of color on a list of objects (lights in this case) as reaction to a string payload
    /// </summary>
    public class LinearGradientLight : MonoBehaviour
    {
        [Header("Expected value range")]
        public float minValue;
        public float maxValue;
        [Space]
        [Tooltip("ColorGradient to map value range on")]
        public Gradient colorGradient;
        public List<Light> lights;
    
        public void OnSetLightColor(string payload)
        {
            // Convert MQTT payload to float and remap from [min, max] to [0, 1]
            var value = float.Parse(payload);
            value = (value - minValue) / (maxValue - minValue);
            value = Mathf.Clamp01(value);
        
            foreach (var l in lights)
            {
                l.color = colorGradient.Evaluate(value);
            }
        }
    }
}
