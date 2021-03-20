using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DotBPE.BestPractice
{
    public interface IBizEventProcessor
    {
        string ProcessorEventName { get; }
        Task<TaskResult> ProcessAsync(IBizEvent @event);
    }

    public abstract class AbsBizEventProcessor<TEvent> : IBizEventProcessor where TEvent:class,IBizEvent
    {
        public string ProcessorEventName => typeof(TEvent).Name;

        public Task<TaskResult> ProcessAsync(IBizEvent @event)
        {
            var data = @event as TEvent;
            if(data == null)
            {
                throw new ArgumentException($"错误的IBizEvent Type{@event.GetType()}");
            }
            return HandlerEvent(data);
        }


        protected abstract Task<TaskResult> HandlerEvent(TEvent @event);
    }
}
