using UnityEngine;

namespace MakerFlightRC.Runtime.CameraRig
{
    public class CameraFollow : MonoBehaviour
    {
        [SerializeField] private Transform target;
        private static readonly Vector3 RearOffset = new Vector3(0f, 3f, -10f);
        private static readonly Vector3 LookOffset = new Vector3(0f, 0.5f, 0f);

        private void Start()
        {
            if (target != null)
            {
                transform.position = target.TransformPoint(RearOffset);
                transform.LookAt(target.position + LookOffset, Vector3.up);
            }
            else
            {
                transform.position = RearOffset;
            }
        }

        private void LateUpdate()
        {
            if (target == null)
            {
                return;
            }

            // Use world-space offset (global position + offset)
            Vector3 cameraPos = target.position + RearOffset;

            // Verify camera position is valid before applying
            if (!float.IsNaN(cameraPos.x) && !float.IsNaN(cameraPos.y) && !float.IsNaN(cameraPos.z) &&
                !float.IsInfinity(cameraPos.x) && !float.IsInfinity(cameraPos.y) && !float.IsInfinity(cameraPos.z))
            {
                transform.position = cameraPos;
                transform.LookAt(target.position + LookOffset, Vector3.up);
            }
        }
    }
}
