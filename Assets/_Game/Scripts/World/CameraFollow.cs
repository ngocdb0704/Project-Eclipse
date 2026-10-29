using UnityEngine;

namespace Eclipse.World
{
    public sealed class CameraFollow : MonoBehaviour
    {
        [SerializeField]
        private Transform target;

        [SerializeField, Min(0f)]
        private float smoothTime = 0.15f;

        private Vector3 velocity;
        private float cameraZ;

        private void Awake()
        {
            cameraZ = transform.position.z;

            if (target == null)
            {
                Debug.LogError(
                    "CameraFollow requires a target.",
                    this);

                enabled = false;
                return;
            }

            SnapToTarget();
        }

        private void LateUpdate()
        {
            Vector3 targetPosition = target.position;
            targetPosition.z = cameraZ;

            if (smoothTime <= 0f)
            {
                transform.position = targetPosition;
                return;
            }

            transform.position = Vector3.SmoothDamp(
                transform.position,
                targetPosition,
                ref velocity,
                smoothTime);
        }

        private void SnapToTarget()
        {
            Vector3 targetPosition = target.position;
            targetPosition.z = cameraZ;

            transform.position = targetPosition;
            velocity = Vector3.zero;
        }
    }
}