﻿using UnityEngine;

namespace Game
{
    [CreateAssetMenu(fileName = "IncreaseDamageTakenWhenStaggerPerk", menuName = "Definition/Technology/Archer/IncreaseDamageTakenWhenStaggerPerk")]
    public class IncreaseDamageTakenWhenStaggerPerk : CharacterTechnologyPerkDefinition
    {
        public class Modifier : Modifier<Modifier, IncreaseDamageTakenWhenStaggerPerk>
        {
            public float IncreaseDamageTakenOfStaggered => definition.increaseDamageTakenOfStaggered;

            public Modifier(IModifiable modifiable, IncreaseDamageTakenWhenStaggerPerk modifierDefinition, IModifierSource source) : base(modifiable, modifierDefinition, source)
            {
            }
        }

        [SerializeField, Range(0, 5)] private float increaseDamageTakenOfStaggered;

        public override string ParseDescription()
        {
            return string.Format(Description, increaseDamageTakenOfStaggered);
        }

        public override Game.Modifier GetModifier(IModifiable modifiable)
        {
            return new Modifier(modifiable, this, modifiable.GetCachedComponent<IModifierSource>());
        }
    }
}