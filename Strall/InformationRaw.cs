using System;

namespace Strall
{
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
        public Guid CloneId { get; set; }

        /// <summary>
        /// Ordem de exibição entre informações irmãs.
        /// </summary>
        public int SiblingOrder { get; set; }
    }
}
