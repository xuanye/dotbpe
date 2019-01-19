using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Google.Protobuf.Collections;
using Microsoft.Extensions.Options;
using PiggyMetrics.Common;

namespace PiggyMetrics.StatisticService.Repository
{
    public class StatisticRepository : BaseRepository
    {
        public StatisticRepository(IOptions<DbOption> Option) : base(Option.Value.StatDbConStr)
        {
        }

        internal async Task<int> SaveDataPointAsync(DataPoint dp)
        {
            string sql = "INSERT INTO `data_point` (`account`,`point_date`) VALUES (@Account,@PointDate); SELECT CAST(LAST_INSERT_ID() AS SIGNED);";

            long ret = await base.GetAsync<long>(sql, new { Account = dp.Account, PointDate = dp.Date });
            return (int)ret;
        }

        internal void SaveDataPointIncomesAsync(int dpId, RepeatedField<ItemMetric> incomes)
        {
            string sql = "INSERT INTO `data_point_income` (`point_id`,`title`,`amount`) VALUES (@PointId,@Title,@Amount)";
            for(var i =0; i < incomes.Count; i++)
            {
                base.Excute(sql, new { PointId = dpId, Title = incomes[i].Title, Amount = incomes[i].Amount });
            }
        }

        internal Task<List<ItemMetric>> FindAllExpenseByAccountAsync(string account)
        {
            string sql = @"SELECT `point_id` as PointId,`title` as Title,`amount`  as  Amount FROM `data_point_expense` a
                inner join data_point b on a.`point_id` = b.point_id
                where b.account=@Account";

            return base.QueryAsync<ItemMetric>(sql,new {Account=account});
        }

        internal Task<List<DataPointStat>> FindAllStatByAccountAsync(string account)
        {
            string sql = @"SELECT `point_id` as PointId,`stat_metric` as StatMetric,`amount`  as  Amount FROM `data_point_stat` a
                            inner join data_point b on a.`point_id` = b.point_id
                            where b.account=@Account";

            return base.QueryAsync<DataPointStat>(sql,new {Account=account});
        }

        internal Task<List<ItemMetric>> FindAllIncomeByAccountAsync(string account)
        {
            string sql = @"SELECT `point_id` as PointId,`title` as Title,`amount`  as  Amount FROM `data_point_income` a
                            inner join data_point b on a.`point_id` = b.point_id
                            where b.account=@Account";

            return base.QueryAsync<ItemMetric>(sql,new {Account=account});
        }

        internal Task<List<DataPoint>> FindAllByAccountAsync(string account)
        {
            string sql = "SELECT `point_id` as Id,`account` as Account,DATE_FORMAT(`point_date`,'%Y-%m-%d %H:%i:%s') as Date FROM `data_point` where `account`=@Account";

            return base.QueryAsync<DataPoint>(sql,new {Account=account});
        }

        internal void SaveDataPointExpensesAsync(int dpId, RepeatedField<ItemMetric> expenses)
        {
            string sql = "INSERT INTO `data_point_expense` (`point_id`,`title`,`amount`) VALUES (@PointId,@Title,@Amount)";
            for (var i = 0; i < expenses.Count; i++)
            {
                base.Excute(sql, new { PointId = dpId, Title = expenses[i].Title, Amount = expenses[i].Amount });
            }
        }

        internal void SaveDataPointRateAsync(int dpId, Dictionary<Currency, double> rates)
        {
            string sql = "INSERT INTO `data_point_stat` (`point_id`,`stat_metric`,`amount`) VALUES (@PointId ,@StatMetric,@Amount)";
            foreach (var kvpair in rates)
            {
                base.Excute(sql, new { PointId = dpId, StatMetric = kvpair.Key.GetHashCode(), Amount = kvpair.Value });
            }
        }


        internal void SaveDataPointStatAsync(int dpId, RepeatedField<DataPointStat> stat)
        {
            string sql = "INSERT INTO `data_point_stat` (`point_id`,`stat_metric`,`amount`) VALUES (@PointId,@StatMetric,@Amount)";
            for (var i = 0; i < stat.Count; i++)
            {
                base.Excute(sql, new { PointId = dpId, StatMetric = stat[i].StatMetric.GetHashCode(), Amount = stat[i].Amount });
            }
        }
    }
}
