﻿using Game.Components;
using Game.Statistics;
using System;
using UnityEngine;

namespace Game.Ability
{
    [Serializable]
    public class DistanceAbilityTargetFilter : AbilityTargetFilter
    {
        [SerializeField] private StatisticReference<float> distance;

        public override void Initialize(AbilityEntity ability)
        {
            base.Initialize(ability);
        }

        public override bool Execute(AbilityEntity source, Entity targetEntity)
        {
            Target target = targetEntity.GetCachedComponent<Target>();
            float distanceValue = distance?.Resolve(source).GetValue(null) ?? 0f;
            return Mathf.Abs((target.ClosestPoint(source.Caster.transform.position) - source.Caster.transform.position).x) < distanceValue;
        }
    }
}
