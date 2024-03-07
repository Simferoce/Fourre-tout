﻿using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Game
{
    public class Agent : MonoBehaviour
    {
        public static List<Agent> agents = new List<Agent>();
        public static Agent Player => agents.FirstOrDefault(x => x.Faction == Faction.Player);

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void Init()
        {
            agents = new List<Agent>();
        }

        [SerializeField] private Faction faction;
        [SerializeField] private Base agentBase;
        [SerializeField] private int direction;

        private Factory factory = new Factory();

        public Faction Faction { get => faction; }
        public Factory Factory { get => factory; set => factory = value; }

        private void Start()
        {
            agentBase.transform.position = Lane.Instance.Project(agentBase.transform.position, out float agentBasePosition);
            agentBase.Spawn(this, 0, agentBasePosition, direction);
        }

        private void OnEnable()
        {
            agents.Add(this);
        }

        private void OnDisable()
        {
            agents.Remove(this);
        }

        public void SpawnLaneObject(LaneObjectDefinition laneObjectDefinition)
        {
            factory.SpawnLaneObject(this, agentBase.SpawnPoint, laneObjectDefinition);
        }
    }
}
