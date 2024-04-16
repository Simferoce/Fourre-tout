﻿using UnityEngine;
using UnityEngine.Serialization;

namespace Game
{
    [CreateAssetMenu(fileName = "Character", menuName = "Definition/AgentObject/Character")]
    public class CharacterDefinition : AgentObjectDefinition
    {
        [Header("Base")]
        [SerializeField] private CharacterDefinition baseDefinition;

        [Header("Character - Statistic")]
        [SerializeField] private float reach = 1f;
        [SerializeField] private float speed = 1f;
        [SerializeField, FormerlySerializedAs("attackPerSeconds")] private float attackSpeed = 1f;
        [SerializeField] private float attackPower = 1f;
        [SerializeField] private float maxHealth = 10f;
        [SerializeField] private float defense = 5f;
        [SerializeField] private float technologyGainPerSecond;

        [Header("Prefab")]
        [SerializeField] private GameObject prefab;

        public float Reach { get => reach; }
        public float Speed { get => speed; }
        public float AttackSpeed { get => attackSpeed; }
        public float AttackPower { get => attackPower; }
        public float MaxHealth { get => maxHealth; }
        public float Defense { get => defense; }
        public float TechnologyGainPerSecond { get => technologyGainPerSecond; set => technologyGainPerSecond = value; }

        public override bool IsSpecialization(AgentObjectDefinition agentObjectDefinition)
        {
            return baseDefinition == agentObjectDefinition || (baseDefinition?.IsSpecialization(agentObjectDefinition) ?? false);
        }

        public override AgentObject Spawn(Agent agent, Vector3 position, int spawnNumber, int direction)
        {
            Character character = GameObject.Instantiate(prefab, position, Quaternion.identity).GetComponent<Character>();
            character.Definition = this;
            character.Spawn(agent, spawnNumber, direction);

            return character;
        }
    }
}