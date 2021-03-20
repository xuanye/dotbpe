using System;
using System.Collections.Generic;
using System.Text;

namespace DotBPE.BestPractice
{
    public class BizEventOptions
    {
        /// <summary>
        /// 事件处理DLL的名称通配符 Abc.Aa.*.dll
        /// </summary>
        public string EventProcessDllPattern { get; set; } = "*.dll";
    }
}
