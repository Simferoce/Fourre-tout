﻿namespace Game
{
    public interface IHealable
    {
        public float MaxHealth { get; }
        public float Health { get; }
        public void Heal(float amount);
    }
}