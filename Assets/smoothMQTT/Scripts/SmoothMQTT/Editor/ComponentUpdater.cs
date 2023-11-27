using System;
using SmoothMQTT.Subscribing;
using UnityEditor;
using UnityEditor.Events;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Events;
using Object = UnityEngine.Object;

namespace SmoothMQTT.Editor
{
    [CustomEditor(typeof(ConverterBehaviour), true)]
    public class ComponentUpdater : UnityEditor.Editor
    {
        [MenuItem("Tools/SmoothMQTT/Update Converter Components", false, 500)]
        public static void ScanForConvertersAndReplace()
        {
            if (EditorUtility.DisplayDialog("Confirm update",
                    "With clicking update now, Unity will attempt to update all converter components in your project. Make sure you have a backup available in case something goes wrong.",
                    "Update Now", "Cancel"))
            {
                var hasChanges = false;
                hasChanges = hasChanges || UpdateFloatConverters();
                hasChanges = hasChanges || UpdateIntConverters();
                hasChanges = hasChanges || UpdateVector3Converters();
                hasChanges = hasChanges || UpdateQuaternionConverters();
                hasChanges = hasChanges || UpdateColorConverters();
                if (hasChanges)
                {
                    EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
                }
                else
                {
                    Debug.Log("Nothing to update!");
                }
            }
            else
            {
                Debug.Log("Canceled update.");
            }
        }


        private static bool UpdateFloatConverters()
        {
#pragma warning disable 0618
            var floatConverters = Object.FindObjectsOfType<SubscriberFloatConverter>();
#pragma warning restore 0618
            if (floatConverters.Length == 0)
            {
                return false;
            }

            foreach (var converter in floatConverters)
            {
                var go = converter.gameObject;
                var goInstance = go;
                var modifyPrefab = PrefabUtility.IsPartOfAnyPrefab(go);
                var prefabPath = "";

                var uc = go.AddComponent<UniversalConverter>();
                uc.type = UniversalConverter.TargetType.Float;
                uc.floatAction = new UniversalConverter.FloatEvent();
                if (converter.subscriber != null)
                {
                    // register to the same subscriber
                    int indexOfSubscriberAction = -1;
                    var numberOfEvents = converter.subscriber.action.GetPersistentEventCount();
                    for (int i = 0; i < numberOfEvents; i++)
                    {
                        if (converter.subscriber.action.GetPersistentTarget(i) == converter)
                        {
                            indexOfSubscriberAction = i;
                            break;
                        }
                    }

                    if (indexOfSubscriberAction >= 0)
                    {
                        UnityEventTools.RemovePersistentListener(converter.subscriber.action, indexOfSubscriberAction);
                    }

                    UnityEventTools.AddPersistentListener(converter.subscriber.action, uc.RunAction);
                    uc._registerWithSubscriber = true;
                }

                for (int i = 0; i < converter.action.GetPersistentEventCount(); i++)
                {
                    // copy persistent actions.
                    var eventTarget = converter.action.GetPersistentTarget(i);
                    var eventFunction = converter.action.GetPersistentMethodName(i);
                    try
                    {
                        var method = (UnityAction<float>)Delegate.CreateDelegate(typeof(UnityAction<float>),
                            eventTarget, eventFunction);

                        UnityEventTools.AddPersistentListener(uc.floatAction, method);
                    }
                    catch
                    {
                        //pass
                    }
                }

                Debug.Log($"Replaced FloatConverter on {converter.name}.", goInstance);
                DestroyImmediate(converter);
                if (modifyPrefab)
                {
                    prefabPath = PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(goInstance);
                    PrefabUtility.SaveAsPrefabAssetAndConnect(PrefabUtility.GetNearestPrefabInstanceRoot(goInstance),
                        prefabPath, InteractionMode.AutomatedAction, out var successfully);
                    Debug.Log($"Changing prefab {(successfully ? "successful" : "failed")}");
                }
            }

            return true;
        }

