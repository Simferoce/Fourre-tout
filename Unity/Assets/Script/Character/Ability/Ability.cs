﻿using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public abstract class Ability : MonoBehaviour
    {
        [StatisticResolve("caster")]
        public Character Character { get; set; }
        [StatisticResolve("base")]
        public AbilityDefinition Definition { get; set; }

        public bool IsCasting { get; set; }
        public virtual bool IsActive => IsCasting;
        public virtual List<IAttackable> Targets => new List<IAttackable>();

        [SerializeField] private StatisticHolder statistics;
        [StatisticResolve("statistics")] public StatisticHolder Statistics => statistics;

        public virtual void Initialize(Character character)
        {
            this.Character = character;
        }

        public abstract void Dispose();

        public abstract void Update();

        public abstract bool CanUse();

        public abstract void Use();

        public abstract void Interrupt();
    }
}
