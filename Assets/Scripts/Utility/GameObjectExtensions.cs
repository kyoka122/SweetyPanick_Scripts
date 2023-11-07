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
        
        public static T[] FindInterface<T>() where T:class
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
        
        public static GameObject[] FindObjectsOfInterface<T>() where T:class
        {
            var gameObjects = new List<GameObject>();
            foreach (var n in GameObject.FindObjectsOfType<Component>())
            {
                if (n is T && !gameObjects.Contains(n.gameObject))
                {
                    gameObjects.Add(n.gameObject);
                }
            }
            return gameObjects.ToArray();
        }
    }
}