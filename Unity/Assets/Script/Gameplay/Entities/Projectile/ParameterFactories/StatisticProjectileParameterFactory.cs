﻿using Game.Projectile;
using Game.Statistics;
using System;
using UnityEngine;

namespace Game.Ability
{
    [Serializable]
    public class StatisticProjectileParameterFactory<T> : ProjectileParameterFactory
    {
        [SerializeField] private string name;
        [SerializeField] private StatisticReference<T> statistic;

        public override ProjectileParameter Create(object entity)
        {
            return new StatisticProjectileParameter<T>(name, statistic.Resolve(entity as Entity));
        }
    }

    [Serializable]
    public class StatisticProjectileParameterFactoryFloat : StatisticProjectileParameterFactory<float>
    {

    }
}
