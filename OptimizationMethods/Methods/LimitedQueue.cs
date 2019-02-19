using System.Collections.Generic;

namespace OptimizationMethods
{
    class LimitedQueue<Item>
    {
        int maxSize;
        public LimitedQueue(int size)
        {
            maxSize = size;
            queue = new Queue<Item>();
        }
        public void Add(Item item)
        {
            queue.Enqueue(item);
            if (queue.Count > maxSize)
                queue.Dequeue();
        }
        public List<Item> ToList()
        {
            return new List<Item>(queue);
        }
        Queue<Item> queue;
    }
}
