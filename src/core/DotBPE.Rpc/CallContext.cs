using System.Collections.Generic;

namespace DotBPE.Rpc
{
    public class CallContext
    {
        public CallContext(){
            RequestHeader = new Dictionary<string,string>();
        }
        /// <summary>
        /// 本次调用经过的每一个节点
        /// </summary>
        /// <returns></returns>
        public string Peer{get;set;}

        /// <summary>
        ///  用于调用返回的状态，暂时没用
        /// </summary>
        /// <returns></returns>
        public int Status {get;set;}

        /// <summary>
        /// 由客户端调用者设置的头信息
        /// </summary>
        /// <returns></returns>
        public Dictionary<string,string> RequestHeader{get;private set;}

        /// <summary>
        /// 终端客户端
        /// </summary>
        /// <returns></returns>
        public string ClientId{
            get{
                if(this.RequestHeader.ContainsKey("CLIENT_IP")){
                    return this.RequestHeader["CLIENT_IP"];
                }
                return "";
            }
        }
    }
}