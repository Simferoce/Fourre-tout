﻿using UnityEngine;

namespace Game
{
    [CreateAssetMenu(fileName = "GainShieldPeriodicallyPerk", menuName = "Definition/Technology/Berserker/GainShieldPeriodicallyPerk")]
    public class GainShieldPeriodicallyPerk : CharacterTechnologyPerkDefinition
    {
        public class Modifier : Modifier<Modifier, GainShieldPeriodicallyPerk>
        {
            private float accumulatedTime = 0.0f;

            public Modifier(IModifiable modifiable, GainShieldPeriodicallyPerk modifierDefinition) : base(modifiable, modifierDefinition)
            {
            }

            public override void Update()
            {
                base.Update();

                if (modifiable.TryGetCachedComponent<Character>(out Character character) && character.IsActive && character.IsEngaged)
                {
                    if ((int)((accumulatedTime + Time.deltaTime) / definition.cooldown) != (int)(accumulatedTime / definition.cooldown))
                    {
                        modifiable.AddModifier(definition.shieldModifierDefinition.CreateShield(modifiable, definition.amount, definition.duration));
                    }

                    accumulatedTime += Time.deltaTime;
                }
            }
        }

        [SerializeField] private float amount;
        [SerializeField] private float duration;
        [SerializeField] private float cooldown;
        [SerializeField] private ShieldModifierDefinition shieldModifierDefinition;

        public override string ParseDescription()
        {
            return $"Gain a shield each {cooldown} seconds for {duration} seconds that absorbs for a total of {amount} damage.";
        }

        public override Game.Modifier GetModifier(IModifiable modifiable)
        {
            return new Modifier(modifiable, this);
        }
    }
}
