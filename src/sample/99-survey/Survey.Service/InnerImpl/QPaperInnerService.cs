using DotBPE.Rpc;
using Google.Protobuf.Collections;
using Microsoft.Extensions.Logging;
using Survey.Core;
using Survey.Service.InnerImpl.Domain;
using Survey.Service.InnerImpl.Repository;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Vulcan.DataAccess;


namespace Survey.Service.InnerImpl
{
    public class QPaperInnerService : QPaperInnerServiceBase
    {
        private readonly QPaperRepository _qpaperRepo;
        private readonly ILogger<QPaperInnerService> _logger;

        public QPaperInnerService(QPaperRepository qpaperRepo,ILogger<QPaperInnerService> logger)
        {
            this._qpaperRepo = qpaperRepo;
            this._logger = logger;
        }



        public override async Task<RpcResult<VoidRsp>> AddAPaperCountAsync(AddAPaperReq request)
        {
            var res = new RpcResult<VoidRsp>();
            res.Data = new VoidRsp();

            await this._qpaperRepo.UpdateAPaperCountAsync(request.QpaperId, request.Count);

            return res;

        }

        /// <summary>
        /// 获取问卷本身信息，不包括问题
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        public override async Task<RpcResult<QPaperRsp>> GetQPaperAsync(GetQPaperReq req)
        {
            var res = new RpcResult<QPaperRsp>();
            res.Data = new QPaperRsp();
            

            var qpaper = await this._qpaperRepo.GetQPaper(req.QpaperId);

            if (qpaper != null)
            {
                //判断是否有权限
                if (req.CheckRole && qpaper.CreateUserId != req.Identity) //非同一个用户
                {
                    res.Code = ErrorCodes.DATA_NOT_FOUND;
                    return res;
                }

                res.Data.QpaperId = qpaper.QpaperId;
                res.Data.Subject = qpaper.Subject;

                if (qpaper.StartTime.HasValue)
                {
                    res.Data.StartTime = qpaper.StartTime.Value.ToString("yyyy-MM-dd HH:mm:ss");
                }
                if (qpaper.EndTime.HasValue)
                {
                    res.Data.EndTime = qpaper.EndTime.Value.ToString("yyyy-MM-dd HH:mm:ss");
                }
                res.Data.Description = qpaper.Description;
                res.Data.CreateUserId = qpaper.CreateUserId;
            }
            else
            {
                res.Code = ErrorCodes.DATA_NOT_FOUND;
            }

            return res;
        }

        /// <summary>
        /// 获取问卷信息，包含问题
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        public override async Task<RpcResult<QPaperFullRsp>> GetQPaperFullAsync(GetQPaperReq req)
        {
            var res = new RpcResult<QPaperFullRsp>();
            res.Data = new QPaperFullRsp();
           

            var qpaper = await this._qpaperRepo.GetQPaper(req.QpaperId);

            if (qpaper != null)
            {
                //判断是否有权限
                if (req.CheckRole && qpaper.CreateUserId != req.Identity) //非同一个用户
                {
                    res.Code = ErrorCodes.DATA_NOT_FOUND;
                    return res;
                }

                res.Data.Qpaper = new DTOQPaperWithQuestion();
                res.Data.Qpaper.QpaperId = qpaper.QpaperId;
                res.Data.Qpaper.Subject = qpaper.Subject;

                if (qpaper.StartTime.HasValue)
                {
                    res.Data.Qpaper.StartTime = qpaper.StartTime.Value.ToString("yyyy-MM-dd HH:mm:ss");
                }
                if (qpaper.EndTime.HasValue)
                {
                    res.Data.Qpaper.EndTime = qpaper.EndTime.Value.ToString("yyyy-MM-dd HH:mm:ss");
                }
                res.Data.Qpaper.Description = qpaper.Description;
                res.Data.Qpaper.ApaperCount = qpaper.ApaperCount;
                //查询列表数据
                var qlist = await this._qpaperRepo.GetQuestionsByPaperID(qpaper.QpaperId);

                if (qlist != null && qlist.Count > 0)
                {
                    addToDTO(res.Data.Qpaper.Questions, qlist);
                }
            }
            else
            {
                res.Code = ErrorCodes.DATA_NOT_FOUND;
            }

            return res;
        }

