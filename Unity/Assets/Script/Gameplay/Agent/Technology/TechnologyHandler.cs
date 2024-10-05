﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    [Serializable]
    public class TechnologyHandler : IDisposable
    {
        public delegate void PerkAcquiredDelegate(TechnologyPerkDefinition technologyPerkDefinition);

        [SerializeField] private float maxTechnology;
        [SerializeField] private List<TechnologyTreeDefinition> technologyTreeDefinitions;

        public event PerkAcquiredDelegate OnPerkAcquired;
        public event Action OnLeveledUp;

        public float CurrentTechnology { get; set; }
        public float CurrentLevel { get; set; }
        public float CurrentTechnologyNormalized { get => CurrentTechnology / maxTechnology; }
        public float MaxTechnology { get => maxTechnology; set => maxTechnology = value; }
        public IReadOnlyList<TechnologyTree> TechnologyTrees { get => technologyTrees; }

        private Agent agent;
        private List<TechnologyTree> technologyTrees = new List<TechnologyTree>();

        public void Initialize(Agent agent)
        {
            this.agent = agent;

            foreach (TechnologyTreeDefinition technologyTreeDefinition in technologyTreeDefinitions)
            {
                TechnologyTree technologyTree = technologyTreeDefinition.Instantiate(agent);
                technologyTree.OnPerkAcquired += PerkAcquired;
                technologyTrees.Add(technologyTree);
            }
        }

        private void PerkAcquired(TechnologyPerkDefinition perk)
        {
            OnPerkAcquired?.Invoke(perk);
        }

        public void Dispose()
        {
            foreach (TechnologyTree technologyTree in technologyTrees)
            {
                technologyTree.OnPerkAcquired -= PerkAcquired;
            }
        }

        public void Update()
        {
            foreach (AgentObject agentObject in AgentObject.All)
            {
                if (agentObject is Character character && agentObject.Agent == agent)
                {
                    CurrentTechnology += Level.Instance.TechnologyGainMultiplier * character.TechnologyGainPerSecond * Time.deltaTime;
                }
            }

            if (CurrentTechnology >= maxTechnology)
            {
                CurrentTechnology -= maxTechnology;
                LevelUp();
            }
        }

        private void LevelUp()
        {
            CurrentLevel++;
            OnLeveledUp?.Invoke();
        }

        public bool Has(TechnologyPerkDefinition technologyPerkDefinition)
        {
            foreach (TechnologyTree technologyTree in technologyTrees)
            {
                if (technologyTree.Has(technologyPerkDefinition))
                    return true;
            }

            return false;
        }

        public TechnologyPerkStatus GetStatus(TechnologyPerkDefinition technologyPerkDefinition)
        {
            foreach (TechnologyTree technologyTree in technologyTrees)
            {
                if (technologyTree.Has(technologyPerkDefinition))
                    return technologyTree.GetStatus(technologyPerkDefinition);
            }

            return new TechnologyPerkStatusNotInTree();
        }

        public TechnologyPerkDefinition GetFirst(System.Func<TechnologyPerkDefinition, bool> selector)
        {
            foreach (TechnologyTree technologyTree in technologyTrees)
            {
                TechnologyPerkDefinition technologyPerkDefinition = technologyTree.GetFirst(selector);
                if (technologyPerkDefinition != null)
                    return technologyPerkDefinition;
            }

            return null;
        }

        public IEnumerable UnlockedPerks()
        {
            foreach (TechnologyTree technologyTree in technologyTrees)
            {
                foreach (TechnologyPerkDefinition technologyPerkDefinition in technologyTree.PerksUnlocked)
                {
                    yield return technologyPerkDefinition;
                }
            }
        }
    }
}
