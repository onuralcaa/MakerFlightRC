using UnityEngine;

namespace MakerFlightRC.Runtime.CameraRig
{
    public class CameraFollow : MonoBehaviour
    {
        [SerializeField] private Transform target;
        private const Vector3 RearOffset = new Vector3(0f, 3f, -10f);

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

            transform.position = target.position + RearOffset;
            transform.LookAt(target.position, Vector3.up);
        }
    }
}
