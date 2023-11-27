using System;
using NUnit.Framework;
using UnityEngine;
using SmoothMQTT.Subscribing;
using SmoothMQTT.Core;

namespace Tests
{
    public class ConditionalTests
    {
        private GameObject _gameObject;
        private ConditionalSubscriber _stringSubscriber;
        private ConditionalFloatSubscriber _floatSubscriber;
        private ConditionalColorSubscriber _colorSubscriber;

        [SetUp]
        public void Setup()
        {
            _gameObject = new GameObject();
            _stringSubscriber = _gameObject.AddComponent<ConditionalSubscriber>();
            _floatSubscriber = _gameObject.AddComponent<ConditionalFloatSubscriber>();
            _stringSubscriber.trueAction = new StringEvent();
            _stringSubscriber.falseAction = new StringEvent();
            _stringSubscriber.compareString = "test";

            _floatSubscriber.trueAction = new FloatEvent();
            _floatSubscriber.falseAction = new FloatEvent();
            _floatSubscriber.compareValue = 5.0f;
            _floatSubscriber.additionalRangeCompareValue = 10.0f;

            _colorSubscriber = _gameObject.AddComponent<ConditionalColorSubscriber>();
            _colorSubscriber.trueAction = new ColorEvent();
            _colorSubscriber.falseAction = new ColorEvent();
            _colorSubscriber.gradient = new Gradient();
            var startColor = new GradientColorKey(Color.red, 0.0f);
            var endColor = new GradientColorKey(Color.blue, 1.0f);
            var startAlpha = new GradientAlphaKey(1.0f, 0.0f);
            var endAlpha = new GradientAlphaKey(1.0f, 1.0f);
            _colorSubscriber.gradient.SetKeys(new [] { startColor, endColor },
                new [] { startAlpha, endAlpha });
        }

        [TearDown]
        public void TearDown()
        {
        }

        [Test]
        public void TestStringCompareEquality()
        {
            string outcome = "";
            _stringSubscriber.trueAction.AddListener((payload) => { outcome = "True"; });
            _stringSubscriber.falseAction.AddListener((payload) => { outcome = "False"; });
            _stringSubscriber.OnAction("test");
            Assert.AreEqual("True", outcome);
            outcome = "";
            _stringSubscriber.OnAction("tEsT");
            Assert.AreEqual("False", outcome);
        }

        [Test]
        public void TestDecimalPoint()
        {
            _floatSubscriber.comparisonOperator = ConditionalFloatSubscriber.ComparisonType.Equal;
            Assert.Catch(typeof(FormatException), () => _floatSubscriber.OnAction("5,0"));
        }

        [Test]
        public void TestFloatCompareEquality()
        {
            string outcome = "";
            _floatSubscriber.trueAction.AddListener((payload) => { outcome = "True"; });
            _floatSubscriber.falseAction.AddListener((payload) => { outcome = "False"; });
            _floatSubscriber.comparisonOperator = ConditionalFloatSubscriber.ComparisonType.Equal;
            _floatSubscriber.OnAction("5.0");
            Assert.AreEqual("True", outcome);
            outcome = "";
            _floatSubscriber.comparisonOperator = ConditionalFloatSubscriber.ComparisonType.Equal;
            _floatSubscriber.OnAction("6.0");
            Assert.AreEqual("False", outcome);
            outcome = "";
            _floatSubscriber.comparisonOperator = ConditionalFloatSubscriber.ComparisonType.NotEqual;
            _floatSubscriber.OnAction("6.0");
            Assert.AreEqual("True", outcome);
        }

