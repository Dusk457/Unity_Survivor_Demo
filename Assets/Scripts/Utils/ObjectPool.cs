using System.Collections.Generic;
using UnityEngine;

namespace SurvivorDemo.Utils
{
    /// <summary>
    /// 泛型对象池 ObjectPool<T>
    /// 用一个 Stack 存空闲对象，一个 HashSet 记录"在用"对象（去重 + O(1) 防重复 Despawn）。
    /// </summary>
    public class ObjectPool<T> where T : Component
    {
        private readonly T _prefab;               
        private readonly Transform _parent;       
        private readonly Stack<T> _inactiveStack; 
        private readonly HashSet<T> _inUse;

        public ObjectPool(T prefab, int prewarmCount = 0, Transform parent = null)
        {
            _prefab = prefab;
            _parent = parent;
            _inactiveStack = new Stack<T>();
            _inUse = new HashSet<T>();

            for (int i = 0; i < prewarmCount; i++)
            {
                var obj = CreateInstance();
                obj.gameObject.SetActive(false);
                _inactiveStack.Push(obj);
            }
        }

        //从池中取一个对象
        public T Spawn(Vector3 position, Quaternion rotation)
        {
            T item = _inactiveStack.Count > 0
                ? _inactiveStack.Pop()
                : CreateInstance();

            item.transform.position = position;
            item.transform.rotation = rotation;
            item.gameObject.SetActive(true);
            _inUse.Add(item);
            return item;
        }

        //把对象放回池中
        public void Despawn(T item)
        {
            if (item == null) return;
            if (!_inUse.Remove(item)) return;
            item.gameObject.SetActive(false);
            _inactiveStack.Push(item);
        }

        //回收全部激活对象
        public void DespawnAll()
        {
            var snapshot = new List<T>(_inUse);
            foreach (var it in snapshot)
                Despawn(it);
        }

        private T CreateInstance()
        {
            T obj = Object.Instantiate(_prefab, _parent);
            return obj;
        }
    }
}
