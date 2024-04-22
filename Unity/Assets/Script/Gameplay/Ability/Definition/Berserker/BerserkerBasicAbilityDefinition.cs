﻿using UnityEngine;

namespace Game
{
    [CreateAssetMenu(fileName = "BerserkerBasicAbilityDefinition", menuName = "Definition/Ability/Berserker/BerserkerBasicAbilityDefinition")]
    public class BerserkerBasicAbilityDefinition : AbilityDefinition
    {
        [Header("Range")]
        [SerializeField, Range(0, 5)] private float reachPercentage;

        [Header("Damage")]
        [SerializeField, Range(0, 5)] private float damagePercentage;

        [Statistic("damage")] public float Damage(Ability ability) => ability.Character.AttackPower * damagePercentage;
        [Statistic("range")] public float Range(Ability ability) => ability.Character.Reach * reachPercentage;
    }
}
