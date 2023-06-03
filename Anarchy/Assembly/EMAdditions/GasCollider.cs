using Anarchy.Configuration;
using AoTTG.EMAdditions.Sounds;
using Optimization.Caching;
using UnityEngine;

namespace AoTTG.EMAdditions
{
    internal class GasCollider : MonoBehaviour
    {
        private float ItemTimer = 600f;
        private const float UpdateInterval = 1.5f;

        private float updateTimer = UpdateInterval;


        void OnCollisionEnter(Collision col)
        {
            if (col.gameObject.name.Contains("AOTTG_HERO"))
            {
                foreach (HERO hero in FengGameManagerMKII.Heroes)
                {
                    if (hero != null && hero.IsLocal)
                    {
                        hero.AddGas(10000f);
                        Pool.Disable(gameObject);
                        FengGameManagerMKII.FGM.BasePV.RPC("DeletePrimitiveRPC", PhotonTargets.Others, gameObject.name);
                        if (Settings.GetSupply && AudioManager.List_string_of_loaded_sounds.Contains("refill")) AudioManager.AudioSource_refill.Play();
                    }
                }
            }
        }

        private void LateUpdate()
        {
            updateTimer -= UnityEngine.Time.unscaledDeltaTime;
            if (updateTimer <= 0f)
            {
                GameObject obj2;
                obj2 = (GameObject)UnityEngine.Object.Instantiate(Resources.Load("redcross"));
                obj2.transform.position = base.transform.position;
                updateTimer = UpdateInterval;
            }

            this.ItemTimer -= Time.deltaTime;
            if (this.ItemTimer <= 0f)
            {
                Pool.Disable(gameObject);
                return;
            }
        }
    }
}

