using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Survey.Service.InnerImpl.Domain;
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
                "`paper_id`,`qpaper_id`,`qpaper_subject`,`user_id`,`create_time`,`remark`",
                "apaper",
                where, null, "paper_id", " ORDER BY create_time DESC");
        }

        internal Task<APaper> GetAPaper(int id)
        {
            string sql = "SELECT `paper_id`,`qpaper_id`,`qpaper_subject`,`user_id`,`create_time`,`remark` FROM apaper where PaperId = @PaperId";
            return base.GetAsync<APaper>(sql, new { PaperId = id });
        }

        internal Task<List<Answer>> GetAnswersByPaperId(int id)
        {
            string sql = "SELECT `answer_id` ,`apaper_id`,`question_id`,`objective_answer`,`subjective_answer` FROM answer WHERE apaper_id = @PaperId";
            return base.QueryAsync<Answer>(sql, new { PaperId = id });
        }
    }
}
