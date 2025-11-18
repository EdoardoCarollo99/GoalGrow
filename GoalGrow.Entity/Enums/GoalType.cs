namespace GoalGrow.Entity.Enums
{
    /// <summary>
    /// Tipo di obiettivo finanziario
    /// </summary>
    public enum GoalType
    {
        /// <summary>
        /// Obiettivo personalizzato creato dall'utente
        /// </summary>
        Custom = 0,

        /// <summary>
        /// Fondo di emergenza (default, non eliminabile)
        /// Consigliato: 3-6 mesi di spese
        /// </summary>
        Emergency = 1,

        /// <summary>
        /// Fondo investimenti (default, non eliminabile)
        /// Soglia minima per sbloccare marketplace consulenti (es. 5000€)
        /// </summary>
        Investment = 2,

        /// <summary>
        /// Obiettivo di risparmio generico
        /// </summary>
        Savings = 3,

        /// <summary>
        /// Obiettivo per acquisto importante (casa, auto, ecc.)
        /// </summary>
        Purchase = 4,

        /// <summary>
        /// Obiettivo per viaggio o vacanza
        /// </summary>
        Travel = 5,

        /// <summary>
        /// Obiettivo per educazione (università, corsi, ecc.)
        /// </summary>
        Education = 6,

        /// <summary>
        /// Obiettivo per pensione
        /// </summary>
        Retirement = 7,

        /// <summary>
        /// Altro tipo di obiettivo
        /// </summary>
        Other = 99
    }
}
