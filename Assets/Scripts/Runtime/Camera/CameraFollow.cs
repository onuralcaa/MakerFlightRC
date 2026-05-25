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

            // Get target position, fallback to zero if NaN
            Vector3 targetPos = target.position;
            if (float.IsNaN(targetPos.x) || float.IsNaN(targetPos.y) || float.IsNaN(targetPos.z))
            {
                targetPos = Vector3.zero;
            }

            transform.position = targetPos + RearOffset;
            transform.LookAt(targetPos, Vector3.up);
        }
    }
}
