using UnityEngine;

namespace MakerFlightRC.Runtime.CameraRig
{
    public class CameraFollow : MonoBehaviour
    {
        [SerializeField] private Transform target;
        [SerializeField] private Vector3 offset = new Vector3(0f, 3f, -7f);
        [SerializeField] private Vector3 eulerOffset = new Vector3(15f, 0f, 0f);
        [SerializeField] private float positionLerpSpeed = 8f;
        [SerializeField] private float rotationLerpSpeed = 8f;

        private void LateUpdate()
        {
            if (target == null)
            {
                return;
            }

            var desiredRotation = target.rotation * Quaternion.Euler(eulerOffset);
            var desiredPosition = target.position + desiredRotation * offset;

            transform.position = Vector3.Lerp(transform.position, desiredPosition, 1f - Mathf.Exp(-positionLerpSpeed * Time.deltaTime));
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(target.position - transform.position, Vector3.up), 1f - Mathf.Exp(-rotationLerpSpeed * Time.deltaTime));
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (rotationLerpSpeed < 0f)
            {
                rotationLerpSpeed = 0f;
            }

            if (positionLerpSpeed < 0f)
            {
                positionLerpSpeed = 0f;
            }
        }
#endif
    }
}
