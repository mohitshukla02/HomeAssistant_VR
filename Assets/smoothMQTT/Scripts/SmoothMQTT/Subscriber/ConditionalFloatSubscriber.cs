using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using SmoothMQTT.Core;
using UnityEngine;
using UnityEngine.Events;

namespace SmoothMQTT.Subscribing
{
    [DisallowMultipleComponent]
    [HelpURL("https://smoothmqtt.schliesky.com/docs/next/user-guide/converter#compare-float-conditionalfloatsubscriber")]
    public class ConditionalFloatSubscriber : MonoBehaviour
    {
        #if UNITY_2019_4_OR_NEWER
        public enum ComparisonType
        {
            [InspectorName("= (equal)")]Equal,
            [InspectorName("!= (not equal)")]NotEqual,
            [InspectorName("> (greater)")]GreaterThan,
            [InspectorName(">= (greater or equal)")]GreaterThanOrEqual,
            [InspectorName("<= (less or equal)")]LessThanOrEqual,
            [InspectorName("< (less)")]LessThan,
            [InspectorName("in range(a, b) >=a and <b")]WithinRange,
            [InspectorName("not in range(a, b) <a or >= b")]OutsideRange
        }
        #else
        public enum ComparisonType
        {
            Equal,
            NotEqual,
            GreaterThan,
            GreaterThanOrEqual,
            LessThanOrEqual,
            LessThan,
            WithinRange,
            OutsideRange
        }
        #endif
        [Header("Condition")]
        [Tooltip("WithinRange and OutsideRange are incl. lower and excl. upper limit.")]
        public ComparisonType comparisonOperator;

        public float compareValue;
        public float additionalRangeCompareValue;

        [Header("Actions")] 
        public FloatEvent trueAction;
        public FloatEvent falseAction;

        public void OnAction(string p)
        {
            float payload;
            try
            {
                payload = float.Parse(p, NumberStyles.Float, CultureInfo.InvariantCulture);
            }
            catch (FormatException)
            {
                throw new FormatException("Skipping because of ill-formatted payload," +
                                          " make sure the payload uses a dot as decimal separator. ");
            }
            
            bool useTrueAction = false;
            switch (comparisonOperator)
            {
                case ComparisonType.Equal:
                    useTrueAction = Math.Abs(payload - compareValue) < Mathf.Epsilon; 
                    break;
                case ComparisonType.NotEqual:
                    useTrueAction = !(Math.Abs(payload - compareValue) < Mathf.Epsilon); 
                    break;
                case ComparisonType.GreaterThan:
                    useTrueAction = payload > compareValue;
                    break;
                case ComparisonType.GreaterThanOrEqual:
                    useTrueAction = payload >= compareValue;
                    break;
                case ComparisonType.LessThanOrEqual:
                    useTrueAction = payload <= compareValue;
                    break;
                case ComparisonType.LessThan:
                    useTrueAction = payload < compareValue;
                    break;
                case ComparisonType.WithinRange:
                    useTrueAction = true;
                    useTrueAction &= payload >= Mathf.Min(compareValue, additionalRangeCompareValue);
                    useTrueAction &= payload < Mathf.Max(compareValue, additionalRangeCompareValue);
                    break;
                case ComparisonType.OutsideRange:
                    useTrueAction = true;
                    useTrueAction &= payload < Mathf.Min(compareValue, additionalRangeCompareValue)
                                  || payload >= Mathf.Max(compareValue, additionalRangeCompareValue);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            if (useTrueAction)
            {
                trueAction?.Invoke(payload);
            }
            else
            {
                falseAction?.Invoke(payload);
            }
        }
    }
}