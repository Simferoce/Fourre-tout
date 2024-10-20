﻿using System;

namespace Game.Projectile
{
    [Serializable]
    public abstract class ProjectileBehaviour : IDisposable
    {
        protected ProjectileEntity projectile;

        public virtual void Initialize(ProjectileEntity projectile)
        {
            this.projectile = projectile;
        }

        public virtual bool Validate(ProjectileEntity projectile) { return false; }

        public virtual void Update() { }

        public virtual void Dispose()
        {

        }
    }
}
