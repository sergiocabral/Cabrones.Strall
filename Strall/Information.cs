using System;

namespace Strall
{
    public class Information: IInformation
    {
        /// <summary>
        /// Identificador.
        /// </summary>
        public Guid Id { get; set; } = Guid.Empty;
        
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
        public string ContentType { get; set; } = string.Empty;
        
        /// <summary>
        /// Informação de onde esta é filha.
        /// </summary>
        public Guid ParentId { get; set; } = Guid.Empty;
        
        /// <summary>
        /// Informação de onde esta é um clone.
        /// </summary>
        public Guid CloneId { get; set; } = Guid.Empty;

        /// <summary>
        /// Ordem de exibição entre informações irmãs.
        /// </summary>
        public int SiblingOrder { get; set; } = 0;
    }
}
