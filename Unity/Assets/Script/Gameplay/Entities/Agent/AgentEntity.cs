﻿using Game.Character;
using Game.Components;
using Game.Modifier;
using Game.Statistics;
using Game.Technology;
using System.Linq;
using UnityEngine;

namespace Game.Agent
{
    public class AgentEntity : Entity
    {
        public delegate void AgentObjectSpawnDelegate(AgentIdentity agentIdentity);

        public event AgentObjectSpawnDelegate OnAgentObjectSpawn;

        public AgentFactory Factory { get => factory; set => factory = value; }
        public BaseEntity Base { get => agentBase; set => agentBase = value; }
        public float Currency { get; set; }
        public int Direction { get => direction; set => direction = value; }
        public TechnologyHandler Technology { get => technology; }
        public AgentLoadout Loadout { get => loadout; set => loadout = value; }
        public FactionType Faction { get => faction; set => faction = value; }

        private int nextSpawneeNumber = 0;
        private AgentLoadout loadout;
        private AgentBehaviour agentBehaviour;
        private FactionType faction;
        private BaseEntity agentBase;
        private int direction;
        private TechnologyHandler technology = new TechnologyHandler();
        private AgentFactory factory = new AgentFactory();

        public void Initialize(AgentBehaviour agentBehaviour, AgentLoadout agentLoadout, FactionType faction, BaseEntity agentBase, int direction)
        {
            ModifierHandler modifierHandler = AddOrGetCachedComponent<ModifierHandler>();
            Caster caster = AddOrGetCachedComponent<Caster>();
            ModifierApplier modifierApplier = AddOrGetCachedComponent<ModifierApplier>();

            loadout = agentLoadout;
            this.agentBehaviour = agentBehaviour;
            this.faction = faction;
            this.agentBase = agentBase;
            this.direction = direction;

            factory.Initialize(this);
            loadout.Initialize(this);
            technology.Initialize(this);
            agentBehaviour.Initialize(this);

            StatisticRepository.Add(new Statistic<FactionType>("faction", null, new SerializeValue<FactionType>(), (FactionType baseValue) => Faction));
            StatisticRepository.Add(new StatisticFloat("flat_attack_power", StatisticDefinitionRegistry.Instance.FlatAttackPower, 0f, (float baseValue) => baseValue + modifierHandler[StatisticDefinitionRegistry.Instance.FlatAttackPower]));
            StatisticRepository.Add(new StatisticFloat("multiplier_speed", StatisticDefinitionRegistry.Instance.MultiplierSpeed, 1f, (float baseValue) => baseValue * modifierHandler[StatisticDefinitionRegistry.Instance.MultiplierSpeed]));
            StatisticRepository.Add(new StatisticFloat("multiplier_reach", StatisticDefinitionRegistry.Instance.MultiplierReach, 1f, (float baseValue) => baseValue * modifierHandler[StatisticDefinitionRegistry.Instance.MultiplierReach]));
            StatisticRepository.Add(new StatisticFloat("multiplier_attack_speed", StatisticDefinitionRegistry.Instance.MultiplierAttackSpeed, 1f, (float baseValue) => baseValue * modifierHandler[StatisticDefinitionRegistry.Instance.MultiplierAttackSpeed]));
            StatisticRepository.Add(new StatisticFloat("ranged_percentage_reach", StatisticDefinitionRegistry.Instance.PercentageMultiplierReach, 0f, (float baseValue) => baseValue + modifierHandler[StatisticDefinitionRegistry.Instance.PercentageMultiplierReach]));
            AgentIdentity agentAgentIdentity = this.AddOrGetCachedComponent<AgentIdentity>();
            agentAgentIdentity.Set(this, int.MinValue, direction);

            AgentIdentity agentIdentity = agentBase.AddOrGetCachedComponent<AgentIdentity>();
            agentIdentity.Set(this, int.MaxValue, direction);
            agentBase.Initialize();

            caster.Initialize();
            caster.AddAbility(agentLoadout.CommanderDefinition.Active);

            modifierApplier.Apply(agentLoadout.CommanderDefinition.Perk, modifierHandler);

            foreach (TechnologyTreeDefinition tree in agentLoadout.CharacterDefinitions.Select(x => x.TechnologyTreeDefinition))
            {
                if (tree != null)
                {
                    technology.AddTree(tree);
                }
            }

            base.Initialize();
        }

        private void Update()
        {
            factory.Update();
            technology.Update();
            agentBehaviour.Update();

            Currency += 5f * Time.deltaTime;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            agentBehaviour.Dispose();
        }

        public bool TryQueueSpawnAgentObject(int index)
        {
            CharacterDefinition characterDefinition = Loadout.GetCharacterDefinitionAtIndex(index);
            return factory.QueueLaneObject(new AgentFactoryCommand(this, agentBase.SpawnPoint, characterDefinition.ProductionDuration / CheatManager.Instance.FactorySpeed, characterDefinition), characterDefinition.Cost);
        }

        public CharacterEntity SpawnAgentObject(CharacterDefinition characterDefinition, Vector3 position, int direction)
        {
            return SpawnAgentObject(characterDefinition, position, direction, nextSpawneeNumber++);
        }

        public CharacterEntity SpawnAgentObject(CharacterDefinition characterDefinition, Vector3 position, int direction, int spawnNumber)
        {
            CharacterEntity character = characterDefinition.Spawn(this, position, spawnNumber, direction);
            OnAgentObjectSpawn?.Invoke(character.GetCachedComponent<AgentIdentity>());
            character.Initialize();

            character.name = $"{characterDefinition.Title} - {faction}";
            return character;
        }
    }
}
