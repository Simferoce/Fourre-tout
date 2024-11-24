﻿using System.Linq;
using UnityEngine;

namespace Game.Statistics
{
    [CreateAssetMenu(fileName = "StandardStatisticDefinition", menuName = "Definition/Statistic/StandardStatisticDefinition")]
    public class StandardStatisticDefinition : StatisticDefinition<float>
    {
        public override float Modify(float value, StatisticRepository repository, Context context)
        {
            float flat = 0f;
            float percentage = 0f;
            float multiplier = 1f;
            float maximum = float.MaxValue;
            float minimum = float.MinValue;

            foreach (Statistic statistic in repository.Statistics)
            {
                if (statistic.Definition == null)
                    continue;

                BaseStatisticDefinitionData baseStatisticDefinitionData = statistic.Definition.Data.OfType<BaseStatisticDefinitionData>().FirstOrDefault(x => x.Definition == this);
                if (baseStatisticDefinitionData == null)
                    continue;

                OperatorStatisticDefinitionData operatorStatisticDefinitionData = statistic.Definition.Data.OfType<OperatorStatisticDefinitionData>().FirstOrDefault();
                if (operatorStatisticDefinitionData == null)
                    continue;

                bool applicable = statistic.Definition.Data.OfType<ContextStatisticDefinitionData>().All(x => x.IsApplicable(context));
                if (!applicable)
                    continue;

                if (operatorStatisticDefinitionData.StatisticOperator == StatisticOperator.Flat)
                    flat += statistic.GetModifiedValue<float>(context);
                else if (operatorStatisticDefinitionData.StatisticOperator == StatisticOperator.Pecentage)
                    percentage += statistic.GetModifiedValue<float>(context);
                else if (operatorStatisticDefinitionData.StatisticOperator == StatisticOperator.Multiplier)
                    multiplier *= statistic.GetModifiedValue<float>(context);
                else if (operatorStatisticDefinitionData.StatisticOperator == StatisticOperator.Maximum)
                    maximum = Mathf.Min(maximum, statistic.GetModifiedValue<float>(context));
                else if (operatorStatisticDefinitionData.StatisticOperator == StatisticOperator.Minimum)
                    minimum = Mathf.Max(minimum, statistic.GetModifiedValue<float>(context));
            }

            return Mathf.Clamp((value + flat) * (1 + percentage) * multiplier, minimum, maximum);
        }
    }
}
