using UnityEngine;

namespace SurvivorDemo.Gameplay
{
 
    public class BossEnemy : EnemyBase
    {
        [Header("Boss 攻击模组")]
        public float attackRange = 3f;        
        public float attack2Range = 4f;       
        public float attack1Damage = 10f;     
        public float attack2Damage = 25f;     
        public float attackAnimLength = 1.6f;

        [Header("Boss 受击")]
        public float damageReduction = 0.5f;  

        private float _attackCd;
        private bool _enraged;               
        private bool _attacking;

        private void Start()
        {
            player = GameObject.FindGameObjectWithTag("Player")?.transform;
        }

        private void Update()
        {
            if (player == null) return;
            _attackCd -= Time.deltaTime;

            float range = _enraged ? attack2Range : attackRange;

            if (!_attacking && Vector2.Distance(transform.position, player.position) <= range && _attackCd <= 0f)
            {
                _attackCd = 2.0f;
                _attacking = true;
                anim?.SetTrigger(_enraged ? "attack2" : "attack1");

                StartCoroutine(ResetAttackGuard());
            }
        }

        public void DealDamage()
        {
            if (player == null) return;

            float range = _enraged ? attack2Range : attackRange;
            if (Vector2.Distance(transform.position, player.position) > range)
                return;

            float dmg = _enraged ? attack2Damage : attack1Damage;
            PlayerController pc = player.GetComponent<PlayerController>();
            if (pc != null) pc.TakeDamage(dmg);
        }

        public void AttackEnd()
        {
            _attacking = false;
        }

        private System.Collections.IEnumerator ResetAttackGuard()
        {
            yield return new WaitForSeconds(attackAnimLength);
            _attacking = false;
        }

        protected override void OnCollisionEnter2D(Collision2D collision)
        {
            
        }

        public override void TakeDamage(float dmg)
        {
            base.TakeDamage(dmg * damageReduction);

            if (!_enraged && config != null && hp <= config.maxHp * 0.5f)
            {
                _enraged = true;
            }
        }

    }
}
