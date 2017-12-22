using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Survey.Core.InnerImpl
{
    public class QPaperInnerService : QPaperInnerServiceBase
    {
        public override Task<QPaperRsp> GetQPaperAsync(GetQPaperReq request)
        {
            throw new NotImplementedException();
        }

        public override Task<QPaperListRsp> QueryQPaperListAsync(QueryQPaperReq request)
        {
            throw new NotImplementedException();
        }

        public override Task<SaveQPaperRsp> SaveQPaperAsync(SaveQPaperReq request)
        {
            throw new NotImplementedException();
        }
    }
}
