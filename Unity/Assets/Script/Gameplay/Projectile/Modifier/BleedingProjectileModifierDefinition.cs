﻿using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Game
{
    [CreateAssetMenu(fileName = "BleedingProjectileModifierDefinition", menuName = "Definition/Modifier/Projectile/BleedingProjectileModifierDefinition")]
    public class BleedingProjectileModifierDefinition : ModifierDefinition
    {
        public class Modifier : Modifier<Modifier, BleedingProjectileModifierDefinition>
        {
            private Projectile projectile;
            private float damagePerSeconds;
            private float duration;

            public Modifier(ModifierHandler modifiable, BleedingProjectileModifierDefinition modifierDefinition, float damagePerSeconds, float duration, IModifierSource modifierSource) : base(modifiable, modifierDefinition, modifierSource)
            {
                this.damagePerSeconds = damagePerSeconds;
                this.duration = duration;

                this.projectile = modifiable.Entity.GetCachedComponent<Projectile>();
                projectile.OnImpacted += Projectile_OnImpacted;
            }

            private void Projectile_OnImpacted(List<ITargeteable> targeteables)
            {
                foreach (ITargeteable targeteable in targeteables)
                {
                    if ((targeteable as Entity).TryGetCachedComponent<ModifierHandler>(out ModifierHandler modifiable))
                    {
                        BleedingModifierDefinition.BleedingModifier modifier = modifiable.GetModifiers()
                            .FirstOrDefault(x => x is BleedingModifierDefinition.BleedingModifier bleedingModifier
                                && bleedingModifier.Source == (object)projectile.AgentObject)
                            as BleedingModifierDefinition.BleedingModifier;

                        if (modifier == null)
                        {
                            modifier = new BleedingModifierDefinition.BleedingModifier(modifiable, definition.bleedingModifierDefinition, duration, damagePerSeconds, projectile.AgentObject as Character);

                            modifiable.AddModifier(modifier);
                        }
                        else
                        {
                            SpreadBleedingPerk.Modifier spreadModifier = projectile.AgentObject.GetCachedComponent<ModifierHandler>().GetModifiers().FirstOrDefault(x => x is SpreadBleedingPerk.Modifier) as SpreadBleedingPerk.Modifier;
                            modifier.Increase(damagePerSeconds, duration, spreadModifier != null, spreadModifier?.SpreadDistance ?? 0f, projectile.AgentObject as Character);
                        }
                    }
                }
            }

            public override void Dispose()
            {
                base.Dispose();
                projectile.OnImpacted -= Projectile_OnImpacted;
            }
        }

        [SerializeField] private BleedingModifierDefinition bleedingModifierDefinition;

        public BleedingModifierDefinition BleedingModifierDefinition { get => bleedingModifierDefinition; set => bleedingModifierDefinition = value; }
    }
}
