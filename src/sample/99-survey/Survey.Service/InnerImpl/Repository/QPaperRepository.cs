using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Survey.Service.InnerImpl.Domain;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Vulcan.DataAccess;

namespace Survey.Service.InnerImpl.Repository
{
    public class QPaperRepository : BaseRepository
    {
        //本示例默认使用同一个数据库，实际情况下，可以分多个库，比如用户是一个库
        public QPaperRepository(IConnectionManagerFactory factory, IOptions<DBOption> Option, ILoggerFactory loggerFactory)
           : base(factory,Option.Value.Master, loggerFactory)
        {
        }

        public Task<int> DeleteQuestionsByPId(int paperId)
        {
            string sql = "DELETE FROM question WHERE paper_id = @PaperId";
            return base.ExcuteAsync(sql, new { PaperId = paperId });
        }

        public Task<int> AddQuestions(List<Question> list)
        {
            return base.BatchInsertAsync(list);
        }

        internal Task<PagedList<QPaper>> QueryQPaperList(string subject, string userId, PageView view)
        {
            string where = "";
            if (!string.IsNullOrEmpty(subject))
            {
                where += " AND `subject` LIKE '%" + subject + "%'";
            }
            if (!string.IsNullOrEmpty(userId))
            {
                where += " AND create_user_id = '" + userId + "'";
            }
            return base.PagedQueryAsync<QPaper>(view,
                "`qpaper_id`,`subject`,`start_time`,`end_time`,`description`,`apaper_count`,`create_user_id`,`update_time`",
                "qpaper",
                where, null, "qpaper_id", " ORDER BY update_time DESC");
        }

        internal Task<int> UpdateAPaperCountAsync(int qpaperId, int count)
        {
            string sql = "update qpaper set  `apaper_count`=`apaper_count`+@Count where qpaper_id = @PaperId";

            return base.ExcuteAsync(sql, new { PaperId = qpaperId, Count = count });
        }

        internal Task<List<Question>> GetQuestionsByPaperID(int paperId)
        {
            string sql = "SELECT `id`,`topic`,`paper_id`,`question_type`,`item_detail`,`sequence`,`extend_input` " +
                "FROM `question` where paper_id = @PaperId ORDER BY sequence ASC";

            return base.QueryAsync<Question>(sql, new { PaperId = paperId });
        }

        internal Task<QPaper> GetQPaper(int paperId)
        {
            string sql = @"SELECT `qpaper_id`,`subject`,`start_time`,`end_time`,`description`
                        ,`apaper_count`,`create_user_id`
                        ,`update_time` FROM qpaper where qpaper_id = @PaperId";
            return base.GetAsync<QPaper>(sql, new { PaperId = paperId });
        }

        internal async Task<bool> CheckHasAPaper(int paperId)
        {
            var count = await base.GetAsync<int>("SELECT `apaper_count` FROM qpaper where qpaper_id = @PaperId ", new { PaperId = paperId });
            return count > 0;
        }
    }
}
