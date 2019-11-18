using System;
using System.Collections.Generic;

namespace Strall
{
    /// <summary>
    /// Representa uma informação.
    /// </summary>
    public class Information : IInformation
    {
        /// <summary>
        /// IDataAccess padrão para manipulação dos dados.
        /// </summary>
        private IDataAccess DataAccess => this.GetDataAccess();

        /// <summary>
        /// Instância usada como molde para determinar os valores padrão.
        /// </summary>
        private static readonly InformationRaw InformationDefault = new InformationRaw();

        /// <summary>
        /// Identificador.
        /// </summary>
        public Guid Id { get; set; } = InformationDefault.Id;
        
        /// <summary>
        /// Descrição.
        /// </summary>
        public string Description { get; set; } = InformationDefault.Description;
        
        /// <summary>
        /// Conteúdo.
        /// </summary>
        public string Content { get; set; } = InformationDefault.Content;

        /// <summary>
        /// Tipo de conteúdo
        /// </summary>
        string IInformationRaw.ContentType
        {
            get => ContentType.ToString();
            set =>
                ContentType = Enum.TryParse<InformationType>(value, out var contentType)
                    ? contentType
                    : InformationType.Text;
        }

        /// <summary>
        /// Tipo de conteúdo
        /// </summary>
        public InformationType ContentType { get; set; } = Enum.Parse<InformationType>(InformationDefault.ContentType);

        /// <summary>
        /// Informação de onde este conteúdo é um clone.
        /// </summary>
        public Guid ContentFromId { get; set; } = InformationDefault.ContentFromId;

        /// <summary>
        /// Informação de onde este conteúdo é um clone.
        /// </summary>
        public IInformation? ContentFrom 
        {
            get => Get(DataAccess.ContentFrom(ContentFromId));
            set => ContentFromId = value?.Id ?? Guid.Empty;
        }
        
        /// <summary>
        /// Informação de onde esta é filha.
        /// </summary>
        public Guid ParentId { get; set; } = InformationDefault.ParentId;

        /// <summary>
        /// Informação de onde esta é filha.
        /// </summary>
        public IInformation? Parent
        {
            get => Get(ParentId);
            set => ParentId = value?.Id ?? Guid.Empty;
        }

        /// <summary>
        /// Relação de parentesco.
        /// </summary>
        public string ParentRelation { get; set; } = InformationDefault.ParentRelation;
        
        /// <summary>
        /// Ordem de exibição entre informações irmãs.
        /// </summary>
        public int SiblingOrder { get; set; } = InformationDefault.SiblingOrder;

        /// <summary>
        /// Faz uma cópia desta instância para uma nova instância.
        /// </summary>
        /// <returns>Nova instância.</returns>
        public object Clone() => this.Copy();
        
        /// <summary>
        /// Dicionário usado para cache de Get(Guid id)
        /// </summary>
        private readonly Dictionary<Guid, IInformation?> _cache = new Dictionary<Guid, IInformation?>();

        /// <summary>
        /// Retorna uma informação com base no id.
        /// Usa cache.
        /// </summary>
        /// <param name="informationId">Id</param>
        /// <param name="ignoreCache">Ignorar o cache e busca no banco de dados.</param>
        /// <returns>Informação</returns>
        private IInformation? Get(Guid informationId, bool ignoreCache = false)
        {
            if (!ignoreCache && _cache.ContainsKey(informationId)) return _cache[informationId];
            var information = DataAccess.Get(informationId)?.Copy(new Information());
            _cache[informationId] = information;
            return information;
        }
   }
}