        private static bool UpdateIntConverters()
        {
#pragma warning disable 0618
            var intConverters = Object.FindObjectsOfType<SubscriberIntConverter>();
#pragma warning restore 0618
            if (intConverters.Length == 0)
            {
                return false;
            }

            foreach (var converter in intConverters)
            {
                var go = converter.gameObject;
                var goInstance = go;
                var modifyPrefab = PrefabUtility.IsPartOfAnyPrefab(go);
                var prefabPath = "";

                var uc = go.AddComponent<UniversalConverter>();
                uc.type = UniversalConverter.TargetType.Int;
                uc.intAction = new UniversalConverter.IntEvent();
                if (converter.subscriber != null)
                {
                    // register to the same subscriber
                    int indexOfSubscriberAction = -1;
                    var numberOfEvents = converter.subscriber.action.GetPersistentEventCount();
                    for (int i = 0; i < numberOfEvents; i++)
                    {
                        if (converter.subscriber.action.GetPersistentTarget(i) == converter)
                        {
                            indexOfSubscriberAction = i;
                            break;
                        }
                    }

                    if (indexOfSubscriberAction >= 0)
                    {
                        UnityEventTools.RemovePersistentListener(converter.subscriber.action, indexOfSubscriberAction);
                    }

                    UnityEventTools.AddPersistentListener(converter.subscriber.action, uc.RunAction);
                    uc._registerWithSubscriber = true;
                }

                for (int i = 0; i < converter.action.GetPersistentEventCount(); i++)
                {
                    // copy persistent actions.
                    var eventTarget = converter.action.GetPersistentTarget(i);
                    var eventFunction = converter.action.GetPersistentMethodName(i);
                    try
                    {
                        var method = (UnityAction<int>)Delegate.CreateDelegate(typeof(UnityAction<int>),
                            eventTarget, eventFunction);

                        UnityEventTools.AddPersistentListener(uc.intAction, method);
                    }
                    catch
                    {
                        //pass
                    }
                }

                Debug.Log($"Replaced IntConverter on {converter.name}.", goInstance);
                DestroyImmediate(converter);
                if (modifyPrefab)
                {
                    prefabPath = PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(goInstance);

                    PrefabUtility.SaveAsPrefabAssetAndConnect(PrefabUtility.GetNearestPrefabInstanceRoot(goInstance),
                        prefabPath, InteractionMode.AutomatedAction, out var successfully);
                    Debug.Log($"Changing prefab {(successfully ? "successful" : "failed")}");
                }
            }

            return true;
        }

        private static bool UpdateVector3Converters()
        {
#pragma warning disable 0618
            var vectorConverters = Object.FindObjectsOfType<SubscriberVector3Converter>();
#pragma warning restore 0618
            if (vectorConverters.Length == 0)
            {
                return false;
            }

            foreach (var converter in vectorConverters)
            {
                var go = converter.gameObject;
                var goInstance = go;
                var modifyPrefab = PrefabUtility.IsPartOfAnyPrefab(go);
                var prefabPath = "";

                var uc = go.AddComponent<UniversalConverter>();
                uc.type = UniversalConverter.TargetType.Vector3;
                uc.vector3Action = new UniversalConverter.Vector3Event();
                if (converter.subscriber != null)
                {
                    // register to the same subscriber
                    int indexOfSubscriberAction = -1;
                    var numberOfEvents = converter.subscriber.action.GetPersistentEventCount();
                    for (int i = 0; i < numberOfEvents; i++)
                    {
                        if (converter.subscriber.action.GetPersistentTarget(i) == converter)
                        {
                            indexOfSubscriberAction = i;
                            break;
                        }
                    }

                    if (indexOfSubscriberAction >= 0)
                    {
                        UnityEventTools.RemovePersistentListener(converter.subscriber.action, indexOfSubscriberAction);
                    }

                    UnityEventTools.AddPersistentListener(converter.subscriber.action, uc.RunAction);
                    uc._registerWithSubscriber = true;
                }

                for (int i = 0; i < converter.action.GetPersistentEventCount(); i++)
                {
                    // copy persistent actions.
                    var eventTarget = converter.action.GetPersistentTarget(i);
                    var eventFunction = converter.action.GetPersistentMethodName(i);
                    try
                    {
                        var method = (UnityAction<Vector3>)Delegate.CreateDelegate(typeof(UnityAction<Vector3>),
                            eventTarget, eventFunction);

                        UnityEventTools.AddPersistentListener(uc.vector3Action, method);
                    }
                    catch
                    {
                        //pass
                    }
                }

                Debug.Log($"Replaced VectorConverter on {converter.name}.", goInstance);
                DestroyImmediate(converter);
                if (modifyPrefab)
                {
                    prefabPath = PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(goInstance);
                    PrefabUtility.SaveAsPrefabAssetAndConnect(PrefabUtility.GetNearestPrefabInstanceRoot(goInstance),
                        prefabPath, InteractionMode.AutomatedAction, out var successfully);
                    Debug.Log($"Changing prefab {(successfully ? "successful" : "failed")}");
                }
            }

            return true;
        }

