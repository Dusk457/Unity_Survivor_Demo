using UnityEngine;
using SurvivorDemo.Managers;

namespace SurvivorDemo.UI
{
    public abstract class PanelBase : MonoBehaviour
    {
        [Tooltip("面板优先级，越小越靠下")]
        public int sortOrder = 0;
        public System.Action OnClosed;

        public virtual void OnShow(System.Action onClosed = null)
        {
            OnClosed = onClosed;
            gameObject.SetActive(true);
            Refresh();
        }

        public virtual void OnHide()
        {
            OnClosed?.Invoke();
            OnClosed = null;
            gameObject.SetActive(false);
        }

        public virtual void Refresh() { }

        protected virtual void OnValidate()
        {

        }
    }
}
