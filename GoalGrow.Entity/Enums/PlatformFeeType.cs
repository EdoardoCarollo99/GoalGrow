namespace GoalGrow.Entity.Enums
{
    /// <summary>
    /// Tipo di fee applicata dalla piattaforma GoalGrow
    /// </summary>
    public enum PlatformFeeType
    {
        /// <summary>
        /// Fee su deposito nel virtual wallet
        /// </summary>
        Deposit = 1,

        /// <summary>
        /// Fee su prelievo dal virtual wallet
        /// </summary>
        Withdrawal = 2,

        /// <summary>
        /// Fee su transazione di investimento
        /// </summary>
        Investment = 3,

        /// <summary>
        /// Fee su vendita/liquidazione investimento
        /// </summary>
        InvestmentSale = 4,

        /// <summary>
        /// Fee su profitti da investimento
        /// </summary>
        InvestmentProfit = 5,

        /// <summary>
        /// Fee mensile di gestione account (se prevista)
        /// </summary>
        MonthlyMaintenance = 6,

        /// <summary>
        /// Fee su trasferimento fondi tra obiettivi
        /// </summary>
        GoalTransfer = 7,

        /// <summary>
        /// Altra tipologia di fee
        /// </summary>
        Other = 99
    }

    /// <summary>
    /// Stato della fee
    /// </summary>
    public enum FeeStatus
    {
        /// <summary>
        /// Fee calcolata ma non ancora riscossa
        /// </summary>
        Pending = 0,

        /// <summary>
        /// Fee riscossa con successo
        /// </summary>
        Collected = 1,

        /// <summary>
        /// Fee annullata/rimborsata
        /// </summary>
        Refunded = 2,

        /// <summary>
        /// Fee in errore (problemi di riscossione)
        /// </summary>
        Failed = 3
    }
}
