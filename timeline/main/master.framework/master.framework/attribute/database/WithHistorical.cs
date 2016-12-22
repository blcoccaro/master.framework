namespace master.framework.attribute.database
{
    public class WithHistorical : System.Attribute
    {
        public master.framework.Enumerators.HistoricalType HistoricalType { get; set; }
        public WithHistorical(master.framework.Enumerators.HistoricalType historicalType = Enumerators.HistoricalType.ByProperty)
        {
            HistoricalType = historicalType;
        }
    }
}
