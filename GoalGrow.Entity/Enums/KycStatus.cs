namespace GoalGrow.Entity.Enums
{
    /// <summary>
    /// Stato del processo di verifica KYC
    /// </summary>
    public enum KycStatus
    {
        /// <summary>
        /// Utente non ha ancora iniziato il processo KYC
        /// </summary>
        NotStarted = 0,

        /// <summary>
        /// Utente ha iniziato ma non completato la submission
        /// </summary>
        Pending = 1,

        /// <summary>
        /// Documenti sottomessi, in attesa di revisione
        /// </summary>
        UnderReview = 2,

        /// <summary>
        /// KYC verificato con successo
        /// </summary>
        Verified = 3,

        /// <summary>
        /// KYC rifiutato (documenti non validi, non leggibili, ecc.)
        /// </summary>
        Rejected = 4,

        /// <summary>
        /// Verifica scaduta (richiede re-verifica annuale)
        /// </summary>
        Expired = 5,

        /// <summary>
        /// Account sospeso per attività sospette
        /// </summary>
        Suspended = 6
    }
}
