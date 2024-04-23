﻿using UnityEngine;

namespace Game
{
    [CreateAssetMenu(fileName = "HealOnShieldEndPerk", menuName = "Definition/Technology/Berserker/HealOnShieldEndPerk")]
    public class HealOnShieldEndPerk : CharacterTechnologyPerkDefinition
    {
        public class Modifier : Modifier<Modifier, HealOnShieldEndPerk>
        {
            public Modifier(IModifiable modifiable, HealOnShieldEndPerk modifierDefinition) : base(modifiable, modifierDefinition)
            {
                if (modifiable.TryGetCachedComponent<IShieldable>(out IShieldable shieldable))
                {
                    shieldable.OnShieldableDestroyed += Shieldable_OnDestroyed;
                }

                modifiable.ModifierRemoved += Modifiable_ModifierRemoved;
            }

            private void Modifiable_ModifierRemoved(Game.Modifier obj)
            {
                if (obj is not ShieldModifierDefinition.Shield shield)
                    return;

                if (!modifiable.TryGetCachedComponent<Character>(out Character character))
                    return;

                float heal = definition.healPerShieldPointRemaining * shield.Remaining;
                if (heal <= 0)
                    return;

                character.Heal(heal);
            }

            private void Shieldable_OnDestroyed(IShieldable shieldable)
            {
                shieldable.OnShieldableDestroyed -= Shieldable_OnDestroyed;
                modifiable.ModifierRemoved -= Modifiable_ModifierRemoved;
            }

            public override void Dispose()
            {
                base.Dispose();

                if (modifiable.TryGetCachedComponent<IShieldable>(out IShieldable shieldable))
                {
                    shieldable.OnShieldableDestroyed -= Shieldable_OnDestroyed;
                }

                modifiable.ModifierRemoved -= Modifiable_ModifierRemoved;
            }
        }

        [SerializeField] private float healPerShieldPointRemaining;

        public override string ParseDescription()
        {
            return string.Format(Description, healPerShieldPointRemaining);
        }

        public override Game.Modifier GetModifier(IModifiable modifiable)
        {
            return new Modifier(modifiable, this);
        }
    }
}
