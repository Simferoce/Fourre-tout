﻿using UnityEngine;

namespace Extension
{
    public static class GameObjectExtension
    {
        public static bool TryGetComponentInParent<T>(this GameObject gameObject, out T component)
        {
            component = gameObject.GetComponentInParent<T>();
            return component != null;
        }
    }
}
