﻿using UnityEngine;

namespace Game
{
    [StatisticClass("definition")]
    public abstract partial class AbilityDefinition : Definition
    {
        [SerializeField] private string title;
        [SerializeField] private string description;
        [SerializeField] private GameObject prefab;

        public string Title { get => title; }
        public string Description { get => description; set => description = value; }

        public virtual string ParseDescription()
        {
            return "";
        }

        public Ability GetAbility()
        {
            GameObject gameObject = Instantiate(prefab);
            Ability ability = gameObject.GetComponent<Ability>();
            ability.Definition = this;
            return ability;
        }
    }
}
