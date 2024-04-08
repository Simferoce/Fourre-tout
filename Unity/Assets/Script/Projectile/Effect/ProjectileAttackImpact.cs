﻿using Extension;
using System;
using System.Linq;
using UnityEngine;

namespace Game
{
    [Serializable]
    public class ProjectileAttackImpact : ProjectileImpact
    {
        public override bool Impact(GameObject collision)
        {
            ProjectileAttackData.Context attackContext = (ProjectileAttackData.Context)projectile.Contexts.FirstOrDefault(x => x is ProjectileAttackData.Context);
            ProjectileImpactData.Context targetContext = (ProjectileImpactData.Context)projectile.Contexts.FirstOrDefault(x => x is ProjectileImpactData.Context);

            if (collision.CompareTag(GameTag.HIT_BOX) &&
                collision.gameObject.TryGetComponentInParent<IAttackable>(out IAttackable attackable)
                && attackable.IsActive
                && targetContext.Criteria.Execute(projectile.Character, attackable, null))
            {
                attackContext.Attack.AttackSource.Sources.Add(projectile);
                attackable.TakeAttack(attackContext.Attack);

                return true;
            }
            else if (collision.gameObject.CompareTag(GameTag.GROUND))
            {
                return true;
            }

            return false;
        }
    }
}
