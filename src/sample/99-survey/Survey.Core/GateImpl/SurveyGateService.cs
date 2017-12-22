using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Survey.Core.GateImpl
{
    public class SurveyGateService : SurveyGateServiceBase
    {
        public override Task<APaperRsp> GetAPaperAsync(GetAPaperReq req)
        {
            throw new NotImplementedException();
        }

        public override Task<APaperListRsp> QueryAPaperListAsync(QueryAPaperReq req)
        {
            throw new NotImplementedException();
        }

        public override Task<SaveAPaperRsp> SaveAPaperAsync(SaveAPaperReq req)
        {
            throw new NotImplementedException();
        }
    }
}
