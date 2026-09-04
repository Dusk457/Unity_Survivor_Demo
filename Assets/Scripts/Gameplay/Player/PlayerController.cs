using UnityEngine;
using SurvivorDemo.Managers;
using SurvivorDemo.Data;
using SurvivorDemo.Utils;

namespace SurvivorDemo.Gameplay
{
    public class PlayerController : MonoBehaviour
    {
        [Header("属性配置")]
        public float moveSpeed = 5f;
        public float maxHp = 100f;

        [Header("武器")]
        public string initialWeaponId = "bullet";

        public float CurrentHp 
        { 
            get; 
            private set; 
        }

        private ObjectPool<Projectile> _bulletPool;
        private WeaponConfig _weapon;
        private float _fireTimer;

        private PlayerStateMachine _sm;
        private Transform _cachedTransform;
        private Animator _anim;
        private SpriteRenderer _sprite;
        private Camera _cam;
        private bool _dead;

        private void Awake()
        {
            _sm = GetComponent<PlayerStateMachine>();
            _cachedTransform = transform;
            _anim = GetComponent<Animator>();
            _sprite = GetComponentInChildren<SpriteRenderer>();
            _cam = Camera.main;
            CurrentHp = maxHp;
        }

        private void Start()
        {
            EventManager.Instance?.On<WeaponChangedEvent>(OnWeaponChanged);
            SetupWeapon(initialWeaponId);
        }

        private void Update()
        {
            if (_dead) return;
            if (GameManager.Instance == null || GameManager.Instance.State != E_GameState.Playing) return;
            _fireTimer -= Time.deltaTime;

            float h = Input.GetAxisRaw("Horizontal");
            float v = Input.GetAxisRaw("Vertical");
            Vector2 move = new Vector2(h, v).normalized;

            if (move.sqrMagnitude > 0f) _sm.SetMove(move);
            else _sm.SetIdle();

            if (_anim != null) 
            {
                _anim.SetFloat("Speed", move.sqrMagnitude > 0f ? 1f : 0f);
            }

            if (_sprite != null && Mathf.Abs(h) > 0.01f)
            {
                _sprite.flipX = h < 0f;
            }

            if (Input.GetMouseButton(0) || Input.GetKey(KeyCode.Space))
            {
                _sm.SetAttack();
            }
        }

        public void Move(Vector2 dir)
        {
            _cachedTransform.Translate(dir * moveSpeed * Time.deltaTime, Space.World);
        }

        public void SetupWeapon(string id)
        {
            _weapon = ConfigLoader.GetWeapon(id);
            if (_weapon == null)
            {
                Debug.LogWarning($"[Player] 武器 {id} 未配置");
                _weapon = ConfigLoader.GetWeapon("bullet");
            }
            if (_weapon == null) return;

            _fireTimer = 0f;
        }

        private void OnWeaponChanged(WeaponChangedEvent e)
        {
            SetupWeapon(e.WeaponId);
        }

        private void OnDestroy()
        {
            if (EventManager.Instance != null)
                EventManager.Instance.Off<WeaponChangedEvent>(OnWeaponChanged);
        }

        public void Attack()
        {
            if (_weapon == null) return;
            if (_fireTimer > 0f) return;

            _fireTimer = 1f / Mathf.Max(0.01f, _weapon.fireRate);
            SpawnProjectiles();
        }

        private void SpawnProjectiles()
        {
            if (_bulletPool == null)
            {
                Projectile proto = Resources.Load<Projectile>("Prefabs/Projectile");
                if (proto == null) 
                { 
                    Debug.LogWarning("[Player] 未找到 Projectile 预制体"); 
                    return; 
                }
                _bulletPool = new ObjectPool<Projectile>(proto, 10, transform.parent);
            }

            int count = Mathf.Max(1, _weapon.count);
            Vector2 aimDir = ((Vector2)(Camera.main.ScreenToWorldPoint(Input.mousePosition) - _cachedTransform.position)).normalized;
            float baseAngle = Mathf.Atan2(aimDir.y, aimDir.x) * Mathf.Rad2Deg;
            float totalSpread = _weapon.spreadAngle;

            for (int i = 0; i < count; i++)
            {
                float offset = count == 1 ? 0f
                    : Mathf.Lerp(-totalSpread / 2f, totalSpread / 2f, i / (float)(count - 1));
                float angle = baseAngle + offset;
                Vector2 dir = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad));

                Projectile p = _bulletPool.Spawn(_cachedTransform.position, Quaternion.Euler(0, 0, angle));
                p.Setup(_weapon.damage, _weapon.projectileSpeed, _weapon.range, _bulletPool);
            }

            AudioManager.Instance?.PlayShoot();
        }

        public void TakeDamage(float dmg)
        {
            if (CurrentHp <= 0f) return;
            CurrentHp -= dmg;
            CurrentHp = Mathf.Max(0f, CurrentHp);
            EventManager.Instance.Emit(new PlayerHpChangedEvent(CurrentHp, maxHp));

            _sm.SetHurt();

            if (CurrentHp <= 0f)
            {
                _dead = true;
                _sm.SetDead();
                if (_anim != null) _anim.SetTrigger("Dead");
                GameManager.Instance.GameOver();
            }
        }

        public void ResetPlayer()
        {
            CurrentHp = maxHp;
            _dead = false;
            _fireTimer = 0f;

            EventManager.Instance?.Emit(new PlayerHpChangedEvent(CurrentHp, maxHp));
            if (_anim != null)
            {
                _anim.Rebind();
                _anim.Update(0f);
            }
        }
    }
}
