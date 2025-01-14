﻿using Game.Agent;
using Game.Character;
using Game.Components;
using Game.Modifier;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Game.Ability
{
    [Serializable]
    public class DisplaceAbilityEffect : AbilityEffect, IContinousAbilityEffect
    {
        [SerializeField] private float destinationDistance;
        [SerializeField] private float translationDuration = 0.3f;
        [SerializeField, Range(0, 1)] private float damping = 0.05f;
        [SerializeField] private ModifierDefinition modifierDefinition;

        private float startedAt;
        private Vector3 destination;

        public override void Initialize(AbilityEntity ability)
        {
            base.Initialize(ability);
        }

        public override bool Validate()
        {
            bool changed = base.Validate();
            return changed;
        }

        public override void Apply()
        {
            destination = Ability.Caster.Entity.transform.position;
            startedAt = Time.time;
        }

        public bool Update(Caster caster)
        {
            Vector3 offset = caster.Entity.GetCachedComponent<AgentIdentity>().Direction * destinationDistance * Vector3.right;

            List<CharacterEntity> targets = Ability.Targets.Select(x => x.Entity).OfType<CharacterEntity>().ToList();
            for (int i = 0; i < targets.Count; i++)
            {
                CharacterEntity character = targets[i];
                Target target = character.GetCachedComponent<Target>();
                character.Displace(Vector3.Lerp(target.CenterPosition, destination + offset * (0.5f + (Mathf.Clamp01(i / (float)3)) * 0.5f), damping) - target.CenterPosition);
            }

            return Time.time - startedAt > translationDuration;
        }
    }
}
