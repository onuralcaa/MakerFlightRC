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
                transform.position = target.position + RearOffset;
            }
        }

        private void LateUpdate()
        {
            if (target == null)
            {
                return;
            }

            // Clamp target position to prevent camera from flying into infinity
            Vector3 targetPos = target.position;
            if (float.IsNaN(targetPos.x) || float.IsNaN(targetPos.y) || float.IsNaN(targetPos.z))
            {
                targetPos = Vector3.zero;
            }

            // Clamp to reasonable bounds (±5000 units from origin)
            targetPos.x = Mathf.Clamp(targetPos.x, -5000f, 5000f);
            targetPos.y = Mathf.Clamp(targetPos.y, -5000f, 5000f);
            targetPos.z = Mathf.Clamp(targetPos.z, -5000f, 5000f);

            transform.position = targetPos + RearOffset;
            transform.LookAt(targetPos, Vector3.up);
        }
    }
}
