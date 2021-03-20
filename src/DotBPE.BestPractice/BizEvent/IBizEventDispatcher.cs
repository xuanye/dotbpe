using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Foundatio.Messaging;

namespace DotBPE.BestPractice
{
    public interface IBizEventDispatcher
    {
        Task RaiseEvent(IBizEvent eventArgs);
    }

   
    
}
