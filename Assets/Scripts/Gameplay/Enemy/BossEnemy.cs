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

        [Header("Boss 受击")]
        public float damageReduction = 0.5f;  

        private float _attackCd;
        private bool _enraged;               

        private void Start()
        {
            player = GameObject.FindGameObjectWithTag("Player")?.transform;
        }

        private void Update()
        {
            if (player == null) return;
            _attackCd -= Time.deltaTime;

            float range = _enraged ? attack2Range : attackRange;
            float dmg   = _enraged ? attack2Damage : attack1Damage;

            if (Vector2.Distance(transform.position, player.position) <= range && _attackCd <= 0f)
            {
                anim?.SetTrigger(_enraged ? "attack2" : "attack1");
                _attackCd = 2.0f;

                PlayerController pc = player.GetComponent<PlayerController>();
                if (pc != null)
                {
                    pc.TakeDamage(dmg);
                }                   
            }
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
