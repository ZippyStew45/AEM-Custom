using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace AoTTG.EMAdditions
{
    internal class ParentScript : MonoBehaviour
    {
        private void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.name.Contains("AOTTG_HERO"))
            {
                // Set the platform as the parent of the player
                collision.transform.SetParent(transform);
                GameObject H = gameObject.GetComponent<Horse>().gameObject;
            }
        }

        private void OnCollisionExit(Collision collision)
        {
            if (collision.gameObject.name.Contains("AOTTG_HERO"))
            {
                // Remove the platform as the parent of the player
                collision.transform.SetParent(null);
            }
        }
    }
}
