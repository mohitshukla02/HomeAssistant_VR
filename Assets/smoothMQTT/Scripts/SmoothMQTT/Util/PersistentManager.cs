using UnityEngine;

namespace SmoothMQTT.Util
{
    public class PersistentManager : MonoBehaviour
    {
        [SerializeField] private bool isPersistent = true;
        private static GameObject assignedObject;

        void Awake()
        {
            if (isPersistent)
            {
                if (assignedObject == null)
                {
                    DontDestroyOnLoad(gameObject);
                    assignedObject = gameObject;
                }
                else if (assignedObject != gameObject)
                {
                    Destroy(gameObject);
                }
            }

        }
    }
}