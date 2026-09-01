using UnityEngine;
using SurvivorDemo.Managers;

namespace SurvivorDemo.Gameplay
{
    public class PlayerStateMachine : MonoBehaviour
    {
        private IPlayerState _current;

        public void ChangeState(IPlayerState newState)
        {
            _current?.Exit();
            _current = newState;
            _current?.Enter(this);
        }

        private void Update()
        {
            _current?.Update(this);
        }

        public void SetIdle() => ChangeState(new PlayerIdleState());
        public void SetMove(Vector2 dir) => ChangeState(new PlayerMoveState(dir));
        public void SetAttack() => ChangeState(new PlayerAttackState());
        public void SetHurt() => ChangeState(new PlayerHurtState());
        public void SetDead() => ChangeState(new PlayerDeadState());
    }
    public interface IPlayerState
    {
        void Enter(PlayerStateMachine sm);
        void Update(PlayerStateMachine sm);
        void Exit();
    }

    public class PlayerIdleState : IPlayerState
    {
        public void Enter(PlayerStateMachine sm) { }
        public void Update(PlayerStateMachine sm) { }
        public void Exit() { }
    }

    public class PlayerMoveState : IPlayerState
    {
        private readonly Vector2 _dir;
        public PlayerMoveState(Vector2 dir) { _dir = dir; }
        public void Enter(PlayerStateMachine sm) { }
        public void Update(PlayerStateMachine sm) { Object.FindObjectOfType<PlayerController>()?.Move(_dir); }
        public void Exit() { }
    }

    public class PlayerAttackState : IPlayerState
    {
        public void Enter(PlayerStateMachine sm) { }
        public void Update(PlayerStateMachine sm) { Object.FindObjectOfType<PlayerController>()?.Attack(); }
        public void Exit() { }
    }

    public class PlayerHurtState : IPlayerState
    {
        private float _timer;
        public void Enter(PlayerStateMachine sm) { _timer = 0.2f; }
        public void Update(PlayerStateMachine sm)
        {
            _timer -= Time.deltaTime;
            if (_timer <= 0f) sm.SetIdle();
        }
        public void Exit() { }
    }

    public class PlayerDeadState : IPlayerState
    {
        public void Enter(PlayerStateMachine sm) { }
        public void Update(PlayerStateMachine sm) { }
        public void Exit() { }
    }
}
