using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace AoTTG.EMAdditions.Scripts
{
    internal class VolumeControl : MonoBehaviour
    {
        public Transform objectTransform; // Reference to the object's Transform component
        public AudioSource audioSource; // Reference to the AudioSource component

        public float minSpeed = 1f; // Minimum speed for minimum volume
        public float maxSpeed = 10f; // Maximum speed for maximum volume

        private Vector3 previousPosition;

        private void Start()
        {
            // Get the required components if not already assigned
            if (objectTransform == null)
                objectTransform = transform;
            if (audioSource == null)
                audioSource = GetComponent<AudioSource>();

            // Store the initial position
            previousPosition = objectTransform.position;
        }

        private void Update()
        {
            // Calculate the speed based on position changes
            float currentSpeed = (objectTransform.position - previousPosition).magnitude / Time.deltaTime;

            // Map the speed to a volume range
            float normalizedSpeed = Mathf.InverseLerp(minSpeed, maxSpeed, currentSpeed);
            float targetVolume = Mathf.Lerp(0f, 1f, normalizedSpeed);

            // Set the volume of the AudioSource
            audioSource.volume = targetVolume;

            // Update the previous position
            previousPosition = objectTransform.position;
        }
    }
}
