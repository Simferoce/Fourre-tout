﻿namespace Game.Projectile
{
    public abstract class ProjectileParameter
    {
        public virtual string Name { get; set; }

        protected ProjectileParameter(string name)
        {
            Name = name;
        }
    }

    public class ProjectileParameter<T> : ProjectileParameter
    {
        private T value;

        public ProjectileParameter(string name, T value) : base(name)
        {
            this.value = value;
        }

        public T GetValue()
        {
            return value;
        }
    }
}
