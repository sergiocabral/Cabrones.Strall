using System;

namespace Strall
{
    public interface IInformation
    {
        /// <summary>
        /// Identificador.
        /// </summary>
        Guid Id { get; set; }
        
        /// <summary>
        /// Descrição.
        /// </summary>
        string? Description { get; set; }
        
        /// <summary>
        /// Conteúdo.
        /// </summary>
        object? Content { get; set; }
        
        /// <summary>
        /// Tipo de conteúdo
        /// </summary>
        InformationType Type { get; set; }
        
        /// <summary>
        /// Informação de onde esta é filha.
        /// </summary>
        IInformation? Parent { get; set; }
        
        /// <summary>
        /// Informação de onde esta é um clone.
        /// </summary>
        IInformation? Clone { get; set; }
        
        /// <summary>
        /// Ordem de exibição entre informações irmãs.
        /// </summary>
        int Order { get; set; }
    }
}