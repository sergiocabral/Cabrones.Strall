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
        string Description { get; set; }
        
        /// <summary>
        /// Conteúdo.
        /// </summary>
        string Content { get; set; }
        
        /// <summary>
        /// Tipo de conteúdo
        /// </summary>
        string ContentType { get; set; }
        
        /// <summary>
        /// Informação de onde esta é filha.
        /// </summary>
        Guid ParentId { get; set; }

        /// <summary>
        /// Relação de parentesco.
        /// </summary>
        string ParentRelation { get; set; }
        
        /// <summary>
        /// Informação de onde esta é um clone.
        /// </summary>
        Guid CloneId { get; set; }
        
        /// <summary>
        /// Ordem de exibição entre informações irmãs.
        /// </summary>
        int SiblingOrder { get; set; }
    }
}