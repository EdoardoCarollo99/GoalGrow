namespace GoalGrow.Entity.Enums
{
    public enum FundMovementStatus
    {
        Pending = 1,        // In attesa di elaborazione
        Processing = 2,     // In elaborazione
        Completed = 3,      // Completato con successo
        Failed = 4,         // Fallito
        Rejected = 5,       // Rifiutato
        Cancelled = 6       // Annullato dall'utente
    }
}
