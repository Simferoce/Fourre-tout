﻿using System;
using UnityEngine;

namespace Game
{
    [Serializable]
    public class ProjectileImpactData : ProjectileData
    {
        [SerializeReference, SubclassSelector] private TargetCriteria criteria;

        public class Context : ProjectileContext
        {
            public TargetCriteria Criteria { get; set; }
        }

        public override ProjectileContext GetContext(Character character)
        {
            return new Context() { Criteria = criteria };
        }
    }
}