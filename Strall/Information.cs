using System;

namespace Strall
{
    /// <summary>
    /// Representa uma informação pura, como é armazenada no banco de dados.
    /// </summary>
    public class Information: IInformation
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
        /// Valor para ContentType
        /// </summary>
        private InformationType _contentType = InformationType.Text;

        /// <summary>
        /// Tipo de conteúdo
        /// </summary>
        public InformationType ContentType
        {
            get => _contentType;
            set => _contentType = value == InformationType.Numeric ? InformationType.Numeric : InformationType.Text;
        }

        /// <summary>
        /// Informação de onde este conteúdo é um clone.
        /// </summary>
        public Guid ContentFromId { get; set; }
        
        /// <summary>
        /// Informação de onde esta é filha.
        /// </summary>
        public Guid ParentId { get; set; }

        /// <summary>
        /// Relação de parentesco.
        /// </summary>
        public string ParentRelation { get; set; } = string.Empty;

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
