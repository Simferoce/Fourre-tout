﻿using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Game
{
    [Serializable]
    public class ProjectileAbilityEffect : AbilityEffect
    {
        [SerializeField] private GameObject projectilePrefab;
        [SerializeReference, SubclassSelector] private ProjectileAbilityEffectOrigin origin;

        [Space]
        [SerializeReference, SubclassSelector] private List<ProjectileFactoryContext> datas = new List<ProjectileFactoryContext>();

        public override void Apply()
        {
            GameObject gameObject = GameObject.Instantiate(projectilePrefab, origin.GetPosition(this), Quaternion.identity);
            Projectile projectile = gameObject.GetComponent<Projectile>();

            ProjectileTargetContext targetContext = new ProjectileTargetContext() { Target = Ability.Targets[0] };
            projectile.Initialize(Ability.Character, datas.Select(x => x.GetContext(Ability.Character)).Append(targetContext).ToList());
        }
    }
}
