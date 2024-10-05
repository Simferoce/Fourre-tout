﻿using System;

namespace Game
{
    [Serializable]
    public abstract class ModifierBehaviour : IDisposable
    {
        protected Modifier modifier;

        public virtual void Initialize(Modifier modifier)
        {
            this.modifier = modifier;
        }

        public virtual void Update() { }
        public virtual void Refresh() { }
        public virtual void Dispose() { }
    }
}