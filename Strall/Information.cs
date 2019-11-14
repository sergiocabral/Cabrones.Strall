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
        public string? Description { get; set; } = null;

        /// <summary>
        /// Conteúdo.
        /// </summary>
        public object? Content { get; set; } = null;

        /// <summary>
        /// Tipo de conteúdo
        /// </summary>
        public InformationType Type { get; set; } = InformationType.Text;

        /// <summary>
        /// Informação de onde esta é filha.
        /// </summary>
        public IInformation? Parent { get; set; } = null;

        /// <summary>
        /// Informação de onde esta é um clone.
        /// </summary>
        public IInformation? Clone { get; set; } = null;

        /// <summary>
        /// Ordem de exibição entre informações irmãs.
        /// </summary>
        public int Order { get; set; } = 0;
    }
}
