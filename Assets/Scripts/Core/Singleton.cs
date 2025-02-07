using UnityEngine;

namespace Mercop.Core
{
    public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static T instance;

        public static T Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = (T) FindObjectOfType(typeof(T));
                    if (instance == null)
                    {
                        instance = new GameObject(typeof(T).ToString()).AddComponent<T>();
                    }
                }

                return instance;
            }
        }

        protected virtual void Awake()
        {
            instance = this as T;
            DontDestroyOnLoad(instance.gameObject); //uncomment if not using from persistent common scene
            Debug.Log($"{typeof(T)} created.");
        }
    }
}