using System;
using System.Threading.Tasks;
using PiggyMetrics.Common;
using PiggyMetrics.StatisticService.Repository;
using PiggyMetrics.StatisticService.Interface;
using DotBPE.Rpc.Exceptions;
using DotBPE.Rpc.Logging;
using System.Collections.Generic;

namespace PiggyMetrics.StatisticService.Impl
{
    public class StatisticServiceImpl : StatisticServiceBase
    {
        static ILogger Logger = DotBPE.Rpc.Environment.Logger.ForType<StatisticServiceImpl>();
        private readonly StatisticRepository _repo;
        private readonly IExchangeRateService _rateService;

        public StatisticServiceImpl(StatisticRepository repo, IExchangeRateService rateService)
        {
            this._repo = repo;
            this._rateService = rateService;
        }
        public override async Task<StatRsp> FindByAccountAsync(FindAccountReq request)
        {
            StatRsp rsp = new StatRsp();
            try
            {
                List<DataPoint> dplist = await _repo.FindAllByAccountAsync(request.Current);
                if(dplist.Count >0)
                {
                    List<ItemMetric> dpIncomeList = await _repo.FindAllIncomeByAccountAsync(request.Current);
                    List<ItemMetric> dpExpenseList = await _repo.FindAllExpenseByAccountAsync(request.Current);
                    List<DataPointStat> dpStatList =  await _repo.FindAllStatByAccountAsync(request.Current);

                    foreach(DataPoint dp in dplist){
                        dp.Incomes.AddRange( dpIncomeList.FindAll(x=> x.DataPointId == dp.Id  )) ;
                        dp.Expenses.AddRange( dpExpenseList.FindAll(x=> x.DataPointId == dp.Id  )) ;
                        dp.Stat.AddRange( dpStatList.FindAll(x=> x.DataPointId == dp.Id  )) ;
                    }
                    rsp.DataPoint.AddRange(dplist);
                }

            }
            catch(Exception ex){
                rsp.Status = -1 ;
                rsp.Message = ex.Message;
                Logger.Error(ex,"Find Stat Erorr:"+ex.Message + ex.StackTrace) ;
            }



            return rsp;
        }

        public override async Task<VoidRsp> UpdateStatisticsAsync(AccountReq request)
        {
            var rsp = new VoidRsp();

            try
            {
                DataPoint dp = new DataPoint()
                {
                    Account = request.Name,
                    Date = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
                };
                double incomeAmout = 0;
                double expenseAmout = 0;

                foreach (var income in request.Incomes)
                {
                    var item = new ItemMetric()
                    {
                        Title = income.Title,
                        Amount = ConvertToRMB(income.Currency, income.Amount)
                    };
                    dp.Incomes.Add(item);
                    incomeAmout += item.Amount;
                }

                foreach (var expense in request.Expenses)
                {
                    var item = new ItemMetric()
                    {
                        Title = expense.Title,
                        Amount = ConvertToRMB(expense.Currency, expense.Amount)
                    };
                    dp.Expenses.Add(item);
                    expenseAmout += item.Amount;
                }

                var incomeDps = new DataPointStat()
                {
                    Amount = incomeAmout,
                    StatMetric = StatMetric.Saving
                };
                var expenseDps = new DataPointStat()
                {
                    Amount = expenseAmout,
                    StatMetric = StatMetric.Saving
                };
                var savingDps = new DataPointStat()
                {
                    Amount = ConvertToRMB(request.Saving.Currency, request.Saving.Amount),
                    StatMetric = StatMetric.Saving
                };
                dp.Stat.Add(incomeDps);
                dp.Stat.Add(expenseDps);
                dp.Stat.Add(savingDps);

                //操作数据库

                using(var trans = _repo.GetTransScope())
                {
                    int dpId = await _repo.SaveDataPointAsync(dp);
                    if (dpId <= 0)
                    {
                        throw new RpcBizException("save datapoint error");
                    }
                    _repo.SaveDataPointIncomesAsync(dpId, dp.Incomes);
                    _repo.SaveDataPointExpensesAsync(dpId, dp.Expenses);
                    _repo.SaveDataPointRateAsync(dpId, _rateService.GetRates());
                    _repo.SaveDataPointStatAsync(dpId, dp.Stat);
                    trans.Complete();
                }


            }
            catch (Exception ex)
            {
                rsp.Status = -1;
                rsp.Message = ex.Message;
                Logger.Error(ex,"Update Statistics Erorr:"+ex.Message + ex.StackTrace) ;
            }

            return rsp;
        }
        private double ConvertToRMB(Currency from, double value)
        {
            return _rateService.Convert(from, Currency.Usd, value);
        }
    }
}
