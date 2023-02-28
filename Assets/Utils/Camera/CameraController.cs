using System.Collections;
using UnityEngine;

namespace Utils
{
    public class CameraController : MonoBehaviour
    {
        public Vector3 targetPosition;
        public Vector3 targetRotation;
        public float moveTime = 1;
        public CameraState state;

        private void OnEnable()
        {
            StopAllCoroutines();
            if (state)
            {
                transform.position = state.position;
                transform.rotation = state.rotation;
                transform.localScale = state.scale;
            }
            StartCoroutine(MoveTo(targetPosition, targetRotation, moveTime));
        }

        private IEnumerator MoveTo(Vector3 targetPosition, Vector3 targetRotation, float moveTime)
        {
            Vector3 startPosition = transform.position;
            Quaternion startRotation = transform.rotation;
            float time = 0;

            while (time < moveTime)
            {
                time += Time.deltaTime;
                float t = time / moveTime;
                t = t * t * (3f - 2f * t);
                transform.position = Vector3.Lerp(startPosition, targetPosition, t);
                transform.rotation = Quaternion.Lerp(startRotation, Quaternion.Euler(targetRotation), t);
                yield return null;
            }

            transform.position = targetPosition;
        }
    }
}
