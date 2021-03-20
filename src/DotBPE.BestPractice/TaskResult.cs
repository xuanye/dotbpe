using System;
using System.Collections.Generic;
using System.Text;

namespace DotBPE.BestPractice
{
    public class TaskResult
    {
        public static TaskResult Success = new TaskResult();
        public int Code { get; set; }

        public string Message { get; set; }
    }
}
