﻿using UnityEngine;
using System.Collections;

namespace NDR2ndTTB
{
    [System.Serializable]
    public class UnitStats
    {
        [SerializeField] float currentHealth = 65;
        [SerializeField] float stamina = 60;
        [SerializeField] float morale = 50;
        [SerializeField] int currentActionPoints;



        [Range(0, 100)] public float health = 65;
        [Range(0, 100)] public float agility = 85;
        [Range(0, 100)] public float strength = 64;
        [Range(0, 100)] public float dexterity = 70;
        [Range(0, 100)] public float wisdom = 81;

        [Range(0, 20)] public int level = 1;

        public int CurrentActionPoints { get => currentActionPoints; set => currentActionPoints = value; }

        public int GetBaseActionPoints()
        {
            int r = 0;

            float h = currentHealth / 20;
            float a = agility / 10;
            float d = dexterity / 20;
            float l = (level + 1) / 2;

            float sum = h + a + d + l;

            r = Mathf.RoundToInt(sum);

            int r2 = r + currentActionPoints;
            r2 = Mathf.Clamp(r2, 0, r + 5);

            return r2;


        }

    }
}