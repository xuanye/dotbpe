namespace PiggyMetrics.Common
{
    public partial class AccountReq
    {
        public string Name{
            get{
                return this.Current;
            }
            set{
                this.Current = value;
            }
        }
    }
}
