using System;
using System.IO;
using System.Linq;
using SmoothMQTT.Core;
using SmoothMQTT.Sending;
using SmoothMQTT.Subscribing;
using UnityEditor;
using UnityEditor.Events;
using UnityEditorInternal;
using UnityEngine;

namespace SmoothMQTT.Editor
{

    public class ComponentToolsMenu : UnityEditor.Editor
    {
        #region GameObjects

        [MenuItem("GameObject/SmoothMQTT/MQTTManager With Broker", false, 10)]
        public static void OnAddMQTTManagerWithBroker(MenuCommand menuCommand)
        {
            GameObject prefab = Resources.Load<GameObject>("MqttManagerWithBroker");
            GameObject go = PrefabUtility.InstantiatePrefab(prefab) as GameObject;
            GameObjectUtility.SetParentAndAlign(go, menuCommand.context as GameObject);
            Undo.RegisterCreatedObjectUndo(go, "Create " + go.name);
            Selection.activeObject = go;
            Undo.RecordObject(go, "Create MQTT Manager");
        }

        [MenuItem("GameObject/SmoothMQTT/MQTTManager Without Broker", false, 10)]
        public static void OnAddMQTTManager(MenuCommand menuCommand)
        {
            GameObject prefab = Resources.Load<GameObject>("MqttManager");
            GameObject go = PrefabUtility.InstantiatePrefab(prefab) as GameObject;
            GameObjectUtility.SetParentAndAlign(go, menuCommand.context as GameObject);
            Undo.RegisterCreatedObjectUndo(go, "Create " + go.name);
            Selection.activeObject = go;
            Undo.RecordObject(go, "Create MQTT Manager");
        }

        #endregion

        #region Subscribers

        static UniversalConverter AddUniversalConverter(GameObject selectedObject, UniversalConverter.TargetType type)
        {
            var component = Undo.AddComponent<UniversalConverter>(selectedObject);
            component.type = type;
            return component;
        }

        static Subscriber GetOrAddSubscriber(GameObject selectedObject)
        {
            var subscriber = selectedObject.GetComponent<Subscriber>();
            if (subscriber == null)
            {
                subscriber = Undo.AddComponent<Subscriber>(selectedObject);
                subscriber.clientID = GUID.Generate().ToString();
            }

            return subscriber;
        }

        private static void AddSubscriberListenerForConverter(Subscriber subscriber, UniversalConverter converter)
        {
            if (subscriber is JsonSubscriber jsonSubscriber)
            {
                UnityEventTools.AddPersistentListener(jsonSubscriber.doOnMessage, converter.RunAction);
            }
            else
            {
                UnityEventTools.AddPersistentListener(subscriber.action, converter.RunAction);
            }

        }

        [MenuItem("Tools/SmoothMQTT/Receive/String Payload", false, 10)]
        public static void OnAddStringSubscriber()
        {
            GameObject selected = UnityEditor.Selection.activeGameObject;
            if (selected == null)
            {
                Debug.LogWarning("No Gameobject selected, to add Subscriber to");
                return;
            }

            var component = Undo.AddComponent<Subscriber>(selected);
            component.clientID = GUID.Generate().ToString();
            EditorUtility.SetDirty(selected);
        }

        [MenuItem("Tools/SmoothMQTT/Receive/Float Payload", false, 10)]
        public static void OnAddFloatSubscriber()
        {
            GameObject selected = UnityEditor.Selection.activeGameObject;
            if (selected == null)
            {
                Debug.LogWarning("No Gameobject selected, to add Subscriber to");
                return;
            }

            var subscriber = GetOrAddSubscriber(selected);
            var converter = AddUniversalConverter(selected, UniversalConverter.TargetType.Float);
            AddSubscriberListenerForConverter(subscriber, converter);

            EditorUtility.SetDirty(selected);
        }

        [MenuItem("Tools/SmoothMQTT/Receive/Int Payload", false, 10)]
        public static void OnAddIntSubscriber()
        {
            GameObject selected = UnityEditor.Selection.activeGameObject;
            if (selected == null)
            {
                Debug.LogWarning("No Gameobject selected, to add Subscriber to");
                return;
            }

            var subscriber = GetOrAddSubscriber(selected);
            var converter = AddUniversalConverter(selected, UniversalConverter.TargetType.Int);
            AddSubscriberListenerForConverter(subscriber, converter);

            EditorUtility.SetDirty(selected);
        }

        [MenuItem("Tools/SmoothMQTT/Receive/Bool Payload", false, 10)]
        public static void OnAddBoolSubscriber()
        {
            GameObject selected = UnityEditor.Selection.activeGameObject;
            if (selected == null)
            {
                Debug.LogWarning("No Gameobject selected, to add Subscriber to");
                return;
            }

            var subscriber = GetOrAddSubscriber(selected);
            var converter = AddUniversalConverter(selected, UniversalConverter.TargetType.Bool);
            AddSubscriberListenerForConverter(subscriber, converter);

            EditorUtility.SetDirty(selected);
        }

