using System.ComponentModel;
using UnityEngine;

internal class TitanKillTrigger : MonoBehaviour
{
    public bool Disabled { get; set; } = false;

    private void OnTriggerEnter(Collider other)
    {
        TITAN titan = other.gameObject.GetComponent<TITAN>();
        if (titan != null && titan.IsLocal && !titan.hasDie)
        {
            if (Disabled)
            {
                return;
            }
            titan.BasePV.RPC("netDie", PhotonTargets.All, new object[0]);
        }
    }
}