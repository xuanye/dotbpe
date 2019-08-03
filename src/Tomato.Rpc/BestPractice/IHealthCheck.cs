using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Tomato.Rpc
{
    public interface IHealthCheck
    {
        Task<int> HealthCheck();       
    }
}
