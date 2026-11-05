
using System;
using Eclipse.Core.Pooling;
using Eclipse.Player;
using UnityEngine;

namespace Eclipse.Loot
{
    [RequireComponent(
        typeof(SpriteRenderer),
        typeof(CircleCollider2D))]
    public sealed class ExpGem : MonoBehaviour, IPoolable
    {
        private SpriteRenderer spriteRenderer;
        private CircleCollider2D pickupCollider;
        private ExpGemPool ownerPool;

        private Color initialColor;
        private Vector3 initialScale;
        private int playerLayer;
        private bool collected;

        public int ExpValue { get; private set; }
        public bool IsInitialized { get; private set; }

        private void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            pickupCollider = GetComponent<CircleCollider2D>();

            initialColor = spriteRenderer.color;
            initialScale = transform.localScale;

            playerLayer = LayerMask.NameToLayer("Player");

            if (playerLayer < 0)
            {
                Debug.LogError("Missing Player layer.", this);
                enabled = false;
                return;
            }

            pickupCollider.callbackLayers = 1 << playerLayer;
        }

        internal void BindPool(ExpGemPool pool)
        {
            ownerPool = pool;
        }

        public void Initialize(int expValue)
        {
            if (expValue <= 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(expValue));
            }

            ExpValue = expValue;
            IsInitialized = true;
            collected = false;
            pickupCollider.enabled = true;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!isActiveAndEnabled ||
                !IsInitialized ||
                collected ||
                ownerPool == null ||
                other == null ||
                other.gameObject.layer != playerLayer)
            {
                return;
            }

            PlayerController player =
                other.GetComponentInParent<PlayerController>();

            if (player == null ||
                !player.isActiveAndEnabled ||
                player.Health == null ||
                player.Health.IsDead)
            {
                return;
            }

            // Prevent duplicate collection callbacks.
            collected = true;
            pickupCollider.enabled = false;

            int value = ExpValue;
            ownerPool.Collect(this, value);
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
            collected = false;
            ownerPool = null;

            if (pickupCollider != null)
                pickupCollider.enabled = false;

            if (spriteRenderer != null)
                spriteRenderer.color = initialColor;

            transform.localScale = initialScale;
        }
    }
}
