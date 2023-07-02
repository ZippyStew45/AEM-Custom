using Anarchy.Configuration;
using AoTTG.EMAdditions.Sounds;
using Optimization.Caching;
using UnityEngine;

namespace AoTTG.EMAdditions
{
    internal class BladeCollider : MonoBehaviour
    {
        private float ItemTimer = 600f;
        private const float UpdateInterval = 2f;

        private float minDistDespawn = 2500f;
        private float minDistCollect = 2f;
        private float HeroDistance;

        private float updateTimer = UpdateInterval;


        void OnCollisionEnter(Collision col)
        {
            if (col.gameObject.name.Contains("AOTTG_HERO"))
            {
                HERO hero = col.gameObject.GetComponent<HERO>();
                if (hero != null && hero.IsLocal && HeroDistance <= minDistCollect)
                {
                    hero.AddBlade(1);
                    Pool.Disable(gameObject);
                    FengGameManagerMKII.BladeObjects.Remove(gameObject);
                    FengGameManagerMKII.FGM.BasePV.RPC("DeletePrimitiveRPC", PhotonTargets.Others, gameObject.name);
                    if (Settings.GetSupply && AudioManager.List_string_of_loaded_sounds.Contains("refill")) AudioManager.AudioSource_refill.Play();
                }
            }
        }

        private void LateUpdate()
        {
            if (PhotonPlayer.MyHero() == null)
            {
                HeroDistance = 10;
            }
            else
                HeroDistance = Vector3.Distance(PhotonPlayer.MyHero().transform.position, gameObject.transform.position);

            updateTimer -= UnityEngine.Time.unscaledDeltaTime;
            if (updateTimer <= 0f)
            {
                if (HeroDistance > minDistDespawn)
                {
                    Pool.Disable(gameObject);
                    FengGameManagerMKII.BladeObjects.Remove(gameObject);
                }

                GameObject obj2;
                obj2 = (GameObject)UnityEngine.Object.Instantiate(Resources.Load("redcross1"));
                obj2.transform.position = base.transform.position;

                updateTimer = UpdateInterval;
            }

            this.ItemTimer -= Time.deltaTime;
            if (this.ItemTimer <= 0f)
            {
                Pool.Disable(gameObject);
                FengGameManagerMKII.BladeObjects.Remove(gameObject);
                return;
            }
        }
    }
}
