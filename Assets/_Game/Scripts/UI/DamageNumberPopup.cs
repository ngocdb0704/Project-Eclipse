using System.Globalization;
using UnityEngine;

namespace Eclipse.UI
{
    [RequireComponent(typeof(TextMesh))]
    public sealed class DamageNumberPopup :
        MonoBehaviour
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

        private void Awake()
        {
            textMesh =
                GetComponent<TextMesh>();

            baseScale =
                transform.localScale;

            baseColor =
                textMesh.color;
        }

        public void Show(
            float amount,
            bool isCritical)
        {
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
                (isCritical
                    ? criticalScale
                    : 1f);

            textMesh.color =
                baseColor;

            remainingLifetime =
                lifetime;

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

            remainingLifetime -=
                Time.deltaTime;

            float normalizedLifetime =
                lifetime > 0f
                    ? Mathf.Clamp01(
                        remainingLifetime /
                        lifetime)
                    : 0f;

            Color color =
                baseColor;

            color.a *=
                normalizedLifetime;

            textMesh.color =
                color;

            if (remainingLifetime <= 0f)
            {
                Destroy(gameObject);
            }
        }
    }
}