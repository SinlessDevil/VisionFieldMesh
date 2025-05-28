using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Code.Players
{
    public class PlayerMover : MonoBehaviour
    {
        private float _rotationSpeed = 5f;

        public async UniTask MoveAlongPath(float moveSpeed, Vector3[] path)
        {
            Transform t = transform;

            for (int i = 0; i < path.Length - 1; i++)
            {
                Vector3 start = path[i];
                Vector3 end = path[i + 1];
                float distance = Vector3.Distance(start, end);
                float time = distance / moveSpeed;
                float timer = 0f;

                while (timer < time)
                {
                    timer += Time.deltaTime;
                    float tNorm = Mathf.Clamp01(timer / time);

                    Vector3 nextPos = Vector3.Lerp(start, end, tNorm);
                    t.position = nextPos;

                    Vector3 dir = (end - t.position).normalized;
                    dir.y = 0f;

                    if (dir != Vector3.zero)
                    {
                        Quaternion lookRot = Quaternion.LookRotation(dir);
                        t.rotation = Quaternion.Slerp(t.rotation, lookRot, _rotationSpeed * Time.deltaTime);
                    }

                    await UniTask.Yield();
                }
            }

            Debug.Log("Path completed.");
        }
    }   
}