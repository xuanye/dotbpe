using System;
using System.Collections.Generic;
using System.Text;

namespace DotBPE.BestPractice
{
    public class BizEventWrapper
    {
        public BizEventWrapper(IBizEvent e)
        {
            this.EventData = e;
            this.EventName = e.Name;
        }

        public string EventName { get; set; }
        public IBizEvent EventData { get; set; }
    }
}
