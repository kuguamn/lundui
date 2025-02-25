﻿using Microsoft.AspNetCore.Http;
using System.Net;

namespace Yuebon.Commons.Net
{
    public class RemoteIpParser : IIpAddressParser
    {
        public bool ContainsIp(string ipRule, string clientIp)
        {
            return IpAddressUtil.ContainsIp(ipRule, clientIp);
        }

        public bool ContainsIp(List<string> ipRules, string clientIp)
        {
            return IpAddressUtil.ContainsIp(ipRules, clientIp);
        }

        public bool ContainsIp(List<string> ipRules, string clientIp, out string rule)
        {
            return IpAddressUtil.ContainsIp(ipRules, clientIp, out rule);
        }

        public virtual IPAddress GetClientIp(HttpContext context)
        {
            return context.Connection.RemoteIpAddress;
        }

        public IPAddress ParseIp(string ipAddress)
        {
            return IpAddressUtil.ParseIp(ipAddress);
        }
    }
}
