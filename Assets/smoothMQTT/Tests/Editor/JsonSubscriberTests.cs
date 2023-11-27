using NUnit.Framework;
using UnityEngine;
using SmoothMQTT.Subscribing;
using SmoothMQTT.Core;

namespace Tests
{
    public class JsonSubscriberTests
    {
        
        
        private GameObject _gameObject;
        private JsonSubscriber _jsonSubscriber;
        private string _mockJson;
        
        [SetUp]
        public void Setup()
        {
            _gameObject = new GameObject();
            _jsonSubscriber = _gameObject.AddComponent<JsonSubscriber>();

            _jsonSubscriber.pathToValue = "test/value";

            _mockJson = "{\"status\":\"success\", \"test\":{\"type\":\"mock\", \"value\":7, \"list\":[5, 6, {\"person\":{\"name\":\"smooth operator\"}}]}, \"Test\":\"case sensitive\"}";
            /*GameObject prefab = Resources.Load<GameObject>("MqttManagerWithBroker");
            _brokerObject = PrefabUtility.InstantiatePrefab(prefab) as GameObject;

            Settings.Instance.host = "localhost";
            Settings.Instance.port = 1883;
            Settings.Instance.QOSLevel = MqttQualityOfServiceLevel.AtMostOnce;*/
        }

        [TearDown]
        public void TearDown()
        {
            GameObject.DestroyImmediate(_gameObject);
            _gameObject= null;

        }
                
        [Test]
        public void KeyValuePathsOnlyNewtonsoft()
        {
            _jsonSubscriber.doOnMessage = new StringEvent();
            _jsonSubscriber.doOnMessage.AddListener((s) =>
            {
                Assert.AreEqual("7", s);
            });
            _jsonSubscriber.OnAction(_mockJson);
            
        }
        
        [Test]
        public void RootLevelObjectReturns()
        {
            _jsonSubscriber.pathToValue = "status";
            _jsonSubscriber.doOnMessage = new StringEvent();
            _jsonSubscriber.doOnMessage.AddListener((s) =>
            {
                Assert.AreEqual("success", s);
            });
            _jsonSubscriber.OnAction(_mockJson);
            
        }
        
        [Test]
        public void CaseSensitivityIsHeeded()
        {
            _jsonSubscriber.pathToValue = "Test";
            _jsonSubscriber.doOnMessage = new StringEvent();
            _jsonSubscriber.doOnMessage.AddListener((s) =>
            {
                Assert.AreEqual("case sensitive", s);
            });
            _jsonSubscriber.OnAction(_mockJson);
            
        }
        
                
        [Test]
        public void SelectItemThroughListAccessor()
        {
            _jsonSubscriber.pathToValue = "test/list/1";
            _jsonSubscriber.doOnMessage = new StringEvent();
            _jsonSubscriber.doOnMessage.AddListener((s) =>
            {
                Assert.AreEqual("6", s);
            });
            _jsonSubscriber.OnAction(_mockJson);
            
        }
        
        #if NewtonsoftJson
        [Test]
        public void SelectObjectNestedInList()
        {
            _jsonSubscriber.pathToValue = "test/list/2/person/name";
            _jsonSubscriber.doOnMessage = new StringEvent();
            _jsonSubscriber.doOnMessage.AddListener((s) =>
            {
                Assert.AreEqual("smooth operator", s);
            });
            _jsonSubscriber.OnAction(_mockJson);
            
        }
        #endif
    }
}
