using UnityEngine;
using SurvivorDemo.Utils;

namespace SurvivorDemo.Gameplay
{
    public class Projectile : MonoBehaviour
    {
        private float _damage;
        private float _speed;
        private float _range;
        private Vector2 _origin;
        private ObjectPool<Projectile> _pool;

        private Rigidbody2D _rb;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
        }

        public void Setup(float damage, float speed, float range, ObjectPool<Projectile> pool)
        {
            _damage = damage;
            _speed = speed;
            _range = range;
            _pool = pool;
            _origin = transform.position;
            _rb.velocity = transform.right * speed;
        }

        private void Update()
        {
            if (Vector2.Distance(_origin, transform.position) > _range)
                Expire();
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            EnemyBase enemy = other.GetComponentInParent<EnemyBase>();
            if (enemy != null)
            {
                enemy.TakeDamage(_damage);
                Expire();
            }
        }

        private void Expire()
        {
            CancelInvoke();
            _pool?.Despawn(this);
        }
    }
}
