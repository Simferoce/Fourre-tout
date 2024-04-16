﻿using UnityEngine;

namespace Game
{
    [CreateAssetMenu(fileName = "HealOnShieldEndPerk", menuName = "Definition/Technology/Berserker/HealOnShieldEndPerk")]
    public class HealOnShieldEndPerk : CharacterTechnologyPerkDefinition
    {
        public class Modifier : Modifier<Modifier>
        {
            private float healPerShieldPoint;

            public Modifier(IModifiable modifiable, ModifierDefinition modifierDefinition, float healPerShieldPoint) : base(modifiable, modifierDefinition)
            {
                if (modifiable.TryGetCachedComponent<IShieldable>(out IShieldable shieldable))
                {
                    shieldable.OnShieldableDestroyed += Shieldable_OnDestroyed;
                    shieldable.OnShieldBroken += Shieldable_OnShieldBroken;
                }

                this.healPerShieldPoint = healPerShieldPoint;
            }

            private void Shieldable_OnShieldBroken(Shield shield)
            {
                if (modifiable is not Character character)
                    return;

                float heal = healPerShieldPoint * shield.Remaining;
                if (heal <= 0)
                    return;

                character.Heal(heal);
            }

            private void Shieldable_OnDestroyed(IShieldable shieldable)
            {
                shieldable.OnShieldableDestroyed -= Shieldable_OnDestroyed;
                shieldable.OnShieldBroken -= Shieldable_OnShieldBroken;
            }

            public override void Dispose()
            {
                base.Dispose();

                if (modifiable.TryGetCachedComponent<IShieldable>(out IShieldable shieldable))
                {
                    shieldable.OnShieldBroken -= Shieldable_OnShieldBroken;
                    shieldable.OnShieldableDestroyed -= Shieldable_OnDestroyed;
                }
            }
        }

        [SerializeField] private float healPerShieldPoint;

        public override Game.Modifier GetModifier(IModifiable modifiable)
        {
            return new Modifier(modifiable, this, healPerShieldPoint);
        }
    }
}