        [MenuItem("Tools/SmoothMQTT/Receive/Color Payload", false, 10)]
        public static void OnAddColorSubscriber()
        {
            GameObject selected = UnityEditor.Selection.activeGameObject;
            if (selected == null)
            {
                Debug.LogWarning("No Gameobject selected, to add Subscriber to");
                return;
            }

            var subscriber = GetOrAddSubscriber(selected);
            var converter = AddUniversalConverter(selected, UniversalConverter.TargetType.Color);
            AddSubscriberListenerForConverter(subscriber, converter);

            EditorUtility.SetDirty(selected);
        }

        [MenuItem("Tools/SmoothMQTT/Receive/Vector3 Payload", false, 10)]
        public static void OnAddVector3Subscriber()
        {
            GameObject selected = UnityEditor.Selection.activeGameObject;
            if (selected == null)
            {
                Debug.LogWarning("No Gameobject selected, to add Subscriber to");
                return;
            }

            var subscriber = GetOrAddSubscriber(selected);
            var converter = AddUniversalConverter(selected, UniversalConverter.TargetType.Vector3);
            AddSubscriberListenerForConverter(subscriber, converter);

            EditorUtility.SetDirty(selected);
        }

        [MenuItem("Tools/SmoothMQTT/Receive/Quaternion Payload", false, 10)]
        public static void OnAddQuaternionSubscriber()
        {
            GameObject selected = UnityEditor.Selection.activeGameObject;
            if (selected == null)
            {
                Debug.LogWarning("No Gameobject selected, to add Subscriber to");
                return;
            }

            var subscriber = GetOrAddSubscriber(selected);
            var converter = AddUniversalConverter(selected, UniversalConverter.TargetType.Quaternion);
            AddSubscriberListenerForConverter(subscriber, converter);

            EditorUtility.SetDirty(selected);
        }



        [MenuItem("Tools/SmoothMQTT/Receive/Json Payload", false, 120)]
        public static void OnAddJsonSubscriber()
        {
            GameObject selected = UnityEditor.Selection.activeGameObject;
            if (selected == null)
            {
                Debug.LogWarning("No Gameobject selected, to add Subscriber to");
                return;
            }

            var subscriber = Undo.AddComponent<JsonSubscriber>(selected);
            subscriber.clientID = GUID.Generate().ToString();

            EditorUtility.SetDirty(selected);
        }

        #endregion

        #region SubscriberSpecials

        [MenuItem("Tools/SmoothMQTT/Receive/Compare String Subscriber", false, 230)]
        public static void OnAddConditionalSubscriber()
        {
            GameObject selected = Selection.activeGameObject;
            if (selected == null)
            {
                Debug.LogWarning("No Gameobject selected, to add Subscriber to");
                return;
            }

            var subscriber = GetOrAddSubscriber(selected);
            var interpreter = Undo.AddComponent<ConditionalSubscriber>(selected);

            UnityEventTools.AddPersistentListener(subscriber.action, interpreter.OnAction);
            EditorUtility.SetDirty(selected);
        }

        [MenuItem("Tools/SmoothMQTT/Receive/Compare Float Subscriber", false, 230)]
        public static void OnAddConditionalFloatSubscriber()
        {
            GameObject selected = Selection.activeGameObject;
            if (selected == null)
            {
                Debug.LogWarning("No Gameobject selected, to add Subscriber to");
                return;
            }

            var subscriber = GetOrAddSubscriber(selected);
            var floatInterpreter = Undo.AddComponent<ConditionalFloatSubscriber>(selected);
            UnityEventTools.AddPersistentListener(subscriber.action, floatInterpreter.OnAction);
            EditorUtility.SetDirty(selected);
        }

        [MenuItem("Tools/SmoothMQTT/Receive/Compare Color Subscriber", false, 230)]
        public static void OnAddConditionalColorSubscriber()
        {
            GameObject selected = Selection.activeGameObject;
            if (selected == null)
            {
                Debug.LogWarning("No Gameobject selected, to add Subscriber to");
                return;
            }

            var subscriber = GetOrAddSubscriber(selected);
            var colorInterpreter = Undo.AddComponent<ConditionalColorSubscriber>(selected);
            UnityEventTools.AddPersistentListener(subscriber.action, colorInterpreter.OnAction);
            EditorUtility.SetDirty(selected);
        }

        #endregion

        #region Publishers

        [MenuItem("Tools/SmoothMQTT/Send/string, int, float, bool", false, 10)]
        public static void OnAddPrimitivePublisher()
        {
            GameObject selected = Selection.activeGameObject;
            if (selected == null)
            {
                Debug.LogWarning("No Gameobject selected, to add Publisher to");
                return;
            }

            Undo.AddComponent<PublishValue>(selected);
            EditorUtility.SetDirty(selected);
        }

