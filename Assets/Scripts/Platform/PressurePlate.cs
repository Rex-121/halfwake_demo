using System;
using System.Collections.Generic;
using UnityEngine;

namespace Platform
{
    public class PressurePlate : MonoBehaviour
    {
        public event Action<PressurePlate, bool> OnActivationChanged;

        [SerializeField] private string[] activatorTags = { "Player", "Clone" };

        private HashSet<Collider2D> collidersInside = new HashSet<Collider2D>();
        private bool wasActivated;

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (IsValidActivator(other))
            {
                collidersInside.Add(other);
                CheckActivation();
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (collidersInside.Remove(other))
                CheckActivation();
        }

        private bool IsValidActivator(Collider2D col)
        {
            var go = col.attachedRigidbody != null ? col.attachedRigidbody.gameObject : col.gameObject;
            foreach (string tag in activatorTags)
            {
                if (go.CompareTag(tag))
                    return true;
            }
            return false;
        }

        private void CheckActivation()
        {
            bool isActivated = collidersInside.Count > 0;
            if (isActivated != wasActivated)
            {
                wasActivated = isActivated;
                Debug.Log($"压力板 {gameObject.name} {(isActivated ? "被激活" : "取消激活")}");
                OnActivationChanged?.Invoke(this, isActivated);
            }
        }

        private void OnDisable()
        {
            collidersInside.Clear();
            if (wasActivated)
            {
                wasActivated = false;
                OnActivationChanged?.Invoke(this, false);
            }
        }
    }
}
