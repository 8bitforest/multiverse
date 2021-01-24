using UnityEngine;

namespace Multiverse.Utils
{
    public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static T _instance;

        // ReSharper disable StaticMemberInGenericType
        private static readonly object Mutex = new object();
        // ReSharper restore StaticMemberInGenericType

        public static T I
        {
            get
            {
                lock (Mutex)
                {
                    if (_instance != null)
                        return _instance;

                    _instance = FindObjectOfType<T>();
                    if (_instance != null)
                        return _instance;

                    Debug.LogWarning($"Singleton {typeof(T)} doesn't exist!");
                    return _instance;
                }
            }
        }
    }
}