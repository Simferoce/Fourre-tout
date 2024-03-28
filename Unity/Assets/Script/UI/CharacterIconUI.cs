﻿using Game;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UI
{
    public class CharacterIconUI : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] private AgentObjectDefinition laneObjectDefinition;

        public void OnPointerClick(PointerEventData eventData)
        {
            Agent.Player.SpawnLaneObject(laneObjectDefinition);
        }
    }
}