        /// <summary>
        /// 分页获取问卷列表
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public override async Task<RpcResult<QPaperListRsp>> QueryQPaperListAsync(QueryQPaperReq request)
        {
            var res = new RpcResult<QPaperListRsp>();
            res.Data = new QPaperListRsp();

            var userId = request.CheckRole ? request.Identity : String.Empty;
            var view = new PageView(request.Page, request.Rp);
        
            var plist = await this._qpaperRepo.QueryQPaperList(Utility.ClearSafeStringParma(request.Query), userId, view);
           
            res.Data.Total = view.PageIndex == 0 ? plist.Total : -1;

            if (plist != null && plist.DataList != null && plist.DataList.Count > 0)
            {
                foreach (var qpaper in plist.DataList)
                {
                    res.Data.List.Add(new DTOQPaper()
                    {
                        QpaperId = qpaper.QpaperId,
                        Subject = qpaper.Subject,
                        Description = qpaper.Description,
                        ApaperCount = qpaper.ApaperCount,
                        StartTime = qpaper.StartTime.HasValue ? qpaper.StartTime.Value.ToString("yyyy-MM-dd HH:mm:ss") : "",
                        EndTime = qpaper.EndTime.HasValue ? qpaper.EndTime.Value.ToString("yyyy-MM-dd HH:mm:ss") : ""
                    });
                }
            }
            return res;
        }

        /// <summary>
        /// 保存问卷信息
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        public override async Task<RpcResult<SaveQPaperRsp>> SaveQPaperAsync(SaveQPaperReq req)
        {
            var res = new RpcResult<SaveQPaperRsp>();
           
            if (req.Questions.Count == 0)
            {
                res.Code = ErrorCodes.PARAMS_VALIDATION_FAIL;
                return res;
            }
            res.Data = new SaveQPaperRsp();

            using (TransScope scope = this._qpaperRepo.BeginTransScope())
            {
                int paperId = 0;

                var qpaper = new QPaper();
                qpaper.QpaperId = req.QpaperId;
                qpaper.Description = req.Description;
                //数据校验
                if (!string.IsNullOrEmpty(req.StartTime))
                {
                    qpaper.StartTime = Convert.ToDateTime(req.StartTime);
                }

                if (!string.IsNullOrEmpty(req.EndTime))
                {
                    qpaper.EndTime = Convert.ToDateTime(req.EndTime);
                }
                qpaper.Subject = req.Subject;
                qpaper.UpdateTime = DateTime.Now;
                if (req.QpaperId > 0) // 更新
                {
                    paperId = req.QpaperId;

                    //更新时需判断是否存在答卷
                    bool hasAPaper = await this._qpaperRepo.CheckHasAPaper(paperId);
                    if (hasAPaper)
                    {
                        res.Code = ErrorCodes.INVALID_OPERATION;
                        res.Data.ReturnMessage = "该问卷已存在答卷,不能修改了！";
                        return res;
                    }

                    var t1 = this._qpaperRepo.UpdateAsync(qpaper);
                    //删除旧题目
                    var t2 = this._qpaperRepo.DeleteQuestionsByPId(paperId);

                    await Task.WhenAll(t1, t2); //两个任务可以并行
                }
                else
                {
                    qpaper.CreateUserId = req.Identity;
                    var newId = await this._qpaperRepo.InsertAsync(qpaper);
                    paperId = (int)newId;
                }

                if (paperId <= 0)
                {
                    //Internal error;
                    res.Code = ErrorCodes.INTERNAL_ERROR;
                    res.Data.ReturnMessage = "操作失败，请稍后重试";
                    return res;
                }
                res.Data.QpaperId = paperId;
                //重新保存问题
                int i = 0;
                var qlist = new List<Question>();
                foreach (var q in req.Questions)
                {
                    var question = new Question();
                    question.Id = q.Id;
                    question.PaperId = paperId;
                    question.Sequence = ++i;
                    question.ExtendInput = q.ExtendInput;
                    question.ItemDetail = q.ItemDetail;
                    question.QuestionType = (sbyte)q.QuestionType.GetHashCode();
                    question.Topic = q.Topic;
                    qlist.Add(question);
                }
                await this._qpaperRepo.AddQuestions(qlist);

                scope.Complete();// 提交事务
            }
            return res;
        }

        private void addToDTO(RepeatedField<DTOQuestion> questions, List<Question> qlist)
        {
            foreach (var q in qlist)
            {
                questions.Add(new DTOQuestion()
                {
                    Id = q.Id,
                    PaperId = q.PaperId,
                    Topic = q.Topic,
                    QuestionType = ConvertToEnumQuestionType(q.QuestionType),
                    ItemDetail = q.ItemDetail,
                    ExtendInput = q.ExtendInput
                });
            }
        }

        private QuestionType ConvertToEnumQuestionType(sbyte type)
        {
            switch (type)
            {
                case 0:
                    return QuestionType.Signle;

                case 1:
                    return QuestionType.Multiple;

                case 2:
                    return QuestionType.Subjective;

                default:
                    return QuestionType.Signle;
            }
        }
    }
}