        [Test]
        public void TestBasicComparators()
        {
            string outcome = "";
            _floatSubscriber.trueAction.AddListener((payload) => { outcome = "True"; });
            _floatSubscriber.falseAction.AddListener((payload) => { outcome = "False"; });
            _floatSubscriber.comparisonOperator = ConditionalFloatSubscriber.ComparisonType.GreaterThan;
            _floatSubscriber.OnAction("6.0");
            Assert.AreEqual("True", outcome);
            outcome = "";
            _floatSubscriber.comparisonOperator = ConditionalFloatSubscriber.ComparisonType.GreaterThan;
            _floatSubscriber.OnAction("4.0");
            Assert.AreEqual("False", outcome);
            outcome = "";
            _floatSubscriber.comparisonOperator = ConditionalFloatSubscriber.ComparisonType.GreaterThan;
            _floatSubscriber.OnAction("5.0");
            Assert.AreEqual("False", outcome);

            outcome = "";
            _floatSubscriber.comparisonOperator = ConditionalFloatSubscriber.ComparisonType.LessThan;
            _floatSubscriber.OnAction("4.0");
            Assert.AreEqual("True", outcome);
            outcome = "";
            _floatSubscriber.comparisonOperator = ConditionalFloatSubscriber.ComparisonType.LessThan;
            _floatSubscriber.OnAction("6.0");
            Assert.AreEqual("False", outcome);
            outcome = "";
            _floatSubscriber.comparisonOperator = ConditionalFloatSubscriber.ComparisonType.LessThan;
            _floatSubscriber.OnAction("5.0");
            Assert.AreEqual("False", outcome);

            outcome = "";
            _floatSubscriber.comparisonOperator = ConditionalFloatSubscriber.ComparisonType.GreaterThanOrEqual;
            _floatSubscriber.OnAction("6.0");
            Assert.AreEqual("True", outcome);
            outcome = "";
            _floatSubscriber.comparisonOperator = ConditionalFloatSubscriber.ComparisonType.GreaterThanOrEqual;
            _floatSubscriber.OnAction("5.0");
            Assert.AreEqual("True", outcome);
            outcome = "";
            _floatSubscriber.comparisonOperator = ConditionalFloatSubscriber.ComparisonType.GreaterThanOrEqual;
            _floatSubscriber.OnAction("4.0");
            Assert.AreEqual("False", outcome);

            outcome = "";
            _floatSubscriber.comparisonOperator = ConditionalFloatSubscriber.ComparisonType.LessThanOrEqual;
            _floatSubscriber.OnAction("4.0");
            Assert.AreEqual("True", outcome);
            outcome = "";
            _floatSubscriber.comparisonOperator = ConditionalFloatSubscriber.ComparisonType.LessThanOrEqual;
            _floatSubscriber.OnAction("5.0");
            Assert.AreEqual("True", outcome);
            outcome = "";
            _floatSubscriber.comparisonOperator = ConditionalFloatSubscriber.ComparisonType.LessThanOrEqual;
            _floatSubscriber.OnAction("6.0");
            Assert.AreEqual("False", outcome);

        }

        [Test]
        public void TestRanges()
        {
            string outcome = "";
            _floatSubscriber.trueAction.AddListener((payload) => { outcome = "True"; });
            _floatSubscriber.falseAction.AddListener((payload) => { outcome = "False"; });
            _floatSubscriber.comparisonOperator = ConditionalFloatSubscriber.ComparisonType.WithinRange;
            _floatSubscriber.OnAction("6.0");
            Assert.AreEqual("True", outcome);
            outcome = "";
            _floatSubscriber.comparisonOperator = ConditionalFloatSubscriber.ComparisonType.WithinRange;
            _floatSubscriber.OnAction("4.0");
            Assert.AreEqual("False", outcome);
            outcome = "";
            _floatSubscriber.comparisonOperator = ConditionalFloatSubscriber.ComparisonType.WithinRange;
            _floatSubscriber.OnAction("5.0");
            Assert.AreEqual("True", outcome);
            outcome = "";
            _floatSubscriber.comparisonOperator = ConditionalFloatSubscriber.ComparisonType.WithinRange;
            _floatSubscriber.OnAction("10.0");
            Assert.AreEqual("False", outcome);
            outcome = "";


            _floatSubscriber.comparisonOperator = ConditionalFloatSubscriber.ComparisonType.OutsideRange;
            _floatSubscriber.OnAction("6.0");
            Assert.AreEqual("False", outcome);
            outcome = "";
            _floatSubscriber.comparisonOperator = ConditionalFloatSubscriber.ComparisonType.OutsideRange;
            _floatSubscriber.OnAction("4.0");
            Assert.AreEqual("True", outcome);
            outcome = "";
            _floatSubscriber.comparisonOperator = ConditionalFloatSubscriber.ComparisonType.OutsideRange;
            _floatSubscriber.OnAction("5.0");
            Assert.AreEqual("False", outcome);
            outcome = "";
            _floatSubscriber.comparisonOperator = ConditionalFloatSubscriber.ComparisonType.OutsideRange;
            _floatSubscriber.OnAction("10.0");
            Assert.AreEqual("True", outcome);
            outcome = "";
        }

