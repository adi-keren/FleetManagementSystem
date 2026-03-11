
using System.Collections.Concurrent;
using FleetManagement.Common.Models;
using FleetManagement.HQ.Interfaces;

namespace FleetManagement.HQ.Services
{
    public class CommandQueue : ICommandQueue
    {
        private readonly ConcurrentDictionary<string, ConcurrentQueue<Request>> _queues = new();

        public void Enqueue(string shipId, Request request)
        {
            // Get or create the queue for the ship and enqueue the request
            var queue = _queues.GetOrAdd(shipId, _ => new ConcurrentQueue<Request>());
            queue.Enqueue(request);
        }

        public bool TryDequeue(string shipId, out Request? request)
        {
            // If the ship has a queue, try to dequeue a request
            if (_queues.TryGetValue(shipId, out var queue))
            {
                return queue.TryDequeue(out request);
            }
            request = null;
            return false;
        }
    }
}