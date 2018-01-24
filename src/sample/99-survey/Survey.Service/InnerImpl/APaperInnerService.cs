using DotBPE.Rpc;
using DotBPE.Rpc.Client;
using Survey.Core;
using Survey.Service.InnerImpl.Domain;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Vulcan.DataAccess;


namespace Survey.Service.InnerImpl
{
    public class APaperInnerService : APaperInnerServiceBase
    {
        private readonly Repository.APaperRepository _apaperRepo;
        private readonly ClientProxy _proxy;

        public APaperInnerService(Repository.APaperRepository apaperRepo,ClientProxy proxy)
        {
            _apaperRepo = apaperRepo;
            _proxy = proxy;
        }

        /// <summary>
        /// 获取答卷详情
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public override async Task<RpcResult<APaperRsp>> GetAPaperAsync(GetAPaperReq request)
        {
            var res = new RpcResult<APaperRsp>();
            res.Data = new APaperRsp();
            var apaper = await this._apaperRepo.GetAPaper(request.PaperId);

            if (apaper == null) // 问卷不存在
            {
                res.Code = ErrorCodes.DATA_NOT_FOUND;
                res.Data.ReturnMessage = "答卷不存在";
                return res;
            }

            var answers = await this._apaperRepo.GetAnswersByPaperId(request.PaperId);

            var qpaperService = _proxy.GetClient<QPaperInnerServiceClient>();
            GetQPaperReq getQPRep = new GetQPaperReq();
            getQPRep.XRequestId = request.XRequestId;
            getQPRep.Identity = request.Identity;
            getQPRep.ClientIp = request.ClientIp;
            getQPRep.CheckRole = request.CheckRole;
            getQPRep.QpaperId = apaper.QpaperId;
            var getQPRes = await qpaperService.GetQPaperFullAsync(getQPRep);

            if (getQPRes.Code != 0)
            {
                res.Code = getQPRes.Code;

                if (getQPRes.Data != null)
                {
                    res.Data.ReturnMessage = getQPRes.Data.ReturnMessage;
                }
                return res;
            }

            //答卷信息
            res.Data.Apaper = new DTOAPaperWithAnswers();
            res.Data.Apaper.ApaperId = apaper.PaperId;
            res.Data.Apaper.CreateTime = apaper.CreateTime.ToString("yyyy-MM-dd HH:mm:ss");
            res.Data.Apaper.QpaperId = apaper.QpaperId;
            res.Data.Apaper.UserId = apaper.UserId;          
            res.Data.Apaper.Remark = apaper.Remark??"";
            //rsp.Apaper.Answers
            if (answers != null && answers.Count > 0)
            {
                foreach (var a in answers)
                {
                    res.Data.Apaper.Answers.Add(new DTOAnswer()
                    {
                        AnswerId = a.AnswerId,
                        ApaperId = a.ApaperId,
                        ObjectiveAnswer = a.ObjectiveAnswer,
                        SubjectiveAnswer = a.SubjectiveAnswer,
                        QuestionId = a.QuestionId
                    });
                }
            }
            //rsp.Apaper.Answers;
            //问卷数据
            res.Data.Qpaper = getQPRes.Data.Qpaper;

            return res;
        }

        public override async Task<RpcResult<APaperStaDetailRsp>> GetAPaperStaAsync(GetAPaperStaDetailReq request)
        {
            var res = new RpcResult<APaperStaDetailRsp>();
            res.Data = new APaperStaDetailRsp();


            var staList = await this._apaperRepo.QueryQPaperStaDetailAsync(request.QpaperId);


            foreach(var item in staList)
            {
                var stad = new APaperStaDetail()
                {
                    QuestionId = item.QuestionId
                };
                stad.Oa.AddRange(new int[]
                {
                    item.OA1, item.OA2,item.OA3,item.OA4,item.OA5,item.OA6,item.OA7,item.OA8,item.OA9,item.OA10
                });
                res.Data.StaDetail.Add(stad);
            }

            return res;
        }

