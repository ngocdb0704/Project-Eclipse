using System;
using System.Collections.Generic;
using UnityEngine;

namespace Eclipse.Core.Pooling
{
    public sealed class GameObjectPool<T>
        where T : Component
    {
        private readonly T prefab;
        private readonly Transform poolParent;

        private readonly Stack<T> inactiveInstances =
            new Stack<T>();

        private readonly HashSet<T> activeInstances =
            new HashSet<T>();

        public int CountActive =>
            activeInstances.Count;

        public int CountInactive =>
            inactiveInstances.Count;

        public int CountAll =>
            CountActive + CountInactive;

        public GameObjectPool(
            T prefab,
            Transform poolParent = null)
        {
            if (prefab == null)
            {
                throw new ArgumentNullException(
                    nameof(prefab));
            }

            this.prefab = prefab;
            this.poolParent = poolParent;
        }

        public void Prewarm(
            int targetCount)
        {
            if (targetCount < 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(targetCount),
                    "Prewarm count must be non-negative.");
            }

            while (CountAll < targetCount)
            {
                T instance =
                    CreateInstance();

                inactiveInstances.Push(
                    instance);
            }
        }

        public T Get()
        {
            T instance =
                AcquireInstance();

            ActivateInstance(
                instance);

            return instance;
        }

        public T Get(
            Vector3 position,
            Quaternion rotation)
        {
            T instance =
                AcquireInstance();

            instance.transform.SetPositionAndRotation(
                position,
                rotation);

            ActivateInstance(
                instance);

            return instance;
        }

        public bool Release(
            T instance)
        {
            // false means:
            // - null instance,
            // - instance from another pool,
            // - or an already released instance.
            if (instance == null ||
                !activeInstances.Remove(instance))
            {
                return false;
            }

            if (instance is IPoolable poolable)
            {
                poolable.OnDespawned();
            }

            instance.gameObject.SetActive(false);

            if (poolParent != null)
            {
                instance.transform.SetParent(
                    poolParent,
                    false);
            }

            inactiveInstances.Push(
                instance);

            return true;
        }

        private T AcquireInstance()
        {
            T instance =
                inactiveInstances.Count > 0
                    ? inactiveInstances.Pop()
                    : CreateInstance();

            if (!activeInstances.Add(instance))
            {
                throw new InvalidOperationException(
                    "Pool attempted to activate an already active instance.");
            }

            return instance;
        }

        private void ActivateInstance(
            T instance)
        {
            instance.gameObject.SetActive(true);

            if (instance is IPoolable poolable)
            {
                poolable.OnSpawned();
            }
        }

        private T CreateInstance()
        {
            T instance =
                UnityEngine.Object.Instantiate(
                    prefab,
                    poolParent);

            instance.gameObject.SetActive(false);

            return instance;
        }
    }
}