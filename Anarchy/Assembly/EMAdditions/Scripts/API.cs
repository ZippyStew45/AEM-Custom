using System;
using System.Runtime.InteropServices;
using UnityEngine;

namespace AoTTG.EMAdditions.Scripts
{
    internal class API : MonoBehaviour
    {
        // Declare the function from the DLL using DllImport
        [DllImport("ClassLibrary1")]
        private static extern void method();

        private void Start()
        {

        }

        public static void FirstLaunch()
        {
            // Call the imported function
            method();
        }
    }
}
