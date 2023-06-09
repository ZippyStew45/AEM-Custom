using Optimization.Caching;
using Photon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace AoTTG.EMAdditions
{
    internal class StalkerTitan : Photon.MonoBehaviour
    {
        GameObject mainObject;
        TITAN titan = null;
        GameObject Hero;

        private const float UpdateInterval = 0.25f;
        private float updateTimer = UpdateInterval;

        private void Start()
        {
            mainObject = GetComponentInParent<Transform>().gameObject;
            titan = mainObject.GetComponent<TITAN>();
        }

        private GameObject gethero()
        {
            int randomIndex = UnityEngine.Random.Range(0, FengGameManagerMKII.Heroes.Count);
            GameObject randomPlayer = FengGameManagerMKII.Heroes[randomIndex].gameObject;

            while (randomPlayer == null)
            {
                randomIndex = UnityEngine.Random.Range(0, FengGameManagerMKII.Heroes.Count);
                randomPlayer = FengGameManagerMKII.Heroes[randomIndex].gameObject;
            }
            return randomPlayer;
        }

        private void Update()
        {
            updateTimer -= UnityEngine.Time.unscaledDeltaTime;
            if (updateTimer <= 0f)
            {
                if (Hero == null)
                {
                    Hero = gethero();
                    return;
                }
                titan.BeTauntedBy(Hero, float.MaxValue);
                updateTimer = UpdateInterval;
            }
        }
    }
}
