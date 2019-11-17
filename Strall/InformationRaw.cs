using System;

namespace Strall
{
    /// <summary>
    /// Representa uma informação pura, como é armazenada no banco de dados.
    /// </summary>
    public class InformationRaw: IInformationRaw
    {
        /// <summary>
        /// Identificador.
        /// </summary>
        public Guid Id { get; set; }
        
        /// <summary>
        /// Descrição.
        /// </summary>
        public string Description { get; set; } = string.Empty;
        
        /// <summary>
        /// Conteúdo.
        /// </summary>
        public string Content { get; set; } = string.Empty;
        
        /// <summary>
        /// Tipo de conteúdo
        /// </summary>
        public string ContentType { get; set; } = InformationType.Text.ToString();
        
        /// <summary>
        /// Informação de onde esta é filha.
        /// </summary>
        public Guid ParentId { get; set; }

        /// <summary>
        /// Relação de parentesco.
        /// </summary>
        public string ParentRelation { get; set; } = string.Empty;
        
        /// <summary>
        /// Informação de onde esta é um clone.
        /// </summary>
        public Guid CloneFromId { get; set; }

        /// <summary>
        /// Ordem de exibição entre informações irmãs.
        /// </summary>
        public int SiblingOrder { get; set; }

        /// <summary>
        /// Faz uma cópia desta instância para uma nova instância.
        /// </summary>
        /// <returns>Nova instância.</returns>
        public object Clone() => this.Copy();
    }
}
