﻿using System;
using UnityEngine;

namespace Game.Statistics
{
    [Serializable]
    public class StandardIntegerStatistic : Statistic<int>
    {
        [SerializeReference, SubclassSelector] private Value value;
        public Value Value { get => value; set => this.value = value; }

        public override void Initialize(Entity entity)
        {
            base.Initialize(entity);
            value.Initialize(entity);
        }

        public override Statistic Snapshot(Context context)
        {
            return new StandardIntegerStatistic() { definition = this.definition, Value = new SerializeValue<int>() { Value = GetModifiedValue(context) } };
        }

        public override int GetModifiedValue(Context context)
        {
            return definition != null ? definition.Modify(value.GetValue<int>(), entity.GetCachedComponent<StatisticRepository>(), context) : GetBaseValue(context);
        }
        public override int GetBaseValue(Context context)
        {
            return value.GetValue<int>();
        }

        public override bool TryGetDescription(out string description, Context context)
        {
            return value.TryGetDescription(out description);
        }

        public override string GetFormattedValue(string format, Context context)
        {
            return value.GetValue<int>().ToString();
        }
    }
}
