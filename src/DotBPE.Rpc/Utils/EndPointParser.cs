// Copyright (c) Xuanye Wong. All rights reserved.
// Licensed under MIT license

using Peach.Infrastructure;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace DotBPE.Rpc.Utils
{
    public static class EndPointParser
    {
        public static IPEndPoint ParseEndPointFromString(string address)
        {
            if (string.IsNullOrEmpty(address))
            {
                throw new ArgumentNullException("Formatted address error, parameter is empty:address");
            }
            string[] arr_add = address.Split(':');
            if (arr_add.Length != 2)
            {
                throw new ArgumentException($"Formatted address error, parameter is empty:{address}");
            }
            try
            {
                var endpoint = new IPEndPoint(IPAddress.Parse(arr_add[0]), int.Parse(arr_add[1]));
                return endpoint;
            }
            catch (Exception ex)
            {
                throw new ArgumentException($"Formatted address error,{address}", ex);
            }
        }

        public static string ParseEndPointToString(EndPoint endpoint)
        {
            if (endpoint is IPEndPoint ip)
            {
                if (ip.Address.IsIPv4MappedToIPv6)
                {
                    return ip.Address.MapToIPv4() + ":" + ip.Port;
                }
                return ip.Address + ":" + ip.Port;
            }
            if (endpoint != null)
            {
                return endpoint.Serialize().ToString();
            }
            return "";
        }



        public static List<IPEndPoint> ParseEndPointListFromString(string remoteAddress)
        {
            Preconditions.CheckArgument(!string.IsNullOrEmpty(remoteAddress), $"Service address configuration error：{remoteAddress}");
            string[] arr_address = remoteAddress.Split(',');
            var list = new List<IPEndPoint>();
            for (int i = 0; i < arr_address.Length; i++)
            {
                list.Add(ParseEndPointFromString(arr_address[i]));
            }
            return list;
        }

        public static string ParseEndPointToIPString(EndPoint endpoint)
        {
            if (endpoint is IPEndPoint ip)
            {
                if (ip.Address.IsIPv4MappedToIPv6)
                {
                    return ip.Address.MapToIPv4().ToString();
                }
                return ip.Address.ToString();
            }
            return "";
        }
    }
}
