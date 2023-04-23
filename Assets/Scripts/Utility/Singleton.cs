using System;
using UnityEngine;

namespace Utility
{
    public abstract class Singleton<T> where T : new()
    {
        private static T instance;

        public static T Instance
        {
            get
            {
                if (instance==null)
                {
                    Type t = typeof(T);
                    instance = new T();
                    if (instance==null)
                    {
                        //クラスが見つからないときエラー
                        Debug.LogError(t + " is nothing.");
                    }
                }

                return instance;
            }
        }
    }
}