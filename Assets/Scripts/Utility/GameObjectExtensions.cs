using System.Collections.Generic;
using UnityEngine;

namespace Utility
{
    public static class GameObjectExtensions
    {
        public static T FindObjectOfInterface<T>() where T:class
        {
            foreach (var n in GameObject.FindObjectsOfType<Component>())
            {
                if (n is T component)
                {
                    return component;
                }
            }

            return null;
        }
        
        public static T[] FindObjectsOfInterface<T>() where T:class
        {
            var components = new List<T>();
            foreach (var n in GameObject.FindObjectsOfType<Component>())
            {
                if (n is T component)
                {
                    components.Add(component);
                }
            }
            return components.ToArray();
        }
    }
}