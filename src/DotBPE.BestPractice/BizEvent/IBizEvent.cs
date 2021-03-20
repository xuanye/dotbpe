using System;
using System.Collections.Generic;
using System.Text;

namespace DotBPE.BestPractice
{
    public interface IBizEvent
    {
        string Name { get; }
    }

    public class BaseBizEvent : IBizEvent
    {
        public virtual string Name
        {
            get {
                return this.GetType().FullName;
            }
        }
    }
}
