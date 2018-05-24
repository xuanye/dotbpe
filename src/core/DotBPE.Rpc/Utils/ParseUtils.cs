using System;
using System.Collections.Generic;
using System.Net;

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
            if (arr_add.Length != 2)
            {
                throw new ArgumentException($"格式化地址错误，参数为空:{address}");
            }
            try
            {
                IPEndPoint endpoint = new IPEndPoint(IPAddress.Parse(arr_add[0]), int.Parse(arr_add[1]));
                return endpoint;
            }
            catch (Exception ex)
            {
                throw new ArgumentException($"格式化地址错误，参数为空:{address}", ex);
            }
        }

        public static string ParseEndPointToString(EndPoint endpoint)
        {
            IPEndPoint ip = endpoint as IPEndPoint;
            if (ip != null)
            {
                if (ip.Address.IsIPv4MappedToIPv6)
                {
                    return ip.Address.MapToIPv4().ToString() + ":" + ip.Port;
                }
                return ip.Address.ToString() + ":" + ip.Port;
            }
            if (endpoint != null)
            {
                return endpoint.Serialize().ToString();
            }
            return "";
        }

        public static List<EndPoint> ParseEndPointListFromString(string remoteAddress)
        {
            Preconditions.CheckArgument(!string.IsNullOrEmpty(remoteAddress), $"服务地址配置错误：{remoteAddress}");
            string[] arr_address = remoteAddress.Split(',');
            List<EndPoint> list = new List<EndPoint>();
            for (int i = 0; i < arr_address.Length; i++)
            {
                list.Add(ParseEndPointFromString(arr_address[i]));
            }
            return list;
        }

        public static string ParseEndPointToIPString(EndPoint endpoint)
        {
            IPEndPoint ip = endpoint as IPEndPoint;
            if (ip != null)
            {
                if (ip.Address.IsIPv4MappedToIPv6)
                {
                    return ip.Address.MapToIPv4().ToString();
                }
                return ip.Address.ToString();
            }
            return "";
        }

        public static InvokeMessageType ParseMessageType(byte byteValue)
        {
            InvokeMessageType type;
            switch (byteValue)
            {
                case 1:
                    type = InvokeMessageType.Request;
                    break;

                case 2:
                    type = InvokeMessageType.Response;
                    break;

                case 3:
                    type = InvokeMessageType.Notify;
                    break;

                default:
                    type = InvokeMessageType.Request;
                    break;
            }
            return type;
        }
    }
}