        [Test]
        public void TestColorGradient()
        {
            var outcome = "";
            _colorSubscriber.trueAction.AddListener((payload) => { outcome = "True"; });
            _colorSubscriber.falseAction.AddListener((payload) => { outcome = "False"; });
            _colorSubscriber.OnAction("#FF0000");
            Assert.AreEqual("True", outcome);
            outcome = "";
            _colorSubscriber.OnAction("#00FF00");
            Assert.AreEqual("False", outcome);
            outcome = "";
            _colorSubscriber.OnAction("#0000FF");
            Assert.AreEqual("True", outcome);
            
            var testColor = _colorSubscriber.gradient.Evaluate(0.25f);
            outcome = "";
            _colorSubscriber.OnAction($"#{(int)(255 * testColor.r):X2}{(int)(255 * testColor.g):X2}{(int)(255 * testColor.b):X2}");
            Assert.AreEqual("True", outcome);
            
            outcome = "";
            testColor.g += 0.1f;
            _colorSubscriber.OnAction($"#{(int)(255 * testColor.r):X2}{(int)(255 * testColor.g):X2}{(int)(255 * testColor.b):X2}");
            Assert.AreEqual("False", outcome);

            outcome = "";
            testColor =Color.clear;
            _colorSubscriber.gradient.colorKeys[0].color = Color.black;
            _colorSubscriber.gradient.colorKeys[1].color = Color.white;
            _colorSubscriber.gradient.alphaKeys[0].alpha = 1f;
            _colorSubscriber.gradient.alphaKeys[1].alpha = 1f;
            _colorSubscriber.OnAction($"#{(int)(255 * testColor.r):X2}{(int)(255 * testColor.g):X2}{(int)(255 * testColor.b):X2}");
            Assert.AreEqual("False", outcome);
            
            outcome = "";
            testColor =Color.white;
            _colorSubscriber.gradient = new Gradient();
            var cKeys = new GradientColorKey[3];
            cKeys[0] = new GradientColorKey(Color.black, 0f);
            cKeys[1] = new GradientColorKey(Color.black, 0.7f);
            cKeys[2] = new GradientColorKey(Color.white, 1f);

            var aKeys = new GradientAlphaKey[3];
            aKeys[0].alpha = 1f;
            aKeys[1].alpha = 1f;
            aKeys[2].alpha = 1f;
            
            _colorSubscriber.gradient.SetKeys(cKeys, aKeys);
            
            _colorSubscriber.OnAction($"#{(int)(255 * testColor.r):X2}{(int)(255 * testColor.g):X2}{(int)(255 * testColor.b):X2}");
            Assert.AreEqual("True", outcome);

            
            outcome = "";
            testColor =Color.green;
            _colorSubscriber.gradient = new Gradient();
            _colorSubscriber.OnAction($"#{(int)(255 * testColor.r):X2}{(int)(255 * testColor.g):X2}{(int)(255 * testColor.b):X2}");
            Assert.AreEqual("False", outcome);

        }
    }
}