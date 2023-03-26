using Anarchy;
using Optimization;
using Optimization.Caching;
using UnityEngine;

public abstract class Interactable : MonoBehaviour
{
    protected HERO h;

    private void OnTriggerStay(Collider other)
    {
        h = other.GetComponent<HERO>();
        if (h == null || !h.IsLocal) return;
        h.CheckInteraction = true;
    }

    private void OnTriggerExit(Collider other)
    {
        h = other.GetComponent<HERO>();
        if (h == null || !h.IsLocal) return;
        h.CheckInteraction = false;
    }

    private void OnDestroy()
    {
        h.CheckInteraction = false;
    }
    public abstract void Interact();
}