using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Antis.Collections.Generic;

namespace AoTTG.EMAdditions.InventorySystem
{
    public class Inventory
    {
        public int ID;
        private static Dictionary<int, Inventory> inventories = new SyncDictionary<int, Inventory>();

        public static Inventory GetInventory(int id)
        {
            return inventories[id];
        }
        private void Start()
        {
            inventories[ID] = this;
        }
    }
}
