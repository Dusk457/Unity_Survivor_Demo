using UnityEngine;
using SurvivorDemo.Managers;

namespace SurvivorDemo.Gameplay
{
    public class WeaponSystem : MonoBehaviour
    {
        [SerializeField] private string[] ownedWeaponIds = { "bullet", "tripleShot" };

        private int _currentIndex;

        public string CurrentWeaponId => ownedWeaponIds[_currentIndex];
        public string[] OwnedWeaponIds => ownedWeaponIds;

        private void Awake()
        {
            if (ownedWeaponIds == null || ownedWeaponIds.Length == 0)
                ownedWeaponIds = new[] { "bullet" };
            _currentIndex = 0;
        }

        public void SelectWeapon(string id)
        {
            for (int i = 0; i < ownedWeaponIds.Length; i++)
            {
                if (ownedWeaponIds[i] == id)
                {
                    _currentIndex = i;
                    EventManager.Instance.Emit(new WeaponChangedEvent(id));
                    return;
                }
            }
            Debug.LogWarning($"[WeaponSystem] 未拥有武器 {id}");
        }

        public void CycleWeapon()
        {
            _currentIndex = (_currentIndex + 1) % ownedWeaponIds.Length;
            EventManager.Instance.Emit(new WeaponChangedEvent(CurrentWeaponId));
        }
    }
}
