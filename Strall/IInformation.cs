using System;

namespace Strall
{
    /// <summary>
    /// Representa uma informação pura, como é armazenada no banco de dados.
    /// </summary>
    public interface IInformation: ICloneable
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
        InformationType ContentType { get; set; }
        
        /// <summary>
        /// Informação de onde este conteúdo é um clone.
        /// </summary>
        Guid ContentFromId { get; set; }
        
        /// <summary>
        /// Informação de onde esta é filha.
        /// </summary>
        Guid ParentId { get; set; }

        /// <summary>
        /// Relação de parentesco.
        /// </summary>
        string ParentRelation { get; set; }
        
        /// <summary>
        /// Ordem de exibição entre informações irmãs.
        /// </summary>
        int SiblingOrder { get; set; }
    }
}