﻿using Game;
using System;

[Serializable]
public class StatisticAbilitySelfProvider : StatisticAbilityProvider
{
    public override IStatisticContext Resolve(IStatisticContext context)
    {
        if (!(context is Ability ability))
            throw new ArgumentException();

        return ability;
    }
}