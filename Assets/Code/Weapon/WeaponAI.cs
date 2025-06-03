using System;
using System.Threading;
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
        
        public void StartFiring(Enemy enemy)
        {
            if (_currentTarget == enemy)
                return;

            _currentTarget = enemy;

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
                await FollowToTargetState(token);
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
            await RotateTo(_weapon.Pivot.transform, targetRot, 0.25f);
        }

        private async UniTask FollowToTargetState(CancellationToken token)
        {
            Quaternion targetRot = Quaternion.Euler(WeaponAimRotation);
            await RotateTo(_weapon.Pivot.transform, targetRot, 0.25f, token);
        }

        private async UniTask ShootTargetState(CancellationToken token)
        {
            _weapon.PlayShootEffect();
            _currentTarget.SetDead();
            _currentTarget = null;
            await UniTask.Delay(1000, cancellationToken: token);
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
