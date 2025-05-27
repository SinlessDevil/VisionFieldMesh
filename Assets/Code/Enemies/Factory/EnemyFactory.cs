using UnityEngine;
using Zenject;

namespace Code.Enemies.Factory
{
    public class EnemyFactory : Infrastructure.Factory.Factory, IEnemyFactory
    {
        private const string EnemiesPrefabPath = "Enemies/Enemy";
        
        public EnemyFactory(IInstantiator instantiator) : base(instantiator)
        {
            
        }

        public Enemy CreateEnemy(Vector3 position)
        {
            var gameObject = Instantiate(EnemiesPrefabPath, position, Quaternion.identity, null);
            var enemy = gameObject.GetComponent<Enemy>();
            return enemy;
        }
    }   
}