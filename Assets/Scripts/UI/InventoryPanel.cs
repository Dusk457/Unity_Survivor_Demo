using UnityEngine;
using UnityEngine.UI;
using TMPro;
using SurvivorDemo.Data;
using SurvivorDemo.Managers;
using SurvivorDemo.Gameplay;

namespace SurvivorDemo.UI
{
    public class InventoryPanel : PanelBase
    {
        [Header("列表")]
        public Transform contentRoot;
        public GameObject itemPrefab;

        [Header("关闭")]
        public Button closeButton;

        private WeaponSystem _ws;

        public override void OnShow(System.Action onClosed = null)
        {
            base.OnShow(onClosed);
            Refresh();
        }

        public override void Refresh()
        {
            if (_ws == null)
            {
                _ws = FindObjectOfType<WeaponSystem>();
            }

            if (_ws == null || contentRoot == null || itemPrefab == null) return;

            for (int i = contentRoot.childCount - 1; i >= 0; i--)
            {
                Destroy(contentRoot.GetChild(i).gameObject);
            }
               
            string current = _ws.CurrentWeaponId;
            foreach (var id in _ws.OwnedWeaponIds)
            {
                GameObject go = Instantiate(itemPrefab, contentRoot);
                go.SetActive(true);
                var label = go.GetComponentInChildren<TextMeshProUGUI>();
                bool isCurrent = id == current;
                if (label != null)
                {
                    label.text = (isCurrent ? "[当前] " : "") + GetWeaponName(id);
                }
                    
                string weaponId = id;
                var btn = go.GetComponent<Button>();
                if (btn != null)
                {
                    btn.onClick.AddListener(() =>
                    {
                        _ws.SelectWeapon(weaponId);
                        UIManager.Instance.Hide("InventoryPanel");
                    });
                }
            }
        }

        private string GetWeaponName(string id)
        {
            var w = ConfigLoader.GetWeapon(id);
            return w != null ? w.name : id;
        }

        private void Start()
        {
            if (closeButton != null)
            {
                closeButton.onClick.AddListener(OnClose);
            }
        }

        private void OnClose()
        {
            UIManager.Instance.Hide("InventoryPanel");
        }
    }
}
