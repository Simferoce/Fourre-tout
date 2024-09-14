﻿using System;

namespace Game
{
    [Serializable]
    public class IsInjuredTargetCriteria : TargetCriteria
    {
        public override bool Execute(Target owner, Target targeteable, IStatisticProvider statisticProvider, Faction ownerFaction, Faction targetFaction)
        {
            return (targeteable.Entity is Character character) && character.IsInjured;
        }
    }
}
