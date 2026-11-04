
using System;
using Eclipse.Core.Pooling;
using UnityEngine;

namespace Eclipse.Loot
{
    [RequireComponent(
        typeof(SpriteRenderer),
        typeof(CircleCollider2D))]
    public sealed class ExpGem :
        MonoBehaviour,
        IPoolable
    {
        private SpriteRenderer spriteRenderer;
        private CircleCollider2D pickupCollider;

        private Color initialColor;
        private Vector3 initialScale;

        public int ExpValue { get; private set; }

        public bool IsInitialized { get; private set; }

        private void Awake()
        {
            spriteRenderer =
                GetComponent<SpriteRenderer>();

            pickupCollider =
                GetComponent<CircleCollider2D>();

            initialColor = spriteRenderer.color;
            initialScale = transform.localScale;
        }

        public void Initialize(int expValue)
        {
            if (expValue <= 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(expValue),
                    "EXP value must be greater than zero.");
            }

            ExpValue = expValue;
            IsInitialized = true;

            pickupCollider.enabled = true;
        }

        public void OnSpawned()
        {
            ResetState();
        }

        public void OnDespawned()
        {
            ResetState();
        }

        private void ResetState()
        {
            ExpValue = 0;
            IsInitialized = false;

            if (pickupCollider != null)
            {
                pickupCollider.enabled = false;
            }

            if (spriteRenderer != null)
            {
                spriteRenderer.color = initialColor;
            }

            transform.localScale = initialScale;
        }
    }
}
