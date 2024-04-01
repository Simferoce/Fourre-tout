﻿using System;

namespace Game
{
    [Serializable]
    public abstract class AbilityEffect
    {
        protected Character character;

        public virtual void Initialize(Character character)
        {
            this.character = character;
        }

        public abstract void Apply();
        public virtual bool CanBeApplied() { return true; }
        public virtual void OnAbilityEnded() { }
        public virtual void OnAbilityStarted() { }
        public virtual void Interrupt() { }
    }
}
