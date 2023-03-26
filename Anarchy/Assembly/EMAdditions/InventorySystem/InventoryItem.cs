using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace AoTTG.EMAdditions.InventorySystem
{
    abstract class InventoryItem
    {
        public string Name;
        public bool Droppable;

        protected abstract GameObject physicalObject { get; }
        public abstract void Use();

        public abstract void Drop();
    }
}
