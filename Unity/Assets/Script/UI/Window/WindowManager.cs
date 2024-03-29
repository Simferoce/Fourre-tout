﻿using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Game
{
    public class WindowManager : MonoBehaviour
    {
        public static WindowManager Instance;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void Init()
        {
            Instance = null;
        }

        private List<Window> windows = new List<Window>();

        private void Awake()
        {
            Instance = this;
            windows = GetComponentsInChildren<Window>(true).ToList();

            foreach (Window window in windows)
                window.Hide();
        }

        public T GetWindow<T>()
            where T : Window
        {
            T window = windows.OfType<T>().FirstOrDefault();

            return window;
        }
    }
}
