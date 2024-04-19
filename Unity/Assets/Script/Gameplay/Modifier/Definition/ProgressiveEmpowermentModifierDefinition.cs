﻿using System.Linq;
using UnityEngine;

namespace Game
{
    [CreateAssetMenu(fileName = "ProgressiveEmpowermentModifierDefinition", menuName = "Definition/Modifier/ProgressiveEmpowermentModifierDefinition")]
    public class ProgressiveEmpowermentModifierDefinition : ModifierDefinition
    {
        public class Modifier : Modifier<Modifier>
        {
            private int maxStack;
            private StackModifierElement stackModifierElement;

            public Modifier(IModifiable modifiable, ModifierDefinition modifierDefinition) : base(modifiable, modifierDefinition)
            {
                this.maxStack = this.GetValueOrThrow<int>(StatisticDefinition.Stack);

                stackModifierElement = new StackModifierElement();
                this.With(stackModifierElement);

                stackModifierElement.OnStackGained += StackModifierElement_OnStackGained;
                stackModifierElement.IncreaseStack();
            }

            private void StackModifierElement_OnStackGained(StackModifierElement stackModifierElement)
            {
                if (stackModifierElement.CurrentStack > maxStack)
                {
                    stackModifierElement.OnStackGained -= StackModifierElement_OnStackGained;

                    EmpoweredModifierDefinition empoweredModifierDefinition = (Definition as ProgressiveEmpowermentModifierDefinition).empoweredModifierDefinition;
                    Game.Modifier modifier = modifiable.GetModifiers().FirstOrDefault(x => x.Definition == empoweredModifierDefinition);
                    if (modifier == null)
                    {
                        modifiable.AddModifier(new EmpoweredModifierDefinition.Modifier(modifiable, empoweredModifierDefinition));
                    }
                    else
                    {
                        modifier.Refresh();
                    }

                    modifiable.RemoveModifier(this);
                }
            }
        }

        [SerializeField] private EmpoweredModifierDefinition empoweredModifierDefinition;

        public override string ParseDescription(object caller, string description)
        {
            description = base.ParseDescription(caller, description);
            description = empoweredModifierDefinition.ParseDescription(caller, description);

            return description;
        }
    }
}
