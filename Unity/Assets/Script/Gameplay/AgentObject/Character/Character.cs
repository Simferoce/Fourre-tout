﻿using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Game
{
    public partial class Character : AgentObject<CharacterDefinition>, IDisplaceable, IStaggerable
    {
        [Header("Collision")]
        [SerializeField] private new Rigidbody2D rigidbody;

        public CharacterAnimator CharacterAnimator { get; set; }

        private StateMachine stateMachine = new StateMachine();

        public override void Spawn(Agent agent, int spawnNumber, int direction)
        {
            base.Spawn(agent, spawnNumber, direction);

            CharacterAnimator = GetComponentInChildren<CharacterAnimator>();

            stateMachine.Initialize(new MoveState(this));
            InitializeAbilities();
        }

        public void FixedUpdate()
        {
            if (IsDead)
                return;

            UpdateAbilities();

            stateMachine.Update();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            DisposeAbilities();
        }

        public ITargeteable GetTarget(TargetCriteria criteria, object caller)
        {
            return GetTargets(criteria, caller).FirstOrDefault();
        }

        public List<ITargeteable> GetTargets(TargetCriteria criteria, object caller)
        {
            List<ITargeteable> potentialTargets = new List<ITargeteable>();
            foreach (ITargeteable attackable in AgentObject.All.OfType<ITargeteable>())
            {
                if (!attackable.IsActive)
                    continue;

                if (!criteria.Execute(this, attackable, caller))
                    continue;

                potentialTargets.Add(attackable);
            }

            return potentialTargets
                .OrderBy(x => x.Priority)
                .ToList();
        }

        protected override void InternalDeath()
        {
            stateMachine.SetState(new DeathState(this));
        }

        public void Displace(Vector2 displacement)
        {
            rigidbody.MovePosition(this.rigidbody.position + displacement);
        }

        public void Stagger(float duration)
        {
            foreach (Ability ability in abilities)
            {
                if (ability.IsActive)
                    ability.Interrupt();
            }

            stateMachine.SetState(new StaggerState(this, duration));
        }

        #region Statistic
        private TargetCriteria engagedCriteria = new IsEnemyTargetCriteria();

        public override float MaxHealth { get => Definition.MaxHealth + GetModifiers().Where(x => x.MaxHealth.HasValue).Sum(x => x.MaxHealth.Value); }
        public override float Defense { get => Definition.Defense + GetModifiers().Where(x => x.Defense.HasValue).Sum(x => x.Defense.Value); }
        public override float AttackSpeed { get => Definition.AttackSpeed; }
        public override float AttackPower { get => Definition.AttackPower + GetModifiers().Where(x => x.AttackPower.HasValue).Sum(x => x.AttackPower.Value); }
        public override float Speed { get => Definition.Speed * (1 + GetModifiers().Where(x => x.SpeedPercentage.HasValue).Sum(x => x.SpeedPercentage.Value)); }
        public override float Reach { get => Definition.Reach; }
        public override bool IsActive { get => !IsDead; }
        public override bool IsEngaged => GetTarget(engagedCriteria, this) != null;
        public override bool IsInvulnerable => GetModifiers().Where(x => x.Invulnerable.HasValue).Any(x => x.Invulnerable.Value);
        public override bool IsDead { get => stateMachine.Current is DeathState; }

        public override bool TryGetStatisticValue<T>(StatisticDefinition statisticDefinition, StatisticType statisticType, out T value)
        {
            if (statisticDefinition == StatisticDefinition.MaxHealth)
            {
                if (statisticType == StatisticType.Base)
                    value = (T)(object)Definition.MaxHealth;
                else if (statisticType == StatisticType.Modified)
                    value = (T)(object)(MaxHealth - Definition.MaxHealth);
                else
                    value = (T)(object)MaxHealth;

                return true;
            }
            else if (statisticDefinition == StatisticDefinition.Defense)
            {
                if (statisticType == StatisticType.Base)
                    value = (T)(object)Definition.Defense;
                else if (statisticType == StatisticType.Modified)
                    value = (T)(object)(Defense - Definition.Defense);
                else
                    value = (T)(object)Defense;

                return true;
            }
            else if (statisticDefinition == StatisticDefinition.AttackPower)
            {
                if (statisticType == StatisticType.Base)
                    value = (T)(object)Definition.AttackPower;
                else if (statisticType == StatisticType.Modified)
                    value = (T)(object)(AttackPower - Definition.AttackPower);
                else
                    value = (T)(object)AttackPower;

                return true;
            }
            else if (statisticDefinition == StatisticDefinition.AttackSpeed)
            {
                if (statisticType == StatisticType.Base)
                    value = (T)(object)Definition.AttackSpeed;
                else if (statisticType == StatisticType.Modified)
                    value = (T)(object)(AttackSpeed - Definition.AttackSpeed);
                else
                    value = (T)(object)AttackSpeed;

                return true;
            }
            else if (statisticDefinition == StatisticDefinition.Speed)
            {
                if (statisticType == StatisticType.Base)
                    value = (T)(object)Definition.Speed;
                else if (statisticType == StatisticType.Modified)
                    value = (T)(object)(Speed - Definition.Speed);
                else
                    value = (T)(object)Speed;

                return true;
            }
            else if (statisticDefinition == StatisticDefinition.Reach)
            {
                if (statisticType == StatisticType.Base)
                    value = (T)(object)Definition.Reach;
                else if (statisticType == StatisticType.Modified)
                    value = (T)(object)(Reach - Definition.Reach);
                else
                    value = (T)(object)Reach;

                return true;
            }

            return base.TryGetStatisticValue<T>(statisticDefinition, statisticType, out value);
        }

        #endregion

        #region Ability
        [Header("Abilities")]
        [SerializeField] private List<AbilityDefinition> abilitiesDefinition = new List<AbilityDefinition>();

        private List<Ability> abilities = new List<Ability>();

        public List<Ability> Abilities { get => abilities; set => abilities = value; }
        public float LastAbilityUsed { get; set; }

        private void InitializeAbilities()
        {
            GameObject abilitiesParent = new GameObject("Abilities");
            abilitiesParent.transform.parent = transform;

            foreach (AbilityDefinition definition in abilitiesDefinition)
            {
                Ability ability = definition.GetAbility();
                ability.transform.parent = abilitiesParent.transform;
                ability.Initialize(this);

                abilities.Add(ability);
            }
        }

        private void UpdateAbilities()
        {
            foreach (Ability ability in abilities)
            {
                if (ability.IsActive)
                    ability.Tick();
            }
        }

        public Ability GetCurrentAbility()
        {
            return abilities.FirstOrDefault(a => a.IsActive);
        }

        public void Cast()
        {
            stateMachine.SetState(new CastingState(this));
        }

        public void EndCast()
        {
            stateMachine.SetState(new MoveState(this));
        }

        public bool CanUseAbility()
        {
            if (Health <= 0 || IsDead)
                return false;

            return true;
        }

        private void DisposeAbilities()
        {
            foreach (Ability ability in abilities)
                ability.Dispose();
        }

        #endregion
    }
}