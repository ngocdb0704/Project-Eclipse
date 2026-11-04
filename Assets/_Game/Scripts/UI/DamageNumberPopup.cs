
using System;
using System.Globalization;
using Eclipse.Core.Pooling;
using UnityEngine;

namespace Eclipse.UI
{
    [RequireComponent(typeof(TextMesh))]
    public sealed class DamageNumberPopup :
        MonoBehaviour,
        IPoolable
    {
        [SerializeField, Min(0.05f)]
        private float lifetime = 0.65f;

        [SerializeField, Min(0f)]
        private float riseSpeed = 1f;

        [SerializeField, Min(1f)]
        private float criticalScale = 1.35f;

        private TextMesh textMesh;

        private Vector3 baseScale;
        private Color baseColor;

        private float remainingLifetime;
        private bool isShowing;

        private Action<DamageNumberPopup> releaseCallback;

        private void Awake()
        {
            textMesh = GetComponent<TextMesh>();

            baseScale = transform.localScale;
            baseColor = textMesh.color;
        }

        public void OnSpawned()
        {
            ResetVisualState();
        }

        public void OnDespawned()
        {
            ResetVisualState();
        }

        private void ResetVisualState()
        {
            isShowing = false;
            remainingLifetime = 0f;
            releaseCallback = null;

            transform.localScale = baseScale;

            textMesh.text = string.Empty;
            textMesh.fontStyle = FontStyle.Normal;
            textMesh.color = baseColor;
        }

        public void Show(
            float amount,
            bool isCritical,
            Action<DamageNumberPopup> onRelease)
        {
            if (onRelease == null)
            {
                throw new ArgumentNullException(
                    nameof(onRelease));
            }

            releaseCallback = onRelease;

            textMesh.text =
                amount.ToString(
                    "0.#",
                    CultureInfo.InvariantCulture);

            textMesh.fontStyle =
                isCritical
                    ? FontStyle.Bold
                    : FontStyle.Normal;

            transform.localScale =
                baseScale *
                (isCritical ? criticalScale : 1f);

            textMesh.color = baseColor;

            remainingLifetime = lifetime;
            isShowing = true;
        }

        private void Update()
        {
            if (!isShowing)
            {
                return;
            }

            transform.position +=
                Vector3.up *
                riseSpeed *
                Time.deltaTime;

            remainingLifetime -= Time.deltaTime;

            float normalizedLifetime =
                Mathf.Clamp01(
                    remainingLifetime / lifetime);

            Color color = baseColor;
            color.a *= normalizedLifetime;

            textMesh.color = color;

            if (remainingLifetime <= 0f)
            {
                Release();
            }
        }

        private void Release()
        {
            if (!isShowing)
            {
                return;
            }

            isShowing = false;
            remainingLifetime = 0f;

            Action<DamageNumberPopup> callback =
                releaseCallback;

            releaseCallback = null;

            if (callback != null)
            {
                callback(this);
            }
            else
            {
                Debug.LogError(
                    "Damage popup has no pool release callback.",
                    this);

                gameObject.SetActive(false);
            }
        }
    }
}
