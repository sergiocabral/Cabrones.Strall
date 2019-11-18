namespace Strall
{
    /// <summary>
    /// Representa uma informação.
    /// </summary>
    public interface IInformation: IInformationRaw
    {
        /// <summary>
        /// Tipo de conteúdo
        /// </summary>
        new InformationType ContentType { get; set; }

        /// <summary>
        /// Informação de onde este conteúdo é um clone.
        /// </summary>
        IInformation? ContentFrom { get; set; }
        
        /// <summary>
        /// Informação de onde esta é filha.
        /// </summary>
        IInformation? Parent { get; set; }
    }
}