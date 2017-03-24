using System.Collections.Concurrent;

namespace Map.Controls.Utilities
{
    internal class ObjectPool<T>
    {
        private readonly int maxSize = 100;
        private ConcurrentBag<T> objects;

        public ObjectPool()
        {
            objects = new ConcurrentBag<T>();
        }

        public ObjectPool(int maxSize)
        {
            this.maxSize = maxSize;
            objects = new ConcurrentBag<T>();
        }

        public T Acquire()
        {
            if (!objects.TryTake(out T @object))
            {
                return default(T);
            }
            return @object;
        }

        public void Release(T @object)
        {
            if (objects.Count <= maxSize)
                objects.Add(@object);
        }
    }
}