        /// <summary>
        /// 获取答卷列表
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public override async Task<RpcResult<APaperListRsp>> QueryAPaperListAsync(QueryAPaperReq request)
        {
            var res = new RpcResult<APaperListRsp>();
            res.Data = new APaperListRsp();

            var view = new PageView(request.Page, request.Rp);

            var plist = await this._apaperRepo.QueryAPaperList(request.Qtext, request.QpaperId, request.CheckRole ? request.Identity : "", view);

            res.Data.Total = view.PageIndex == 0 ? plist.Total : -1;

            if (plist != null && plist.DataList != null && plist.DataList.Count > 0)
            {
                foreach (var apaper in plist.DataList)
                {
                    res.Data.List.Add(new DTOAPaper()
                    {
                        ApaperId = apaper.PaperId,
                        CreateTime = apaper.CreateTime.ToString("yyyy-MM-dd HH:mm:ss"),
                        QpaperId = apaper.QpaperId,
                        UserId = apaper.UserId,
                        QpaperSubject = apaper.QpaperSubject??"",
                        QpaperUserId = apaper.QpaperUserId??"",
                        Remark = apaper.Remark
                    });
                }
            }
            return res;
        }

        /// <summary>
        /// 保存答卷信息
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public override async Task<RpcResult<SaveAPaperRsp>> SaveAPaperAsync(SaveAPaperReq req)
        {
            var res = new RpcResult<SaveAPaperRsp>();
            res.Data = new SaveAPaperRsp();

            if (req.Answers.Count == 0)
            {
                res.Code = ErrorCodes.PARAMS_VALIDATION_FAIL;
                res.Data.ReturnMessage = "答案不能为空";
                return res;
            }
            var qpaperService = _proxy.GetClient<QPaperInnerServiceClient>();
            GetQPaperReq getQPRep = new GetQPaperReq();
            getQPRep.XRequestId = req.XRequestId;
            getQPRep.Identity = req.Identity;
            getQPRep.ClientIp = req.ClientIp;
            getQPRep.CheckRole = false;
            getQPRep.QpaperId = req.QpaperId;

            var getQPRes = await qpaperService.GetQPaperAsync(getQPRep);

            if (getQPRes.Code != 0)
            {
                res.Code = getQPRes.Code;
                if(getQPRes.Data != null)
                {
                    res.Data.ReturnMessage = getQPRes.Data.ReturnMessage;
                }
                
                return res;
            }
            if(getQPRes.Data == null)
            {
                res.Code = ErrorCodes.DATA_NOT_FOUND;
                res.Data.ReturnMessage = "问卷不存在";
                return res;
            }

            using (TransScope scope = this._apaperRepo.BeginTransScope())
            {

               

                var apaper = new APaper();
                apaper.CreateTime = DateTime.Now;
                apaper.QpaperId = req.QpaperId;
                apaper.QpaperSubject = getQPRes.Data.Subject;
                apaper.QpaperUserId = getQPRes.Data.CreateUserId;
                apaper.Remark = req.Remark;
                apaper.UserId = string.IsNullOrEmpty(req.UserId) ? req.Identity : req.UserId;

                bool hasA = await this._apaperRepo.CheckAPaperAsync(apaper.QpaperId, apaper.UserId);

                if (hasA)
                {
                    res.Code = ErrorCodes.BIZ_RULE_FAIL;
                    res.Data.ReturnMessage = "该用户已经回答过该问卷了";
                    return res;
                }

                var newId = await this._apaperRepo.InsertAsync(apaper);

                if (newId < 0)
                {
                    res.Code = ErrorCodes.INTERNAL_ERROR;
                    res.Data.ReturnMessage = "新增问卷失败，请稍后重试";
                    return res;
                }
                var apaperId = (int)newId;

                var alist = new List<Answer>();
                foreach (var a in req.Answers)
                {
                    var answer = new Answer();
                    answer.AnswerId = Guid.NewGuid().ToString("N");
                    answer.ApaperId = apaperId;
                    answer.ObjectiveAnswer = a.ObjectiveAnswer;
                    answer.SubjectiveAnswer = a.SubjectiveAnswer;
                    answer.QuestionId = a.QuestionId;

                    alist.Add(answer);
                }
                await this._apaperRepo.AddAnswers(alist);
                res.Data.ApaperId = apaperId;

                scope.Complete();// 提交事务
            }


            // 异步执行不管是否成功
            var addReq = new AddAPaperReq();
            addReq.ClientIp = req.ClientIp;
            addReq.Identity = req.Identity;
            addReq.XRequestId = req.XRequestId;
            addReq.QpaperId = req.QpaperId;
            addReq.Count = 1;
#pragma warning disable CS4014 // 由于此调用不会等待，因此在调用完成前将继续执行当前方法
            qpaperService.AddAPaperCountAsync(addReq);
#pragma warning restore CS4014 // 由于此调用不会等待，因此在调用完成前将继续执行当前方法

            return res;
        }
    }
}