        private static bool UpdateQuaternionConverters()
        {
#pragma warning disable 0618
            var quaternionConverters = Object.FindObjectsOfType<SubscriberQuaternionConverter>();
#pragma warning restore 0618
            if (quaternionConverters.Length == 0)
            {
                return false;
            }

            foreach (var converter in quaternionConverters)
            {
                var go = converter.gameObject;
                var goInstance = go;
                var modifyPrefab = PrefabUtility.IsPartOfAnyPrefab(go);
                var prefabPath = "";

                var uc = go.AddComponent<UniversalConverter>();
                uc.type = UniversalConverter.TargetType.Quaternion;
                uc.quaternionAction = new UniversalConverter.QuaternionEvent();
                if (converter.subscriber != null)
                {
                    // register to the same subscriber
                    int indexOfSubscriberAction = -1;
                    var numberOfEvents = converter.subscriber.action.GetPersistentEventCount();
                    for (int i = 0; i < numberOfEvents; i++)
                    {
                        if (converter.subscriber.action.GetPersistentTarget(i) == converter)
                        {
                            indexOfSubscriberAction = i;
                            break;
                        }
                    }

                    if (indexOfSubscriberAction >= 0)
                    {
                        UnityEventTools.RemovePersistentListener(converter.subscriber.action, indexOfSubscriberAction);
                    }

                    UnityEventTools.AddPersistentListener(converter.subscriber.action, uc.RunAction);
                    uc._registerWithSubscriber = true;
                }

                for (int i = 0; i < converter.action.GetPersistentEventCount(); i++)
                {
                    // copy persistent actions.
                    var eventTarget = converter.action.GetPersistentTarget(i);
                    var eventFunction = converter.action.GetPersistentMethodName(i);
                    try
                    {
                        var method = (UnityAction<Quaternion>)Delegate.CreateDelegate(typeof(UnityAction<Quaternion>),
                            eventTarget, eventFunction);

                        UnityEventTools.AddPersistentListener(uc.quaternionAction, method);
                    }
                    catch
                    {
                        //pass
                    }
                }

                Debug.Log($"Replaced QuaternionConverter on {converter.name}.", goInstance);
                DestroyImmediate(converter);
                if (modifyPrefab)
                {
                    prefabPath = PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(goInstance);
                    PrefabUtility.SaveAsPrefabAssetAndConnect(PrefabUtility.GetNearestPrefabInstanceRoot(goInstance),
                        prefabPath, InteractionMode.AutomatedAction, out var successfully);
                    Debug.Log($"Changing prefab {(successfully ? "successful" : "failed")}");
                }
            }

            return true;
        }

        private static bool UpdateColorConverters()
        {
#pragma warning disable 0618
            var colorConverters = Object.FindObjectsOfType<SubscriberColorConverter>();
#pragma warning restore 0618
            if (colorConverters.Length == 0)
            {
                return false;
            }

            foreach (var converter in colorConverters)
            {
                var go = converter.gameObject;
                var goInstance = go;
                var modifyPrefab = PrefabUtility.IsPartOfAnyPrefab(go);
                var prefabPath = "";

                var uc = go.AddComponent<UniversalConverter>();
                uc.type = UniversalConverter.TargetType.Color;
                uc.colorAction = new UniversalConverter.ColorEvent();


                if (converter.subscriber != null)
                {
                    // register to the same subscriber
                    int indexOfSubscriberAction = -1;
                    var numberOfEvents = converter.subscriber.action.GetPersistentEventCount();
                    for (int i = 0; i < numberOfEvents; i++)
                    {
                        if (converter.subscriber.action.GetPersistentTarget(i) == converter)
                        {
                            indexOfSubscriberAction = i;
                            break;
                        }
                    }

                    if (indexOfSubscriberAction >= 0)
                    {
                        UnityEventTools.RemovePersistentListener(converter.subscriber.action, indexOfSubscriberAction);
                    }

                    UnityEventTools.AddPersistentListener(converter.subscriber.action, uc.RunAction);
                    uc._registerWithSubscriber = true;
                }

                for (int i = 0; i < converter.action.GetPersistentEventCount(); i++)
                {
                    // copy persistent actions.
                    var eventTarget = converter.action.GetPersistentTarget(i);
                    var eventFunction = converter.action.GetPersistentMethodName(i);
                    try
                    {
                        var method = (UnityAction<Color>)Delegate.CreateDelegate(typeof(UnityAction<Color>),
                            eventTarget, eventFunction);

                        UnityEventTools.AddPersistentListener(uc.colorAction, method);
                    }
                    catch
                    {
                        //pass
                    }
                }

                Debug.Log($"Replaced ColorConverter on {converter.name}.", goInstance);
                DestroyImmediate(converter);
                if (modifyPrefab)
                {
                    prefabPath = PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(goInstance);
                    PrefabUtility.SaveAsPrefabAssetAndConnect(PrefabUtility.GetNearestPrefabInstanceRoot(goInstance),
                        prefabPath, InteractionMode.AutomatedAction, out var successfully);
                    Debug.Log($"Changing prefab {(successfully ? "successful" : "failed")}");
                }
            }

            return true;
        }
    }
}