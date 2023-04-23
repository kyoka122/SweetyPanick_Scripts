using System;
using UnityEngine;

namespace Utility
{
    public abstract class SingletonMonoBehaviour<T> : MonoBehaviour where T : MonoBehaviour
    {
        protected abstract bool IsDontDestroyOnLoad { get; }
        private static T instance;
        protected bool isDuplication { get; private set; }

        public static T Instance
        {
            get
            {
                if (!instance)
                {
                    Type t = typeof(T);
                    instance = (T) FindObjectOfType(t);
                    if (!instance)
                    {
                        //クラスが見つからないときエラー
                        Debug.LogError(t + " is nothing.");
                    }
                }

                return instance;
            }
        }

        protected virtual void Awake()
        {
            if (this != Instance)
            {
                isDuplication = true;
                Destroy(gameObject);
                return;
            }

            if (IsDontDestroyOnLoad)
            {
                transform.parent = null;
                DontDestroyOnLoad(gameObject);
            }
        }
    }
}