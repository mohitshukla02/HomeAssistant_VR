using System;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Events;


namespace SmoothMQTT.Subscribing
{
    [HelpURL("https://smoothmqtt.schliesky.com/docs/user-guide/converter#universal-converter")]
    public class UniversalConverter : MonoBehaviour
    {
        [SerializeField, HideInInspector] public bool _registerWithSubscriber;
        
        public enum TargetType
        {
            None,
            Float,
            Bool,
            Int,
            Vector3,
            Quaternion,
            Color
        }

        [SerializeField] public TargetType type;

        public FloatEvent floatAction;
        public BoolEvent boolAction;
        public IntEvent intAction;
        public Vector3Event vector3Action;
        public QuaternionEvent quaternionAction;
        public ColorEvent colorAction;

    public void RunAction(string payload)
        {
            switch (type)
            {
                case TargetType.None:
                    return;
                case TargetType.Float:
                    InvokeFloat(payload);
                    break;
                case TargetType.Bool:
                    InvokeBool(payload);
                    break;
                case TargetType.Int:
                    InvokeInt(payload);
                    break;
                case TargetType.Vector3:
                    InvokeVector3(payload);
                    break;
                case TargetType.Quaternion:
                    InvokeQuaternion(payload);
                    break;
                case TargetType.Color:
                    InvokeColor(payload);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void InvokeInt(string payload)
        {
            try
            {
                var value = int.Parse(payload);
                intAction?.Invoke(value);
            }
            catch
            {
                throw new InvalidDataException($"Payload \"{payload}\" could not be parsed to int!");
            }
        }

        private void InvokeBool(string payload)
        {
            if (bool.TryParse(payload, out var bvalue))
            {
                boolAction?.Invoke(bvalue);
            }
            else
            {
                throw new InvalidDataException($"Payload \"{payload}\" could not be parsed to bool!");
            }
        }

        private void InvokeFloat(string payload)
        {
            floatAction?.Invoke(payload.ToFloat());
        }

        private void InvokeQuaternion(string payload)
        {
            var match = Regex.Split(payload, @",\s");
            //var match = Regex.Match(payload, @"\(([0-9,.]+), ([0-9,.]+), ([0-9,.]+)\)");
            //var data = payload.Trim(trimchars).Split(',');
            var data = new string[match.Length];
            for(int i = 0; i< data.Length; ++i)
            {
                var s = match[i];
                s = s.Trim(new char[] { '(', ')' });
                s = s.Replace(",", ".");
                data[i] = s;
            }

            if (match.Length == 4)
            {
                var x = data[0].ToFloat();
                var y = data[1].ToFloat();
                var z = data[2].ToFloat();
                var w = data[3].ToFloat();
                quaternionAction?.Invoke(new Quaternion(x, y, z, w));

            }
            else
            {
                throw new InvalidDataException($"Payload \"{payload}\" could not be parsed to Quaternion!");
            }
        }

        private void InvokeVector3(string payload)
        {
            var match = Regex.Split(payload, @",\s");
            var data = new string[match.Length];
            for(int i = 0; i< data.Length; ++i)
            {
                var s = match[i];
                s = s.Trim(new char[] { '(', ')' });
                s = s.Replace(",", ".");
                data[i] = s;
            }

            if (match.Length == 3)
            {
                var x = data[0].ToFloat();
                var y = data[1].ToFloat();
                var z = data[2].ToFloat();
                vector3Action?.Invoke(new Vector3(x, y, z));
            }
            else
            {
                throw new InvalidDataException($"Payload \"{payload}\" could not be parsed to Vector3!");
            }
        }

        private void InvokeColor(string payload)
        {
            if (payload.StartsWith("#"))
            {
                var r = int.Parse(payload.Substring(1, 2),NumberStyles.HexNumber);
                var g = int.Parse(payload.Substring(3, 2),NumberStyles.HexNumber);
                var b = int.Parse(payload.Substring(5, 2),NumberStyles.HexNumber);
                colorAction.Invoke(new Color(r/255f, g/255f, b/255f));
            }
        } 

#region events
        [Serializable]
        public class FloatEvent : UnityEvent<float>
        {
        }

        [Serializable]
        public class BoolEvent : UnityEvent<bool>
        {
        }

        [Serializable]
        public class IntEvent : UnityEvent<int>
        {
        }

        [Serializable]
        public class Vector3Event : UnityEvent<Vector3>
        {
        }

        [Serializable]
        public class QuaternionEvent : UnityEvent<Quaternion>
        {
            
        }

        [Serializable]
        public class ColorEvent : UnityEvent<Color>
        {
            
        }
#endregion

    }
    public static class StringConverters
    {
        public static float ToFloat(this string input)
        {
            if (float.TryParse(input, NumberStyles.Float, CultureInfo.InvariantCulture, out var value))
            {
                return value;
            }

            throw new InvalidDataException($"Cannot parse {input} as float");
        }

        public static bool RoughlyEquals(this Vector3 left, Vector3 right, float delta = 0.1f)
        {
            var result = true;
            result = result && Mathf.Abs(left.x - right.x) < delta;
            result = result && Mathf.Abs(left.y - right.y) < delta;
            result = result && Mathf.Abs(left.z - right.z) < delta;
            return result;
        }
        
        public static bool RoughlyEquals(this Quaternion left, Quaternion right, float delta = 0.1f)
        {
            var result = true;
            result = result && Mathf.Abs(left.x - right.x) < delta;
            result = result && Mathf.Abs(left.y - right.y) < delta;
            result = result && Mathf.Abs(left.z - right.z) < delta;
            result = result && Mathf.Abs(left.w - right.w) < delta;
            return result;
        }
    }
}