﻿using Game.Ability;
using Game.Statistics;
using TMPro;
using UnityEngine;

namespace Game.UI.Windows
{
    public class AbilityInspectorWindow : Window
    {
        [SerializeField] private TextMeshProUGUI title;
        [SerializeField] private TextMeshProUGUI description;
        [SerializeField] private TextMeshProUGUI cooldownText;

        public static AbilityInspectorWindow Open(AbilityEntity ability, string abilitySlotName)
        {
            AbilityInspectorWindow abilityInspectorWindow = WindowManager.Instance.GetWindow<AbilityInspectorWindow>();
            abilityInspectorWindow.Show();
            abilityInspectorWindow.title.text = ability.Definition.Title;
            abilityInspectorWindow.description.text = ability.ParseDescription();

            if (ability.Cooldown > 0f)
            {
                abilityInspectorWindow.cooldownText.alpha = 1f;
                abilityInspectorWindow.cooldownText.text = $"{ability.Cooldown}{StatisticDefinitionRepository.Instance.GetById(StatisticIdentifiant.Cooldown).TextIcon}";
            }
            else
            {
                abilityInspectorWindow.cooldownText.alpha = 0f;
            }

            return abilityInspectorWindow;
        }

        public void Close()
        {
            Hide();
        }
    }
}