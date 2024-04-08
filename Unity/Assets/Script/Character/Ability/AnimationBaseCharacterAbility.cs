﻿using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Game
{
    [Serializable]
    public class AnimationBaseCharacterAbility : CharacterAbility
    {
        [SerializeReference, SerializeReferenceDropdown] private List<AbilityCondition> conditions = new List<AbilityCondition>();
        [SerializeField] private CharacterAnimatorParameter.Parameter parameter = CharacterAnimatorParameter.Parameter.Ability;
        [Space]
        [SerializeReference, SerializeReferenceDropdown] private List<AbilityEffect> effects = new List<AbilityEffect>();

        public override List<IAttackable> Targets => (conditions.FirstOrDefault(x => x is HasTargetAbilityCondition) as HasTargetAbilityCondition)?.Targets ?? base.Targets;
        public override bool IsActive { get => IsCasting || IsLingering; }
        public bool IsLingering { get; set; } = false;

        public AnimationBaseCharacterAbility() : base()
        {

        }

        public AnimationBaseCharacterAbility(AnimationBaseCharacterAbility other) : base(other)
        {
            conditions = other.conditions.Select(condition => condition.Clone()).ToList();
            effects = other.effects.Select(condition => condition.Clone()).ToList();
            parameter = other.parameter;
        }

        public override void Initialize(Character character)
        {
            base.Initialize(character);

            foreach (AbilityCondition condition in conditions)
                condition.Initialize(this);

            foreach (AbilityEffect effect in effects)
                effect.Initialize(this);
        }

        public override bool CanUse()
        {
            return conditions.All(x => x.Execute()) && effects.All(x => x.CanBeApplied()) && IsCasting == false && IsLingering == false;
        }

        public override void Use()
        {
            Character.Cast();
            Character.CharacterAnimator.SetTrigger(parameter);
            Character.LastAbilityUsed = Time.time;
            IsCasting = true;

            foreach (AbilityCondition condition in conditions)
                condition.OnAbilityStarted();

            foreach (AbilityEffect effect in effects)
                effect.OnAbilityStarted();

            Character.CharacterAnimator.OnAbilityUsed += OnAnimatorEventAbilityUsed;
            AnimatorEventChannel.Subscribe(Character.CharacterAnimator.Animator, AnimatorEventChannel.Event.OnExit, AnimatorEventChannel.Id.Ability, OnCastEnded);
        }

        public override void Update()
        {
            if (IsCasting == true)
                return;

            bool end = true;
            foreach (ILingeringAbilityEffect lingeringAbilityEffect in effects.OfType<ILingeringAbilityEffect>())
            {
                end &= lingeringAbilityEffect.Update(Character);
            }

            if (end)
                End();
        }

        protected void OnAnimatorEventAbilityUsed()
        {
            foreach (AbilityEffect effect in effects)
                effect.Apply();
        }

        private void OnCastEnded()
        {
            IsCasting = false;

            if (effects.OfType<ILingeringAbilityEffect>().Count() == 0)
                End();
            else
                IsLingering = true;

            Character.EndCast();
        }

        public override void Dispose()
        {
            Character.CharacterAnimator.OnAbilityUsed -= OnAnimatorEventAbilityUsed;
            AnimatorEventChannel.Unsubscribe(Character.CharacterAnimator.Animator, AnimatorEventChannel.Event.OnExit, AnimatorEventChannel.Id.Ability, OnCastEnded);
        }

        private void End()
        {
            IsLingering = false;

            foreach (AbilityCondition condition in conditions)
                condition.OnAbilityEnded();

            foreach (AbilityEffect effect in effects)
                effect.OnAbilityEnded();

            Dispose();
        }

        public override void Interrupt()
        {
            Character.EndCast();
            IsLingering = false;
            IsCasting = false;

            foreach (AbilityCondition condition in conditions)
                condition.Interrupt();

            foreach (AbilityEffect effect in effects)
                effect.Interrupt();

            Dispose();
        }

        public override CharacterAbility Clone()
        {
            return new AnimationBaseCharacterAbility(this);
        }
    }
}
