﻿using Game.Statistics;

namespace Game.Modifier
{
    public abstract class StatisticModifierParameter : ModifierParameter
    {
        public StatisticDefinition StatisticDefinition { get; set; }

        protected StatisticModifierParameter(string name, StatisticDefinition statisticDefinition)
        {
            StatisticDefinition = statisticDefinition;
            this.Name = name;
        }
    }

    public class StatisticModifierParameter<T> : StatisticModifierParameter
    {
        public T Value { get; set; }

        public StatisticModifierParameter(string name, StatisticDefinition statisticDefinition, T value) : base(name, statisticDefinition)
        {
            Value = value;
        }
    }
}
