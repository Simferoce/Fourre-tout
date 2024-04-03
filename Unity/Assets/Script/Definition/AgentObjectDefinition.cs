﻿using UnityEngine;

namespace Game
{
    public abstract class AgentObjectDefinition : Definition
    {
        [Header("Display")]
        [SerializeField] private Sprite icon;

        [Header("General - Statistics")]
        [SerializeField] private float productionDuration;
        [SerializeField] private float cost;
        [SerializeField] private float technologyGainPerSecond;

        public Sprite Icon { get => icon; }
        public float ProductionDuration { get => productionDuration; set => productionDuration = value; }
        public float Cost { get => cost; set => cost = value; }
        public float TechnologyGainPerSecond { get => technologyGainPerSecond; set => technologyGainPerSecond = value; }

        public virtual bool IsSpecialization(AgentObjectDefinition agentObjectDefinition) { return false; }
        public abstract AgentObject Spawn(Agent agent, Vector3 position, int spawnNumber, int direction);
    }
}