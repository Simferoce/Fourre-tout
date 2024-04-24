﻿using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Game
{
    [CreateAssetMenu(fileName = "BleedingProjectileModifierDefinition", menuName = "Definition/Modifier/BleedingProjectileModifierDefinition")]
    public class BleedingProjectileModifierDefinition : ModifierDefinition
    {
        public class Modifier : Modifier<Modifier, BleedingProjectileModifierDefinition>
        {
            private Projectile projectile;
            private float damagePerSeconds;
            private float duration;

            public Modifier(IModifiable modifiable, BleedingProjectileModifierDefinition modifierDefinition, float damagePerSeconds, float duration) : base(modifiable, modifierDefinition)
            {
                this.damagePerSeconds = damagePerSeconds;
                this.duration = duration;

                this.projectile = modifiable.GetCachedComponent<Projectile>();
                projectile.OnImpacted += Projectile_OnImpacted;
            }

            private void Projectile_OnImpacted(List<ITargeteable> targeteables)
            {
                foreach (ITargeteable targeteable in targeteables)
                {
                    if (targeteable.TryGetCachedComponent<IModifiable>(out IModifiable modifiable))
                    {
                        BleedingModifierDefinition.Modifier modifier = modifiable.GetModifiers()
                            .FirstOrDefault(x => x is BleedingModifierDefinition.Modifier bleedingModifier
                                && bleedingModifier.Source == (object)projectile.Character)
                            as BleedingModifierDefinition.Modifier;

                        if (modifier == null)
                        {
                            modifier = new BleedingModifierDefinition.Modifier(modifiable, definition.bleedingModifierDefinition, projectile.Character, duration);
                            modifier.DamagePerSeconds += damagePerSeconds;

                            modifiable.AddModifier(modifier);
                        }
                        else
                        {
                            modifier.Increase(damagePerSeconds);
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
    }
}
