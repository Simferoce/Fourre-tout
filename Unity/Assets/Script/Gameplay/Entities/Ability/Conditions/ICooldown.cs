﻿namespace Game.Ability
{
    public interface ICooldown
    {
        public float Remaining { get; }
        public float Total { get; }
    }
}
