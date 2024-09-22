﻿using System;
using UnityEngine;

namespace Game
{
    [Serializable]
    public class CooldownCondition : AbilityCondition
    {
        [SerializeField] private Statistic cooldown;

        private float lastUsed = 0f;

        public override void Initialize(Ability ability)
        {
            base.Initialize(ability);
            lastUsed = float.MinValue;
        }

        public override bool Execute()
        {
            return Time.time - lastUsed > cooldown;
        }

        public override void OnAbilityEnded()
        {
            lastUsed = Time.time;
        }
    }
}
