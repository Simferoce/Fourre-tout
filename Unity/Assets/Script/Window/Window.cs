﻿using UnityEngine;

namespace Game
{
    public abstract class Window : MonoBehaviour
    {
        public virtual void Show()
        {
            this.gameObject.SetActive(true);
        }

        public virtual void Hide()
        {
            this.gameObject.SetActive(false);
        }
    }
}
