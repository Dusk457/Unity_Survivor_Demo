using UnityEngine;
using System.Collections;
using SurvivorDemo.Managers;
using SurvivorDemo.Data;


namespace SurvivorDemo.Gameplay
{
    /// <summary>
    /// EnemyBase：怪物基类
    /// </summary>
    public class EnemyBase : MonoBehaviour
    {
        protected EnemyConfig config;
        protected float hp;
        protected Transform player;
        protected Rigidbody2D rb;
        protected Animator anim;
        protected bool dead;

        protected virtual void Awake()
        {
            anim = GetComponent<Animator>();
        }

        public virtual void Init(EnemyConfig cfg, Transform target)
        {
            config = cfg;
            player = target;
            hp = cfg.maxHp;
            rb = GetComponent<Rigidbody2D>();
            dead = false;
        }

        protected virtual void FixedUpdate()
        {
            if (config == null || player == null || dead) return;
            Vector2 dir = ((Vector2)(player.position - transform.position)).normalized;
            rb.velocity = dir * config.moveSpeed;
        }
        
        public virtual void TakeDamage(float dmg)
        {
            if (dead) return;
            hp -= dmg;

            if (hp <= 0f) { Die(); return; }

            if (anim != null) anim.SetTrigger("hurt");
        }

        protected virtual void Die()
        {
            dead = true;
            if (anim != null) anim.SetTrigger("death");

            if (config != null) GameManager.Instance.AddScore(config.score);
            GameManager.Instance.AddKill();
            AudioManager.Instance?.PlayHit();

            if (rb != null) rb.velocity = Vector2.zero;
            Collider2D col = GetComponent<Collider2D>();
            if (col != null) col.enabled = false;

            StartCoroutine(DisableAfterAnim());
        }

        private IEnumerator DisableAfterAnim()
        {
            yield return new WaitForSeconds(0.6f);
            Destroy(gameObject);
        }

        protected virtual void OnCollisionEnter2D(Collision2D collision)
        {
            PlayerController pc = collision.collider.GetComponent<PlayerController>();
            if (pc != null && config != null && !dead)
                pc.TakeDamage(config.damage);
        }
    }
}
