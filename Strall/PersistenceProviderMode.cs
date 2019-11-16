namespace Strall
{
    /// <summary>
    /// Modos possíveis para um IPersistenceProvider.
    /// </summary>
    public enum PersistenceProviderMode
    {
        /// <summary>
        /// Conexão fechada.
        /// </summary>
        Closed = 1 << 1,
        
        /// <summary>
        /// Conexão aberta.
        /// </summary>
        Opened = 1 << 2
    }
}