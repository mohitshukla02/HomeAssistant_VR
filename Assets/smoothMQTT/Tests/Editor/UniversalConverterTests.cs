using System;
using System.IO;
using NUnit.Framework;
using UnityEngine;
using SmoothMQTT.Subscribing;
using Random = UnityEngine.Random;

namespace Tests
{
    public class UniversalConverterTests
    {
        private GameObject _gameObject;
        private UniversalConverter _universalConverter;
        
        [SetUp]
        public void Setup()
        {
            _gameObject = new GameObject();
            _universalConverter = _gameObject.AddComponent<UniversalConverter>();
        }

        [TearDown]
        public void TearDown()
        {
            GameObject.DestroyImmediate(_gameObject);
            _gameObject= null;

        }
        
        
        // A Test behaves as an ordinary method
        [Test]
        public void NoErrorOnNone()
        {

            _universalConverter.type = UniversalConverter.TargetType.None;
            
            _universalConverter.RunAction("");
        }

        [Test]
        public void FailsOnUnknownType()
        {
            _universalConverter.type = (UniversalConverter.TargetType) int.MaxValue;
            Assert.Throws<ArgumentOutOfRangeException>(()=>_universalConverter.RunAction(""));
        }

        [Test]
        public void ConvertFloatCultureInvariant()
        {
            var teststring = "23.42";
            _universalConverter.type = UniversalConverter.TargetType.Float;
            _universalConverter.floatAction = new UniversalConverter.FloatEvent();
            _universalConverter.floatAction.AddListener(f =>
            {
                Assert.AreEqual(teststring.ToFloat(), f);
            });
            _universalConverter.RunAction(teststring);

        }
        [Test]
        public void ConvertFloatFailsOnWrongSeparator()
        {
            _universalConverter.type = UniversalConverter.TargetType.Float;
            _universalConverter.floatAction = new UniversalConverter.FloatEvent();
            _universalConverter.floatAction.AddListener(f =>
            {
                
            });
            
            Assert.Catch<InvalidDataException>(() => _universalConverter.RunAction("23,42,21"));
            Assert.Catch<InvalidDataException>(() => _universalConverter.RunAction("23,42"));

        }

        [Test]
        public void ConvertBoolReturnsTrue()
        {
            _universalConverter.type = UniversalConverter.TargetType.Bool;
            _universalConverter.boolAction = new UniversalConverter.BoolEvent();
            _universalConverter.boolAction.AddListener(b =>
            {
                Assert.IsTrue(b);
            });
            
            _universalConverter.RunAction("True");
            _universalConverter.RunAction("true");
        }
        
        [Test]
        public void ConvertBoolReturnsFalse()
        {
            _universalConverter.type = UniversalConverter.TargetType.Bool;
            _universalConverter.boolAction = new UniversalConverter.BoolEvent();
            _universalConverter.boolAction.AddListener(b =>
            {
                Assert.IsFalse(b);
            });
            
            _universalConverter.RunAction("False");
            _universalConverter.RunAction("false");
        }

        [Test]
        public void ConvertInt()
        {
            _universalConverter.type = UniversalConverter.TargetType.Int;
            _universalConverter.intAction = new UniversalConverter.IntEvent();
            _universalConverter.intAction.AddListener(i =>
            {
                Assert.AreEqual(42, i);
            });
            _universalConverter.RunAction("42");

            _universalConverter.intAction = new UniversalConverter.IntEvent();
            _universalConverter.intAction.AddListener(i =>
            {
                Assert.AreEqual(-42, i);
            });
            _universalConverter.RunAction("-42");

        }
        
        [Test]
        public void ConvertVector3()
        {
            // Zero Vector
            var vector = new Vector3(0, 0, 0);
            _universalConverter.type = UniversalConverter.TargetType.Vector3;
            _universalConverter.vector3Action = new UniversalConverter.Vector3Event();
            _universalConverter.vector3Action.AddListener(v =>
            {
                Assert.AreEqual(vector, v);
            });
            _universalConverter.RunAction(vector.ToString());

            // Random static vector
            vector = new Vector3(-1.7001f, 2.3456f, 1f);
            _universalConverter.vector3Action = new UniversalConverter.Vector3Event();
            _universalConverter.vector3Action.AddListener(v =>
            {
                Assert.IsTrue(vector.RoughlyEquals(v), $"{vector}.RoughlyEquals({v}, delta: 0.1f)");
            });
            _universalConverter.RunAction(vector.ToString());
            
            // true random and precise vector
            vector = Random.insideUnitSphere;
            _universalConverter.vector3Action = new UniversalConverter.Vector3Event();
            _universalConverter.vector3Action.AddListener(v =>
            {
                Assert.IsTrue(vector.RoughlyEquals(v, 0.0001f), $"{vector}.RoughlyEquals({v}, delta: 0.0001f)");
            });
            _universalConverter.RunAction(vector.ToString("F4"));

        }
        
        [Test]
        public void ConvertQuaternion()
        {
            //identity rotation;
            var rotation = Quaternion.identity;
            _universalConverter.type = UniversalConverter.TargetType.Quaternion;
            _universalConverter.quaternionAction = new UniversalConverter.QuaternionEvent();
            _universalConverter.quaternionAction.AddListener(q =>
            {
                Assert.AreEqual(rotation, q);
            });
            _universalConverter.RunAction(rotation.ToString());

            // random static rotation
            rotation = new Quaternion( -0.527f, 0.728f,  0.310f, 0.310f);
            _universalConverter.quaternionAction = new UniversalConverter.QuaternionEvent();
            _universalConverter.quaternionAction.AddListener(q =>
            {
                Assert.IsTrue(rotation.RoughlyEquals(q), $"{rotation}.RoughlyEquals({q}, delta: 0.1f)");
            });
            _universalConverter.RunAction(rotation.ToString());

            // true random precise rotation
            rotation = Random.rotationUniform;
            _universalConverter.quaternionAction = new UniversalConverter.QuaternionEvent();
            _universalConverter.quaternionAction.AddListener(q =>
            {
                Assert.IsTrue(rotation.RoughlyEquals(q, 0.0001f), $"{rotation}.RoughlyEquals({q}, delta: 0.0001f)");
            });
            _universalConverter.RunAction(rotation.ToString("F4"));
        }
        
        [Test]
        public void ConvertColor()
        {
            var color = Color.magenta;
            _universalConverter.type = UniversalConverter.TargetType.Color;
            _universalConverter.colorAction = new UniversalConverter.ColorEvent();
            _universalConverter.colorAction.AddListener(c =>
            {
                Assert.AreEqual(color, c);
            });
            var hexstring = $"#{(int) Mathf.Lerp(0, 255, color.r):X2}" +
                            $"{(int) Mathf.Lerp(0, 255, color.g):X2}" +
                            $"{(int) Mathf.Lerp(0, 255, color.b):X2}";
            _universalConverter.RunAction(hexstring);
        }


        
    }
}
