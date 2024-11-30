﻿using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Game
{
    [CreateAssetMenu(fileName = "CreateDamagePoolPerk", menuName = "Definition/Technology/Seer/CreateDamagePoolPerk")]
    public class CreateDamagePoolPerk : ModifierDefinition
    {
        public class Modifier : Modifier<Modifier, CreateDamagePoolPerk>, IProjectileModifier
        {
            private int currentAttackApplied = 0;
            private Ability affectedAbility;

            public bool HasModifier => currentAttackApplied >= definition.stack;

            public Modifier(CreateDamagePoolPerk modifierDefinition) : base(modifierDefinition)
            {

            }

            public override void Initialize(ModifierHandler modifiable, ModifierApplier source, List<ModifierParameter> parameters)
            {
                base.Initialize(modifiable, source, parameters);
                affectedAbility = modifiable.Entity.GetCachedComponent<Caster>().Abilities.FirstOrDefault(x => x.Definition == definition.affectedAbility);
                affectedAbility.OnAbilityEffectApplied += AffectedAbility_OnAbilityEffectApplied;
            }

            private void AffectedAbility_OnAbilityEffectApplied()
            {
                if (currentAttackApplied < definition.stack)
                    currentAttackApplied++;
            }

            public override float? GetStack()
            {
                return currentAttackApplied;
            }

            public override void Dispose()
            {
                base.Dispose();
                affectedAbility.OnAbilityEffectApplied -= AffectedAbility_OnAbilityEffectApplied;
            }

            public Game.Modifier GetModifier(Projectile projectile)
            {
                currentAttackApplied = 0;
                return new PoolProjectileModifierDefinition.Modifier(projectile.GetCachedComponent<ModifierHandler>(), definition.projectileModifierDefinition, definition.duration, definition.damage);
            }
        }

        [SerializeField] private int stack;
        [SerializeField] private float duration;
        [SerializeField] private float damage;
        [SerializeField] private PoolProjectileModifierDefinition projectileModifierDefinition;
        [SerializeField] private AbilityDefinition affectedAbility;

        public override string ParseDescription()
        {
            return string.Format(Description, stack, damage, duration);
        }

        public override Game.Modifier Instantiate()
        {
            return new Modifier(this);
        }
    }
}
