using System;
using System.Collections.Generic;
using System.Linq;
using SmoothMQTT.Sending;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PublishFromTMPInputField))]
public class TMPChecker : Editor
{
    #if !TextMeshPro
    public override void OnInspectorGUI()
    {
        EditorGUILayout.LabelField("TextMeshPro has not yet been registered in the Scripting Symbols. If it is installed, just press the Button below. Otherwise install it please.");
        if (!GUILayout.Button("Look for TextMeshPro Now."))
        {
            return;
        }
        Debug.LogWarning("Looking for TextMeshPro");
            
        var buildTarget =  EditorUserBuildSettings.selectedBuildTargetGroup;
        List<string> symbols = new List<string>(PlayerSettings.GetScriptingDefineSymbolsForGroup(buildTarget).Split(';'));
        if (symbols.Contains("TextMeshPro"))
        {
            return;
        }
        var plugin = AppDomain.CurrentDomain
            .GetAssemblies()
            .Where( assembly => assembly.FullName.Contains( "TextMeshPro" ) )
            .Select( assembly => assembly.GetType( "TMPro.TMP_FontAsset", false ) )
            .FirstOrDefault(t => t!=null);
        if (plugin != null)
        {
            symbols.Add("TextMeshPro");

            var newSymbols = "";
            foreach (var symbol in symbols)
            {
                newSymbols += symbol + ";";
            }

            newSymbols.TrimEnd(';');
            PlayerSettings.SetScriptingDefineSymbolsForGroup(buildTarget, newSymbols);
            Debug.Log("TextMeshPro detected! Please wait a few seconds for scripts to recompile!");
            
            return;
        }
        Debug.LogWarning("TextMesh Pro has not been detected.");
    }
    #endif
}
