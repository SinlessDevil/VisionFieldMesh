using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Code.Weapon
{
    public class WeaponAI : MonoBehaviour
    {
        [SerializeField] private Weapon _weapon;
        
        private Enemy _currentTarget;
        
        public void StartFiring(Enemy enemy)
        {
            if(Equals(_currentTarget, enemy))
                return;
            
            _currentTarget = enemy;
        }

        public void StopFiring(Enemy enemy)
        {
            if(_currentTarget != enemy)
                return;
            
            _currentTarget = null;
        }
        
        public async UniTask IdleState()
        {
            
        }

        public async UniTask FollowToTargetState()
        {
            
        }
        
        public async UniTask ShootTargetState()
        {
            
        }
    }
}