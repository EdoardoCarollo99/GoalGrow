namespace GoalGrow.Entity.Enums
{
    public enum NotificationType
    {
        Info = 1,           // Informativa generica
        Success = 2,        // Operazione completata con successo
        Warning = 3,        // Avviso (es: budget al 80%)
        Error = 4,          // Errore
        Alert = 5,          // Alert importante (es: budget superato)
        Goal = 6,           // Relativo a obiettivi
        Investment = 7,     // Relativo a investimenti
        Budget = 8,         // Relativo a budget
        Transaction = 9,    // Relativo a transazioni
        Consultant = 10,    // Messaggio dal consulente
        Achievement = 11,   // Achievement sbloccato
        System = 12         // Notifica di sistema
    }
}
