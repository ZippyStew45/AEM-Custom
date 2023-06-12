using Photon;
using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Anarchy.UI.Animation;

namespace AoTTG.EMAdditions
{
    internal class FakerTitan : Photon.MonoBehaviour
    {
        GameObject mainObject;
        TITAN titan = null;

        private void Start()
        {
            mainObject = GetComponentInParent<Transform>().gameObject;
            titan = mainObject.GetComponent<TITAN>();
            titan.runAnimation = RanndomRunAni();
        }

        private string RanndomRunAni()
        {
            if (titan.abnormalType == AbnormalType.Crawler)
                return "crawler_run";

            int ran = UnityEngine.Random.Range(0, 3);
            string[] animation = { "run_walk", "run_abnormal", "run_abnormal_1" };
            string RunAni;
            switch (ran)
            {
                case 0:
                    if (titan.abnormalType == AbnormalType.Normal)
                    {
                        RunAni = animation[1];
                        break;
                    }
                    RunAni = animation[0];
                    break;
                case 1:
                    if (titan.abnormalType == AbnormalType.Jumper || titan.abnormalType == AbnormalType.Aberrant)
                    {
                        RunAni = animation[0];
                        break;
                    }
                    RunAni = animation[1];
                    break;
                case 2:
                    if (titan.abnormalType == AbnormalType.Punk)
                    {
                        RunAni = animation[0];
                        break;
                    }
                    RunAni = animation[2];
                    break;
                default:
                    RunAni = animation[0];
                    break;
            }
            return RunAni;
        }
    }
}
