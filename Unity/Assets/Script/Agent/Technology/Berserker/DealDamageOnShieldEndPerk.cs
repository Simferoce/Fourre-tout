﻿using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    [CreateAssetMenu(fileName = "DealDamageOnShieldEndPerk", menuName = "Definition/Technology/Berserker/DealDamageOnShieldEndPerk")]
    public class DealDamageOnShieldEndPerk : CharacterTechnologyPerkDefinition
    {
        public class Modifier : Modifier<Modifier>, IAttackSource
        {
            private float percentageAttackPower;
            private float percentageReach;

            public Modifier(IModifiable modifiable, float percentageAttackPower, float percentageReach) : base(modifiable)
            {
                if (modifiable is IShieldable shieldable)
                {
                    shieldable.OnDeath += Shieldable_OnDeath;
                    shieldable.OnShieldBroken += Shieldable_OnShieldBroken;
                }

                this.percentageAttackPower = percentageAttackPower;
                this.percentageReach = percentageReach;
            }

            private void Shieldable_OnShieldBroken(Shield shield)
            {
                if (modifiable is not Character character)
                    return;

                float damage = percentageAttackPower * character.AttackPower * (1 - (shield.Remaining / shield.Initial));
                if (damage <= 0)
                    return;

                foreach (AgentObject agent in AgentObject.All)
                {
                    if (agent == character)
                        continue;

                    if (agent is not IAttackable attackable)
                        continue;

                    if (!agent.IsActive)
                        continue;

                    if (Mathf.Abs((attackable.ClosestPoint(character.CenterPosition) - character.CenterPosition).x) > character.Reach * percentageReach)
                        continue;

                    Attack attack = new Attack(new AttackSource(new List<IAttackSource>() { character, this }), damage, 0, 0);
                    attackable.TakeAttack(attack);
                }
            }

            private void Shieldable_OnDeath(ITargeteable targeteable)
            {
                if (modifiable is IShieldable shieldable)
                {
                    shieldable.OnDeath -= Shieldable_OnDeath;
                    shieldable.OnShieldBroken -= Shieldable_OnShieldBroken;
                }
            }

            public override void Dispose()
            {
                base.Dispose();

                if (modifiable is IShieldable shieldable)
                {
                    shieldable.OnShieldBroken -= Shieldable_OnShieldBroken;
                    shieldable.OnDeath -= Shieldable_OnDeath;
                }
            }
        }

        [SerializeField, Range(0, 3)] private float percentageAttackPower;
        [SerializeField, Range(0, 3)] private float percentageReach;

        public override Game.Modifier GetModifier(IModifiable modifiable)
        {
            return new Modifier(modifiable, percentageAttackPower, percentageReach);
        }
    }
}
