﻿using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Game
{
    public abstract class Modifier : IModifier, IDisposable
    {
        public List<ModifierElement> modifierElements = new List<ModifierElement>();
        public ModifierDefinition Definition { get; }

        public virtual float? SpeedPercentage => null;
        public virtual float? Defense => null;
        public virtual float? MaxHealth => null;
        public virtual float? AttackPower => null;
        public virtual bool? Invulnerable => null;
        public IModifiable Modifiable { get => modifiable; set => modifiable = value; }

        protected IModifiable modifiable;

        protected Modifier(ModifierDefinition modifierDefinition, IModifiable modifiable)
        {
            this.modifiable = modifiable;
            this.Definition = modifierDefinition;
        }

        public void Initialize()
        {
            foreach (ModifierElement element in modifierElements)
                element.Initialize();
        }

        public virtual void Update()
        {
            bool end = false;
            foreach (ModifierElement element in modifierElements)
            {
                end |= element.Update();
            }

            if (end)
                modifiable.RemoveModifier(this);
        }

        public virtual float? GetPercentageRemainingDuration()
        {
            CharacterModifierTimeElement modifierElement = (CharacterModifierTimeElement)modifierElements.FirstOrDefault(x => x is CharacterModifierTimeElement);

            if (modifierElement == null)
                return null;

            return Mathf.Clamp01(modifierElement.RemaingDuration / modifierElement.Duration);
        }

        public virtual float? GetStack()
        {
            StackModifierElement stackModifierElement = (StackModifierElement)modifierElements.FirstOrDefault(x => x is StackModifierElement);
            return stackModifierElement?.CurrentStack;
        }

        public virtual T GetValueOrThrow<T>(StatisticDefinition definition)
        {
            return TryGetValue(definition, out T value) == true ? value : throw new Exception($"Could not resolve the statistic {definition}");
        }

        public virtual bool TryGetValue<T>(StatisticDefinition definition, out T value)
        {
            if (definition == StatisticDefinition.AttackPower)
            {
                float? stat = AttackPower;
                if (stat.HasValue)
                {
                    value = (T)(object)stat.Value;
                    return true;
                }

                return Definition.TryGetValue(this, definition, out value);
            }
            else if (definition == StatisticDefinition.Defense)
            {
                float? stat = Defense;
                if (stat.HasValue)
                {
                    value = (T)(object)stat.Value;
                    return true;
                }

                return Definition.TryGetValue(this, definition, out value);
            }
            else if (definition == StatisticDefinition.Speed)
            {
                float? stat = SpeedPercentage;
                if (stat.HasValue)
                {
                    value = (T)(object)stat.Value;
                    return true;
                }

                return Definition.TryGetValue(this, definition, out value);
            }
            else if (definition == StatisticDefinition.MaxHealth)
            {
                float? stat = MaxHealth;
                if (stat.HasValue)
                {
                    value = (T)(object)stat.Value;
                    return true;
                }

                return Definition.TryGetValue(this, definition, out value);
            }

            return Definition.TryGetValue(this, definition, out value);
        }

        public virtual void Refresh()
        {
            foreach (ModifierElement element in modifierElements)
            {
                element.Refresh();
            }
        }

        public virtual void Dispose() { }
    }

    public abstract class Modifier<T> : Modifier
        where T : Modifier<T>
    {
        protected Modifier(IModifiable modifiable, ModifierDefinition modifierDefinition) : base(modifierDefinition, modifiable)
        {
        }

        public T With(List<ModifierElement> modifierElements)
        {
            this.modifierElements.AddRange(modifierElements);
            return (T)this;
        }

        public T With(ModifierElement modifierElement)
        {
            this.modifierElements.Add(modifierElement);
            return (T)this;
        }
    }
}
