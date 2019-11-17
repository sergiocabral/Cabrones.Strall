namespace Strall
{
    /// <summary>
    /// Tipos possíveis do conteudo da informação.
    /// </summary>
    public enum InformationType
    {
        /// <summary>
        /// Texto.
        /// </summary>
        Text = 1 << 1,
        
        /// <summary>
        /// Numérico.
        /// </summary>
        Numeric = 1 << 2
    }
}