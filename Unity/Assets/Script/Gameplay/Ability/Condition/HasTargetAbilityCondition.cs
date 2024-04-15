﻿using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    [Serializable]
    public class HasTargetAbilityCondition : AbilityCondition
    {
        [SerializeReference, SubclassSelector] private TargetCriteria criteria;
        [SerializeField] private int count = 1;

        public List<ITargeteable> Targets = new List<ITargeteable>();

        public override bool Execute()
        {
            Targets = ability.Character.GetTargets(criteria, ability);
            return Targets.Count >= count;
        }
    }
}