using DotBPE.Rpc.Codes;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace DotBPE.Rpc.DefaultImpls
{
    public abstract class AbstractBridgeRouter<TMessage>:IBridgeRouter<TMessage> where TMessage:InvokeMessage
    {
        private readonly static Dictionary<string, int> chooseRandom = new Dictionary<string, int>();
        private readonly object lockObject = new object();

        public abstract RouterPoint GetRouterPoint(TMessage message);
        


        /// <summary>
        /// 轮询选择一个远端节点
        /// 项目中可能需要更加复杂的计算算法,可以考虑服务的负载情况，或者可以设置权重什么的
        /// </summary>
        /// <param name="key">服务标识 serviceId$0</param>
        /// <param name="list">该服务标识所有的服务地址</param>
        /// <returns>选中的服务地址</returns>
        protected virtual EndPoint ChooseEndPoint(string key, List<EndPoint> list)
        {
            int chooseIndex = 0;
            lock (lockObject)
            {
                if (!chooseRandom.ContainsKey(key))
                {
                    chooseRandom.Add(key, chooseIndex);
                }
                else
                {
                    chooseIndex = chooseRandom[key]+1;
                    if (chooseIndex >= list.Count)
                    {
                        chooseIndex = 0;
                    }
                    chooseRandom[key] = chooseIndex;
                }
            }
            return list[chooseIndex];
        }
    }
}
