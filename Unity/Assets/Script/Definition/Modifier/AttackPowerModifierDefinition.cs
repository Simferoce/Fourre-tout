﻿using UnityEngine;

namespace Game
{
    [CreateAssetMenu(fileName = "AttackPowerModifierDefinition", menuName = "Definition/Modifier/AttackPowerModifierDefinition")]
    public class AttackPowerModifierDefinition : ModifierDefinition
    {
        public class AttackPowerBuff : Modifier<AttackPowerBuff>
        {
            private float amount;

            public override float? AttackPower => amount;

            public AttackPowerBuff(IModifiable modifiable, ModifierDefinition modifierDefinition, float amount) : base(modifiable, modifierDefinition)
            {
                this.amount = amount;
            }
        }
    }
}
