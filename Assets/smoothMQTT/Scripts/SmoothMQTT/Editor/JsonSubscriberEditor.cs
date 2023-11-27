using System;
using System.Collections.Generic;
using System.Linq;
using SmoothMQTT.Subscribing;
using UnityEditor;
using UnityEngine;

namespace SmoothMQTT.Editor
{
    [CustomEditor(typeof(JsonSubscriber))]
    public class JsonSubscriberEditor : UnityEditor.Editor
    {
        private JsonSubscriber _subscriber;
        public override void OnInspectorGUI()
        {
            _subscriber = (JsonSubscriber)target;
            var useNewtonsoft = DetectAndActivateNewtonsoft();

            if (useNewtonsoft)
            {
                EditorGUILayout.LabelField("Using Newtonsoft JSON");
            }
            else
            {
                EditorGUILayout.LabelField("Using simple JSON (nested objects and flat lists only");
            }
            base.OnInspectorGUI();
        }


        public bool DetectAndActivateNewtonsoft()
        {
            // Deactivate unreachable code warning
            #pragma warning disable 0162
            #if NewtonsoftJson
            return true;
            #endif
            EditorGUILayout.LabelField(
                "Newtonsoft JSON has not yet been registered in the Scripting Symbols. If it is installed, just press the Button below. Otherwise install it first, please.");
            if (GUILayout.Button("Look for Newtonsoft JSON."))
            {
                Debug.LogWarning("Looking for Newtonsoft JSON");

                var buildTarget = EditorUserBuildSettings.selectedBuildTargetGroup;

                List<string> symbols =
                    new List<string>(PlayerSettings.GetScriptingDefineSymbolsForGroup(buildTarget).Split(';'));
                if (symbols.Contains("NewtonsoftJson"))
                {
                    return false;
                }

                // TODO: Find Name and class in Newtonsoft Json
                var plugin = AppDomain.CurrentDomain
                    .GetAssemblies()
                    .Where(assembly => assembly.FullName.Contains("Newtonsoft.Json"))
                    .Select(assembly => assembly.GetType("Newtonsoft.Json.JsonConvert", false))
                    .FirstOrDefault(t => t != null);
                if (plugin != null)
                {
                    symbols.Add("NewtonsoftJson");

                    var newSymbols = "";
                    foreach (var symbol in symbols)
                    {
                        newSymbols += symbol + ";";
                    }

                    newSymbols.TrimEnd(';');
                    PlayerSettings.SetScriptingDefineSymbolsForGroup(buildTarget, newSymbols);
                    Debug.Log("Newtonsoft Json detected! Please wait a few seconds for scripts to recompile!");
                    return false;
                }
                Debug.LogWarning("Newtonsoft Json has not been detected.");
            }
            return false;
        }
        #pragma warning restore 0162
}
}