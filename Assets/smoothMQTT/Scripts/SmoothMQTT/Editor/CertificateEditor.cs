using System;
using System.IO;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using SmoothMQTT.Core;
using UnityEditor;
using UnityEngine;

namespace SmoothMQTT.Editor
{
    [CustomEditor(typeof(ScriptableCertificate))]
    public class CertificateEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            var certificateStreamingAssetsPath = Path.GetFullPath(Path.Combine(Application.streamingAssetsPath, "certificates"));
            var cert = (ScriptableCertificate) target;
            var appPath = Path.GetFullPath(Path.GetDirectoryName(Application.dataPath));
            var relPath = cert.GetRelativePath() ?? "";
            var checkPath = (cert.absPath ?? "").ToLower();
            EditorGUILayout.LabelField($"Relative Path {(checkPath.Contains("streamingassets")?"StreamingAssets":"Assets")}", relPath);
            
            if (GUILayout.Button("Browse for certificate file"))
            {
                try
                {
                    var startPath = Application.streamingAssetsPath;
                    if (!Directory.Exists(startPath))
                    {
                        startPath = Application.dataPath;
                    }
                    var path =
                        EditorUtility.OpenFilePanel("Select Certificate", startPath, "pfx,crt,pem");
                    cert.absPath = path ?? "";
                }
                catch
                {
                    //
                }
            }

            cert.password = EditorGUILayout.PasswordField("Password", cert.password);

            try
            {
                var myCert = new X509Certificate2(cert.absPath, cert.password);
                EditorGUILayout.TextArea(myCert.ToString(), GUILayout.MaxWidth(350f));
                var validity = (myCert.NotAfter - DateTime.Today).Days;
                EditorGUILayout.LabelField("Validity (Days)", validity.ToString());
                EditorGUILayout.LabelField($"This is about {validity / 365f:F1} years.");

                EditorGUILayout.Space();
                EditorGUILayout.LabelField(
                    $"This Certificate contains {(myCert.HasPrivateKey ? "a" : "no")} private key.");
            }
            catch (CryptographicException e)
            {
                EditorGUILayout.LabelField("Error: ", e.Message);
                EditorGUILayout.LabelField("* The certificate could be invalid.");
                EditorGUILayout.LabelField("* The certificate's password could be incorrect.");
            }
            catch (FileNotFoundException e)
            {
                EditorGUILayout.LabelField(e.Message);
            }
            catch (ArgumentException)
            {
                EditorGUILayout.LabelField("Error: ", "Path is empty.");
            }
            catch (DirectoryNotFoundException)
            {
                EditorGUILayout.LabelField("Error: ", "Path does not (fully) exist.");
            }

            if (cert.absPath != null && !cert.absPath.Contains(certificateStreamingAssetsPath))
            {
                var targetPath = cert.GetRelativePath()!=null ? Path.Combine(certificateStreamingAssetsPath, Path.GetFileName(cert.GetRelativePath())): "";
                if (!File.Exists(targetPath))
                {
                    EditorGUILayout.LabelField("Certificates must be placed in StreamingAssets/certificates");
                    if (File.Exists(cert.absPath) && GUILayout.Button("Copy File to Streaming Assets now."))
                    {
                        if (!Directory.Exists(certificateStreamingAssetsPath))
                        {
                            Directory.CreateDirectory(certificateStreamingAssetsPath);

                        }

                        File.Copy(cert.absPath, targetPath);
                        cert.absPath = targetPath;
                        AssetDatabase.Refresh();
                    }
                }
                else
                {
                    Debug.Log($"Using already existing file StreamingAssets/certificates/{cert.GetRelativePath()}, if you want to replace it, remove it first.");
                    cert.absPath = targetPath;
                }
    
            }

            //base.OnInspectorGUI();
            EditorUtility.SetDirty(cert);
        }
    }
}