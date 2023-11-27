using System;
using System.IO;
using UnityEngine;

namespace SmoothMQTT.Core
{
    
    [CreateAssetMenu(fileName = "my_certificate", menuName = "SmoothMQTT/Certificate", order = 0)]
    [HelpURL("https://smoothmqtt.schliesky.com/docs/next/user-guide/certificates#certificate-assets")]
    public class ScriptableCertificate : ScriptableObject
    {
        public string absPath;
        public string password;
        public string GetRelativePath()
        {
            if (absPath == null)
            {
                return null;
            }

            var relativePath = absPath.Equals(String.Empty)
                ? ""
                : Path.GetFullPath(absPath).Replace(Path.GetFullPath(Application.streamingAssetsPath), "");
            relativePath = relativePath.TrimStart('\\');
            if (Path.IsPathRooted(relativePath))
            {
                relativePath = Path.GetFullPath(absPath).Replace(Path.GetFullPath(Application.dataPath), "").TrimStart('\\');
            }
            relativePath = relativePath.TrimStart('/');
            if (relativePath.Equals(String.Empty))
            {
                return null;
            }

            return relativePath;
        }
    }
}