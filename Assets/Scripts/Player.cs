﻿using ItemSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HordeSurvivalGame
{
    public class Player : MonoBehaviour
    {


        [SerializeField]
        private Item iron;
        [SerializeField]
        private Item coal;
        [SerializeField]
        private Item lead;


        [SerializeField]
        private HeartSpriteManager heart;

        public static Transform playerTransform;
        // Start is called before the first frame update
        void Awake()
        {
            playerTransform = this.transform;

            PlayerResources.iron = iron;
            PlayerResources.coal = coal;
            PlayerResources.lead = lead;

        }

        public void LostHealth()
        {
            Debug.Log("Ouch!");
            heart.HeartsChanged();
        }
    }
}
