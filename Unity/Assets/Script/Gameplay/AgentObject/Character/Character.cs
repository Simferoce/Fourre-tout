﻿using Assets.Script.Agent.Technology;
using Extension;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Game
{
    [RequireComponent(typeof(Attackable))]
    [RequireComponent(typeof(Target))]
    public partial class Character : AgentObject<CharacterDefinition>, IAttackSource, IAttackableOwner, ITargetOwner, IModifierSource
    {
        [Header("Collision")]
        [SerializeField] private new Rigidbody2D rigidbody;

        public event AttackedLanded OnAttackLanded;
        public event Action OnDeath;
        public event Action<Modifier> OnModifierAdded;

        public CharacterAnimator CharacterAnimator { get; set; }
        public List<TransformTag> TransformTags { get; set; }
        public List<Modifier> AppliedModifiers { get; set; } = new List<Modifier>();
        public HashSet<IAttackable> RecentlyAttackedAttackeables { get; set; } = new HashSet<IAttackable>();
        public override bool IsActive { get => !IsDead; }
        public Vector3 CenterPosition { get => this.GetCachedComponent<ITargeteable>().CenterPosition; }

        public float Health { get; set; }
        public float MaxHealth { get => Definition.MaxHealth + GetCachedComponent<IModifiable>().GetModifiers().Where(x => x.MaxHealth.HasValue).Sum(x => x.MaxHealth.Value); }
        public float Defense { get => Definition.Defense + GetCachedComponent<IModifiable>().GetModifiers().Where(x => x.Defense.HasValue).Sum(x => x.Defense.Value); }
        public float AttackSpeed { get => Definition.AttackSpeed * (1 + GetCachedComponent<IModifiable>().GetModifiers().Where(x => x.AttackSpeedPercentage.HasValue).Sum(x => x.AttackSpeedPercentage.Value)); }
        public float AttackPower { get => Definition.AttackPower + GetCachedComponent<IModifiable>().GetModifiers().Where(x => x.AttackPower.HasValue).Sum(x => x.AttackPower.Value); }
        public float Speed { get => Definition.Speed * (1 + GetCachedComponent<IModifiable>().GetModifiers().Where(x => x.SpeedPercentage.HasValue).Sum(x => x.SpeedPercentage.Value)); }
        public float Reach { get => Definition.Reach * (1 + GetCachedComponent<IModifiable>().GetModifiers().Where(x => x.ReachPercentage.HasValue).Sum(x => x.ReachPercentage.Value)); }
        public float TechnologyGainPerSecond => Definition.TechnologyGainPerSecond;

        public bool IsEngaged => GetTarget(engagedCriteria, this) != null;
        public bool IsInvulnerable => GetCachedComponent<IModifiable>().GetModifiers().Where(x => x.IsInvulnerable.HasValue).Any(x => x.IsInvulnerable.Value);
        public bool IsConfused => GetCachedComponent<IModifiable>().GetModifiers().Where(x => x.IsConfused.HasValue).Any(x => x.IsConfused.Value);
        public bool IsDead { get => stateMachine.Current is DeathState; }
        public bool IsInjured { get => Health < MaxHealth; }

        private StateMachine stateMachine = new StateMachine();
        private TargetCriteria engagedCriteria = new IsEnemyTargetCriteria();

        protected override void Awake()
        {
            base.Awake();
            TransformTags = GetComponentsInChildren<TransformTag>().ToList();
        }

        public override void Spawn(Agent agent, int spawnNumber, int direction)
        {
            base.Spawn(agent, spawnNumber, direction);

            CharacterAnimator = GetComponentInChildren<CharacterAnimator>();

            InitializeAbilities();
            AgentObjectDefinition agentObjectDefinition = GetDefinition();
            List<ITechnologyModify> modifiers = agent.Technology.UnlockedPerks().OfType<ITechnologyModify>().ToList();
            foreach (ITechnologyModify modifier in modifiers)
            {
                if (modifier.Affect(agentObjectDefinition))
                    this.GetCachedComponent<IModifiable>().AddModifier(modifier.GetModifier(this.GetCachedComponent<IModifiable>()));
            }

            stateMachine.Initialize(new MoveState(this));
            this.Health = MaxHealth;
        }

        public void Update()
        {
            RecentlyAttackedAttackeables.RemoveWhere(x => x is UnityEngine.Object o && o == null);
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

        public void SetDirection()
        {
            transform.localScale = new Vector3(Mathf.Sign(IsConfused ? -Direction : Direction) * Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }

        public void AddModifier(Modifier modifier)
        {
            AppliedModifiers.Add(modifier);
            OnModifierAdded?.Invoke(modifier);
        }

        public void RemoveModifier(Modifier modifier)
        {
            AppliedModifiers.Remove(modifier);
        }

        public Attack GenerateAttack(float damage, float armorPenetration, float leach, bool ranged, bool overtime, bool reflectable, IAttackable target, params IAttackSource[] source)
        {
            bool empowered = false;

            List<Modifier> modifiers = GetCachedComponent<IModifiable>().GetModifiers();

            EmpoweredModifierDefinition.Modifier empowerment = modifiers.FirstOrDefault(x => x is EmpoweredModifierDefinition.Modifier) as EmpoweredModifierDefinition.Modifier;
            if (empowerment != null)
            {
                damage *= 1 + empowerment.PercentageDamageIncrease;
                empowerment.Consume();

                empowered = true;
            }

            if (modifiers.Count > 0)
            {
                float damageDealtReduction = modifiers.Max(x => x.DamageDealtReduction ?? 0);
                damage *= (1 - damageDealtReduction);
            }

            if (target.GetCachedComponent<IModifiable>().GetModifiers().Any(x => x is DamageDealtReductionModifierDefinition.Modifier))
            {
                damage += GetCachedComponent<IModifiable>().GetModifiers().Sum(x => x.DamageDealtAgainstWeak ?? 0);
            }

            return new Attack(new AttackSource(this).Add(source), damage, armorPenetration, leach, ranged, empowered, reflectable, overtime);
        }

        public ITargeteable GetTarget(TargetCriteria criteria, object caller)
        {
            return GetTargets(criteria, caller).FirstOrDefault();
        }

        public List<ITargeteable> GetTargets(TargetCriteria criteria, object caller)
        {
            List<ITargeteable> potentialTargets = new List<ITargeteable>();
            foreach (ITargeteable targetteable in AgentObject.All.Select(x => x.GetCachedComponent<ITargeteable>()).Where(x => x != null))
            {
                if (!targetteable.IsActive)
                    continue;

                if (targetteable == this.GetCachedComponent<ITargeteable>())
                    continue;

                if (!criteria.Execute(this.GetCachedComponent<ITargeteable>(), targetteable, caller, Faction, IsConfused ? targetteable.Faction.GetConfusedFaction() : targetteable.Faction))
                    continue;

                potentialTargets.Add(targetteable);
            }

            return potentialTargets
                .OrderBy(x => x.Priority)
                .ToList();
        }

        public void Death()
        {
            EventChannelDeath.Instance.Publish(new EventChannelDeath.Event() { AgentObject = this });
            OnDeath?.Invoke();
            stateMachine.SetState(new DeathState(this));

            GetCachedComponent<IModifiable>().Clear();
        }

        public void Displace(Vector2 displacement)
        {
            rigidbody.MovePosition(this.rigidbody.position + displacement);
        }

        public void Heal(float amount)
        {
            this.Health += amount;
            this.Health = Mathf.Clamp(Health, 0, MaxHealth);
        }

        public void AttackLanded(AttackResult attackResult)
        {
            RecentlyAttackedAttackeables.Add(attackResult.Target);
            Heal(attackResult.DamageTaken * attackResult.Attack.Leach);
            OnAttackLanded?.Invoke(attackResult);
        }

        public bool RecentlyAttacked(IAttackable attackable)
        {
            return RecentlyAttackedAttackeables.Contains(attackable);
        }

        #region Ability
        [Header("Abilities")]
        [SerializeField] private List<AbilityDefinition> abilitiesDefinition = new List<AbilityDefinition>();

        public event Action<Ability> OnAbilityUsed;

        public List<Ability> Abilities { get => abilities; set => abilities = value; }
        public float LastAbilityUsed { get; set; }

        private List<Ability> abilities = new List<Ability>();

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
            if (stateMachine.Next == null)
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