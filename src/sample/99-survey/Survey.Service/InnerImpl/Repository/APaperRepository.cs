using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Survey.Service.InnerImpl.Domain;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Vulcan.DataAccess;

namespace Survey.Service.InnerImpl.Repository
{
    public class APaperRepository : BaseRepository
    {
        //本示例使用同一个数据库
        public APaperRepository(IConnectionManagerFactory factory, IOptions<DBOption> Option,ILoggerFactory loggerFactory)
           : base(factory,Option.Value.Master, loggerFactory)
        {
        }
                

        internal Task<int> AddAnswers(List<Answer> list)
        {
            return base.BatchInsertAsync(list);
        }

        internal Task<PagedList<APaper>> QueryAPaperList(string subject, int? qPaperId, string userId, PageView view)
        {
            string where = "";
            if (qPaperId.HasValue && qPaperId.Value > 0)
            {
                where += " AND qpaper_id=" + qPaperId.Value;
            }
            if (!string.IsNullOrEmpty(subject))
            {
                where += " AND qpaper_subject LIKE '%" + subject + "%'";
            }
            if (!string.IsNullOrEmpty(userId))
            {
                where += " AND qpaper_user_id = '" + userId + "'";
            }

            return base.PagedQueryAsync<APaper>(view,
                "`paper_id`,`qpaper_id`,`qpaper_subject`,`qpaper_user_id`,`user_id`,`create_time`,`remark`",
                "apaper",
                where, null, "paper_id", " ORDER BY create_time DESC");
        }

        internal Task<APaper> GetAPaper(int id)
        {
            string sql = "SELECT `paper_id`,`qpaper_id`,`qpaper_subject`,`qpaper_user_id`,`user_id`,`create_time`,`remark` FROM apaper where paper_id = @PaperId";
            return base.GetAsync<APaper>(sql, new { PaperId = id });
        }

        internal Task<List<Answer>> GetAnswersByPaperId(int id)
        {
            string sql = "SELECT `answer_id` ,`apaper_id`,`question_id`,`objective_answer`,`subjective_answer` FROM answer WHERE apaper_id = @PaperId";
            return base.QueryAsync<Answer>(sql, new { PaperId = id });
        }

        internal async Task<bool> CheckAPaperAsync(int qpaperId, string userId)
        {
            string sql = "select count(1) FROM apaper where qpaper_id = @PaperId and user_id =@UserId";

            var hasC =await base.GetAsync<int>(sql, new { PaperId =qpaperId,UserId = userId });
            return hasC > 0;
        }

        internal Task<List<QPaperStaDetail>> QueryQPaperStaDetailAsync(int qpaperId)
        {
            string sql = @" SELECT a.question_id,
                        CONVERT(SUM(CASE WHEN a.objective_answer & 1 = 1 THEN 1 ELSE 0 END),signed) AS OA1,
                        CONVERT(SUM(CASE WHEN a.objective_answer & 2 = 2 THEN 1 ELSE 0 END),signed) AS OA2,
                        CONVERT(SUM(CASE WHEN a.objective_answer & 4 = 4 THEN 1 ELSE 0 END),signed) AS OA3,
                        CONVERT(SUM(CASE WHEN a.objective_answer & 8 = 8 THEN 1 ELSE 0 END),signed) AS OA4,
                        CONVERT(SUM(CASE WHEN a.objective_answer & 16 = 16 THEN 1 ELSE 0 END),signed) AS OA5,
                        CONVERT(SUM(CASE WHEN a.objective_answer & 32 = 32 THEN 1 ELSE 0 END),signed) AS OA6,
                        CONVERT(SUM(CASE WHEN a.objective_answer & 64 = 64 THEN 1 ELSE 0 END),signed) AS OA7,
                        CONVERT(SUM(CASE WHEN a.objective_answer & 128 = 128 THEN 1 ELSE 0 END),signed) AS OA8,
                        CONVERT(SUM(CASE WHEN a.objective_answer & 256 = 256 THEN 1 ELSE 0 END),signed) AS OA9,
                        CONVERT(SUM(CASE WHEN a.objective_answer & 512 = 512 THEN 1 ELSE 0 END),signed) AS OA10  
                        FROM answer a
                        inner join apaper b on a.apaper_id =b.paper_id
                        WHERE b.qpaper_id=@QPaperId and a.objective_answer>0
                        GROUP BY a.question_id";

            return base.QueryAsync<QPaperStaDetail>(sql, new { QPaperId = qpaperId });
        }
    }
}
