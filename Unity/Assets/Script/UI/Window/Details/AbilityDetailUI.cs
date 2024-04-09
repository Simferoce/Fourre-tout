﻿using UnityEngine;
using UnityEngine.UI;

namespace Game
{
    public class AbilityDetailUI : MonoBehaviour
    {
        [SerializeField] private Image icon;

        private Ability characterAbility;

        public void Refresh(Ability characterAbility)
        {
            this.characterAbility = characterAbility;
            icon.sprite = characterAbility.Definition.Icon;
        }

        public void Inspect()
        {
            AbilityInspectorWindow.Open(characterAbility);
        }
    }
}
