using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class Base : AgentObject, ITargeteable, IModifiable, IAttackable
    {
        [SerializeField] private float health;
        [SerializeField] private float maxHealth;
        [SerializeField] private SpawnPoint spawnPoint;
        [SerializeField] private Transform targetPosition;

        public event Action<DamageSource, IAttackable> OnDamageTaken;

        public SpawnPoint SpawnPoint { get => spawnPoint; set => spawnPoint = value; }
        public Faction Faction => Agent.Faction;
        public int Priority => int.MaxValue;
        public float MaxHealth { get => maxHealth; set => maxHealth = value; }
        public float Health { get => health; set => health = value; }
        public Vector3 Position => transform.position;
        public ModifierHandler ModifierHandler { get; set; } = new ModifierHandler();

        protected override void Awake()
        {
            health = maxHealth;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            ModifierHandler.Dispose();
        }

        public bool Attackable()
        {
            return this.health > 0;
        }

        public bool CanBlocks(Faction faction)
        {
            return faction != this.Faction;
        }

        public void TakeAttack(DamageSource source, float damage)
        {
            health -= damage;

            OnDamageTaken?.Invoke(source, this);
        }

        public void AddModifier(Modifier modifier)
        {
            ModifierHandler.Add(modifier);
        }

        public void RemoveModifier(Modifier modifier)
        {
            ModifierHandler.Remove(modifier);
        }

        public List<Modifier> GetModifiers()
        {
            return ModifierHandler.Modifiers;
        }
    }
}

