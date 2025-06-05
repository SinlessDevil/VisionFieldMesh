using System;
using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Code.Weapon
{
    public class WeaponAI : MonoBehaviour
    {
        [SerializeField] private Weapon _weapon;

        private Enemy _currentTarget;
        private CancellationTokenSource _cancellationToken;

        private static readonly Vector3 WeaponIdleRotation = new(-90f, 3f, 0f);
        private static readonly Vector3 WeaponAimRotation = new(0, 0f, 0f);

        private void Awake()
        {
            IdleState().Forget();
        }
        
        public void StartFiring(Enemy enemy)
        {
            if (_currentTarget == enemy)
                return;

            _currentTarget = enemy;
            _currentTarget.SetAnimationDetected();
            
            _cancellationToken?.Cancel();
            _cancellationToken = new CancellationTokenSource();

            HandleFireSequenceAsync(_cancellationToken.Token).Forget();
        }

        public void StopFiring(Enemy enemy)
        {
            if (_currentTarget != enemy)
                return;

            _cancellationToken?.Cancel();
            _currentTarget = null;
            
            IdleState().Forget();
        }

        private async UniTaskVoid HandleFireSequenceAsync(CancellationToken token)
        {
            try
            {
                await ReadyFollowToTargetState(token);
                await ShootTargetState(token);
                await IdleState();
            }
            catch (OperationCanceledException)
            {
                Debug.Log("WeaponAI: Fire sequence cancelled.");
            }
        }

        private async UniTask IdleState()
        {
            Quaternion targetRot = Quaternion.Euler(WeaponIdleRotation);
            await RotateTo(_weapon.Pivot.transform, targetRot, 0.15f);
            _weapon.Play();
        }

        private async UniTask ReadyFollowToTargetState(CancellationToken token)
        {
            _weapon.Stop();
            Quaternion targetRot = Quaternion.Euler(WeaponAimRotation);
            await RotateTo(_weapon.Pivot.transform, targetRot, 0.15f, token);
            await FollowToTarget(token);
        }

        private async Task FollowToTarget(CancellationToken token)
        {
            float followTime = 0.5f;
            float elapsed = 0f;

            while (_currentTarget != null && !token.IsCancellationRequested && elapsed < followTime)
            {
                Vector3 toTarget = _currentTarget.transform.position - _weapon.Pivot.transform.position;
                toTarget.y = 0f;
                if (toTarget != Vector3.zero)
                {
                    Quaternion lookRot = Quaternion.LookRotation(toTarget.normalized);
                    Vector3 euler = lookRot.eulerAngles;

                    Quaternion finalRot = Quaternion.Euler(0, euler.y, 0);
                    _weapon.Pivot.transform.rotation = Quaternion.Slerp(
                        _weapon.Pivot.transform.rotation,
                        finalRot,
                        Time.deltaTime * 5f
                    );
                }

                await UniTask.Yield(token);
                elapsed += Time.deltaTime;
            }
        }

        private async UniTask ShootTargetState(CancellationToken token)
        {
            _weapon.PlayShootEffect();
            _currentTarget.SetDead();
            _currentTarget = null;
            await UniTask.Delay(250, cancellationToken: token);
        }

        private async UniTask RotateTo(Transform target, Quaternion to, float duration, CancellationToken token = default)
        {
            Quaternion from = target.rotation;
            float time = 0f;

            while (time < duration)
            {
                time += Time.deltaTime;
                float t = Mathf.Clamp01(time / duration);
                target.rotation = Quaternion.Slerp(from, to, t);
                await UniTask.Yield(token);
            }

            target.rotation = to;
        }
    }
}