        [MenuItem("Tools/SmoothMQTT/Send/Color", false, 10)]
        public static void OnAddColorPublisher()
        {
            GameObject selected = Selection.activeGameObject;
            if (selected == null)
            {
                Debug.LogWarning("No Gameobject selected, to add Publisher to");
                return;
            }

            Undo.AddComponent<PublishColor>(selected);
            EditorUtility.SetDirty(selected);
        }

        [MenuItem("Tools/SmoothMQTT/Send/Vector3", false, 10)]
        public static void OnAddVector3Publisher()
        {
            GameObject selected = Selection.activeGameObject;
            if (selected == null)
            {
                Debug.LogWarning("No Gameobject selected, to add Publisher to");
                return;
            }

            Undo.AddComponent<PublishVector3>(selected);
            EditorUtility.SetDirty(selected);
        }

        [MenuItem("Tools/SmoothMQTT/Send/Rotation(Quaternion)", false, 10)]
        public static void OnAddQuaternionPublisher()
        {
            GameObject selected = Selection.activeGameObject;
            if (selected == null)
            {
                Debug.LogWarning("No Gameobject selected, to add Publisher to");
                return;
            }

            Undo.AddComponent<PublishQuaternion>(selected);
            EditorUtility.SetDirty(selected);
        }

        [MenuItem("Tools/SmoothMQTT/Send/TMPInputField (Needs selected TMP_InputField)", false, 10)]
        public static void OnAddTMPInputPublisher()
        {
            GameObject selected = Selection.activeGameObject;
            if (selected == null)
            {
                Debug.LogWarning("No Gameobject selected, to add Publisher to");
                return;
            }

            Undo.AddComponent<PublishFromTMPInputField>(selected);
            EditorUtility.SetDirty(selected);
        }

        [MenuItem("Tools/SmoothMQTT/Send/React To UnityEvents", false, 10)]
        public static void OnAddEventSender()
        {
            GameObject selected = Selection.activeGameObject;
            if (selected == null)
            {
                Debug.LogWarning("No Gameobject selected, to add Publisher to");
                return;
            }

            Undo.AddComponent<SendOnEvent>(selected);
            EditorUtility.SetDirty(selected);
        }

        [MenuItem("Tools/SmoothMQTT/Send/React To Trigger or Collision", false, 10)]
        public static void OnAddTriggerCollisionSender()
        {
            GameObject selected = Selection.activeGameObject;
            if (selected == null)
            {
                Debug.LogWarning("No Gameobject selected, to add Publisher to");
                return;
            }

            if (selected.GetComponent<Collider>() == null)
            {
                Undo.AddComponent<SphereCollider>(selected);
            }

            var component = Undo.AddComponent<PublishTriggerCollision>(selected);
            component.topic = $"{selected.name}/TriggerCollision";
            Debug.Log(
                "For triggers and collisions to work properly, you might need a Rigidbody. Please check out the collision matrix at https://docs.unity3d.com/Manual/CollidersOverview.html");
            EditorUtility.SetDirty(selected);
        }

        #endregion

        #region Other Tools
         
        [MenuItem("Tools/SmoothMQTT/Activate Android (Experimental)", false, 510)]
        public static void OnActivateAndroid()
        {
            if (EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Android, BuildTarget.Android))
            {
                var smoothMQTTAsmDefPath = AssetDatabase.GetAllAssetPaths().Single(v => v.EndsWith("SmoothMQTT.asmdef"));
                if (smoothMQTTAsmDefPath == null)
                {
                    throw new Exception("Couldn't find SmoothMQTT.asmdef, make sure it is installed and unmodified.");
                }
                var asmdefJson = JsonUtility.FromJson<AsmDefJson>(File.ReadAllText(smoothMQTTAsmDefPath));
                var includePlatforms = asmdefJson.includePlatforms.ToList();
                if (!includePlatforms.Contains("Android"))
                {
                    includePlatforms.Add("Android");
                    asmdefJson.includePlatforms = includePlatforms.ToArray();
                    Debug.Log($"Writing asmdef {asmdefJson}");
                    File.WriteAllText(smoothMQTTAsmDefPath, JsonUtility.ToJson(asmdefJson, true));
                    AssetDatabase.Refresh();
                }
                return;
            }
            Debug.LogWarning("Couldn't switch to Android. Please check if you have the Android build target installed.");
        }

        #endregion
    }

    public class AsmDefJson
    {
        public string name;
#if UNITY_2020_2_OR_NEWER
         public string rootNamespace;
#endif
        public bool allowUnsafeCode = false;
        public bool overrideReferences = false;
        public bool autoReferenced = false;
        public bool noEngineReferences = false;

        public string[] references;
        public string[] includePlatforms;
        public string[] excludePlatforms;
        public string[] precompiledReferences;
        public string[] defineConstraints;
        public string[] versionDefines;
    }
}