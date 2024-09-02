﻿using System;
using Unity.Profiling;
using UnityEngine;

namespace Game
{
    [Serializable]
    public class StatisticReference<T>
    {
        [SerializeField] private string path;

        public T GetValueOrThrow(IStatisticProviderOld statisticProvider)
        {
            return TryGetValue(statisticProvider, out T value) ? value : throw new Exception($"Could not resolve the path \"{path}\" for \"{statisticProvider}\"");
        }

        public T GetValueOrDefault(IStatisticProviderOld statisticProvider)
        {
            return TryGetValue(statisticProvider, out T value) ? value : default;
        }

        private bool TryGetValue(IStatisticProviderOld statisticProvider, out T value)
        {
            using (new ProfilerMarker("Statistic.Resolve").Auto())
            {
                if (string.IsNullOrEmpty(path))
                {
                    value = default(T);
                    return false;
                }

                return statisticProvider.TryGetStatistic<T>(path, out value);
            }
        }
    }
}
