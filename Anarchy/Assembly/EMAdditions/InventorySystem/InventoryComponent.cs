using Anarchy;
using AoTTG.EMAdditions.InventorySystem;
using Optimization;
using Optimization.Caching;
using UnityEngine;

public class InventoryComponent : Interactable
{
    private Inventory inv;
    private void Start()
    {
        inv = new Inventory();
    }

    public override void Interact()
    {
        Anarchy.UI.Chat.Add("Worked");
    }
}