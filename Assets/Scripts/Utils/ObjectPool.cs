using System.Collections.Generic;
using UnityEngine;

namespace SurvivorDemo.Utils
{
    /// <summary>
    /// 泛型对象池 ObjectPool<T>
    /// </summary>
    public class ObjectPool<T> where T : Component
    {
        private readonly T _prefab;               
        private readonly Transform _parent;       
        private readonly Stack<T> _inactiveStack; 
        private readonly List<T> _activeList;     

        public ObjectPool(T prefab, int prewarmCount = 0, Transform parent = null)
        {
            _prefab = prefab;
            _parent = parent;
            _inactiveStack = new Stack<T>();
            _activeList = new List<T>();

            // 预先实例化若干对象，避免第一次使用卡顿
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
            _activeList.Add(item);
            return item;
        }

        //把对象放回池中
        public void Despawn(T item)
        {
            if (item == null) return;
            item.gameObject.SetActive(false);
            _activeList.Remove(item);
            _inactiveStack.Push(item);
        }

        //回收全部激活对象
        public void DespawnAll()
        {
            for (int i = _activeList.Count - 1; i >= 0; i--)
            {
                if (_activeList[i] == null) 
                { 
                    _activeList.RemoveAt(i); 
                    continue; 
                }
                _activeList[i].gameObject.SetActive(false);
                _inactiveStack.Push(_activeList[i]);
                _activeList.RemoveAt(i);
            }
        }

        private T CreateInstance()
        {
            T obj = Object.Instantiate(_prefab, _parent);
            return obj;
        }
    }
}
