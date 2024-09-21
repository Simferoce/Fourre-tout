﻿using System;
using UnityEngine;

namespace Game
{
    public abstract class ModifierDefinition : Definition, IStatisticContext
    {
        [Serializable]
        public abstract class Instancier
        {
            public abstract ModifierDefinition Definition { get; set; }
            public abstract Modifier Instantiate(ModifierHandler modifiable, IModifierSource source);
        }

        [Serializable]
        public abstract class Instancier<T> : Instancier
            where T : ModifierDefinition
        {
            [SerializeField] protected T definition;

            public override ModifierDefinition Definition { get => definition; set => definition = (T)value; }
        }

        [SerializeField] private string title;
        [SerializeField] private Sprite icon;
        [SerializeField] private string description;
        [SerializeField] private bool showOnHealthBar = true;

        public Sprite Icon { get => icon; }
        public string Title { get => title; }
        public bool Show { get => showOnHealthBar; set => showOnHealthBar = value; }
        public string Description { get => description; set => description = value; }

        public virtual string ParseDescription()
        {
            return description;
        }

        public bool IsName(ReadOnlySpan<char> name)
        {
            return name.SequenceEqual("definition");
        }

        public virtual Statistic GetStatistic(ReadOnlySpan<char> value)
        {
            return null;
        }

        public virtual IStatisticContext GetContext(ReadOnlySpan<char> value)
        {
            return null;
        }
    }
}