﻿using System;

namespace Game
{
    [Serializable]
    public class IsAllyTargetCriteria : TargetCriteria
    {
        public override bool Execute(ITargeteable owner, ITargeteable targeteable, StatisticContext context)
        {
            return targeteable.IsAlly(owner);
        }

        public override TargetCriteria Clone()
        {
            return new IsAllyTargetCriteria();
        }
    }
}
