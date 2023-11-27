using System;
using SmoothMQTT.Subscribing;
using SmoothMQTT.Core;
using UnityEditor;
using UnityEditor.Events;
using UnityEngine;

[CustomEditor(typeof(UniversalConverter))]
public class UniversalConverterEditor : Editor
{
    public override void OnInspectorGUI()
    {
        var uc = (UniversalConverter)target;

        uc._registerWithSubscriber = EditorGUILayout.ToggleLeft("Automatically register with Subscriber.Action", uc._registerWithSubscriber);

        uc.type = (UniversalConverter.TargetType)EditorGUILayout.EnumPopup(uc.type);
        this.serializedObject.Update();

        var subscriber = uc.GetComponent<Subscriber>();
        if (uc._registerWithSubscriber && subscriber != null)
        {
            var index = -1;
            for (int i = 0; i < subscriber.action.GetPersistentEventCount(); ++i)
            {
                index = subscriber.action.GetPersistentMethodName(i).Equals(nameof(uc.RunAction))? i : -1;
            }

            if (index == -1 && uc.type != UniversalConverter.TargetType.None)
            {
                UnityEventTools.AddPersistentListener(subscriber.action, uc.RunAction);
            }
            else if (index != -1 && uc.type == UniversalConverter.TargetType.None)
            {
                UnityEventTools.RemovePersistentListener(subscriber.action, index);
            }
        }
        
        switch (uc.type){
            case UniversalConverter.TargetType.None:
                GUILayout.Label("Select a target type to convert to!");
                break;
            case UniversalConverter.TargetType.Float:
                EditorGUILayout.PropertyField(serializedObject.FindProperty("floatAction"), true);
                break;
            case UniversalConverter.TargetType.Bool:
                EditorGUILayout.PropertyField(serializedObject.FindProperty("boolAction"), true);
                break;
            case UniversalConverter.TargetType.Int:
                EditorGUILayout.PropertyField(serializedObject.FindProperty("intAction"), true);
                break;
            case UniversalConverter.TargetType.Vector3:
                EditorGUILayout.PropertyField(serializedObject.FindProperty("vector3Action"), true);
                break;
            case UniversalConverter.TargetType.Quaternion:
                EditorGUILayout.PropertyField(serializedObject.FindProperty("quaternionAction"), true);
                break;
            case UniversalConverter.TargetType.Color:
                EditorGUILayout.PropertyField(serializedObject.FindProperty("colorAction"), true);
                break;
            default:
                throw new ArgumentOutOfRangeException("TargetType not defined in Editor");
        }
        this.serializedObject.ApplyModifiedProperties();

    }
}
