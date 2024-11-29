﻿using UnityEngine;

namespace Game
{
    [CreateAssetMenu(fileName = "AttackSpeedBaseOnStaggerAppliedPerk", menuName = "Definition/Technology/Archer/AttackSpeedBaseOnStaggerAppliedPerk")]
    public class AttackSpeedBaseOnStaggerAppliedPerk : CharacterTechnologyPerkDefinition
    {
        public class Modifier : Modifier<Modifier, AttackSpeedBaseOnStaggerAppliedPerk>
        {
            private int amountOfStaggerApplied = 0;
            private Statistic<float> attackSpeedPercentage;

            public Modifier(ModifierHandler modifiable, AttackSpeedBaseOnStaggerAppliedPerk modifierDefinition, IModifierSource source) : base(modifiable, modifierDefinition, source)
            {
                modifiable.Entity.GetCachedComponent<IModifierSource>().OnModifierAdded += Modifier_OnModifierAdded;
                attackSpeedPercentage = new Statistic<float>(StatisticDefinition.PercentageAttackSpeed);
            }

            public override float? GetStack()
            {
                return amountOfStaggerApplied;
            }

            private void Modifier_OnModifierAdded(Game.Modifier modifier)
            {
                if (modifier is StaggerModifierDefinition.Modifier)
                    amountOfStaggerApplied++;

                attackSpeedPercentage.SetValue(amountOfStaggerApplied * definition.attackSpeedByStaggerApplied);
            }

            public override void Dispose()
            {
                base.Dispose();
                modifiable.Entity.GetCachedComponent<IModifierSource>().OnModifierAdded -= Modifier_OnModifierAdded;
            }
        }

        [SerializeField, Range(0, 5)] private float attackSpeedByStaggerApplied;

        public override string ParseDescription()
        {
            return string.Format(Description, attackSpeedByStaggerApplied);
        }

        public override Game.Modifier GetModifier(ModifierHandler modifiable)
        {
            return new Modifier(modifiable, this, modifiable.Entity.GetCachedComponent<IModifierSource>());
        }
    }
}
