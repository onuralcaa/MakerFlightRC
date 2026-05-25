using UnityEngine;

namespace MakerFlightRC.Runtime.CameraRig
{
    public class CameraFollow : MonoBehaviour
    {
        [SerializeField] private Transform target;
        private static readonly Vector3 RearOffset = new Vector3(0f, 3f, -10f);

        private void Start()
        {
            if (target != null)
            {
                var targetPos = target.position;
                if (float.IsNaN(targetPos.x) || float.IsNaN(targetPos.y) || float.IsNaN(targetPos.z) ||
                    float.IsInfinity(targetPos.x) || float.IsInfinity(targetPos.y) || float.IsInfinity(targetPos.z))
                {
                    targetPos = Vector3.zero;
                }

                targetPos.y = 0f;
                transform.position = targetPos + RearOffset;
            }
            else
            {
                transform.position = RearOffset;
            }
        }

        private void LateUpdate()
        {
            // Get target position with safety checks
            Vector3 targetPos = target != null ? target.position : Vector3.zero;

            // Handle NaN or Infinity values
            if (float.IsNaN(targetPos.x) || float.IsNaN(targetPos.y) || float.IsNaN(targetPos.z) ||
                float.IsInfinity(targetPos.x) || float.IsInfinity(targetPos.y) || float.IsInfinity(targetPos.z))
            {
                targetPos = Vector3.zero;
            }

            targetPos.y = 0f;

            // Calculate camera position from origin-based offset
            Vector3 cameraPos = targetPos + RearOffset;

            // Verify camera position is valid before applying
            if (!float.IsNaN(cameraPos.x) && !float.IsNaN(cameraPos.y) && !float.IsNaN(cameraPos.z) &&
                !float.IsInfinity(cameraPos.x) && !float.IsInfinity(cameraPos.y) && !float.IsInfinity(cameraPos.z))
            {
                transform.position = cameraPos;
                transform.LookAt(targetPos, Vector3.up);
            }
        }
    }
}
