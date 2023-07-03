using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace AoTTG.EMAdditions
{
    internal class ParentScript : MonoBehaviour
    {
        private Transform parentTransform;  // Reference to the parent object's transform
        private Vector3 initialLocalPosition;  // Initial local position relative to the parent
        private Quaternion initialLocalRotation;  // Initial local rotation relative to the parent

        private void Awake()
        {
            parentTransform = transform.parent;
            initialLocalPosition = transform.localPosition;
            initialLocalRotation = transform.localRotation;
        }

        private void LateUpdate()
        {
            // Update the object's position and rotation relative to the parent
            transform.position = parentTransform.TransformPoint(initialLocalPosition);
            transform.rotation = parentTransform.rotation * initialLocalRotation;

            rigidbody.velocity = Vector3.zero;
            rigidbody.angularVelocity = Vector3.zero;
        }
    }
}
