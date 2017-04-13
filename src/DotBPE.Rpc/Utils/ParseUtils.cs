using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace DotBPE.Rpc.Utils
{
    public static class ParseUtils
    {
        public static EndPoint ParseEndPointFromString(string address)
        {
            if (string.IsNullOrEmpty(address))
            {
                throw new ArgumentNullException("格式化地址错误，参数为空:address");
            }
            string[] arr_add = address.Split(':');
            if(arr_add.Length != 2)
            {
                throw new ArgumentException($"格式化地址错误，参数为空:{address}");
            }
            try
            {
                IPEndPoint endpoint = new IPEndPoint(IPAddress.Parse(arr_add[0]), int.Parse(arr_add[1]));
                return endpoint;
            }
            catch(Exception ex)
            {
                throw new ArgumentException($"格式化地址错误，参数为空:{address}", ex);
            }
            
        }
    }
}
