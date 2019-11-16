using System;
using System.Collections.Generic;
using System.Linq;
using Strall.Exceptions;

namespace Strall
{
    /// <summary>
    /// Representa uma informação.
    /// </summary>
    public class Information : IInformation, IInformationRaw
    {
        /// <summary>
        /// Instância usada como molde para determinar os valores padrão.
        /// </summary>
        private static readonly InformationRaw InformationDefault = new InformationRaw();

        /// <summary>
        /// Construtor.
        /// </summary>
        public Information() =>
            Copy(InformationDefault, this);
        
        /// <summary>
        /// Identificador.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Descrição.
        /// </summary>
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// Valor de cache para: Content
        /// </summary>
        private string _content = string.Empty;
        
        /// <summary>
        /// Conteúdo.
        /// </summary>
        public string Content
        {
            get
            {
                if (CloneFromId != Guid.Empty && CloneFrom != null) _content = CloneFrom.Content;
                return _content;
            }
            set => _content = value;
        }

        /// <summary>
        /// Tipo de conteúdo
        /// </summary>
        string IInformationRaw.ContentType
        {
            get => ContentType.ToString();
            set
            {
                if (!Enum.TryParse<InformationType>(value, out var contentType))
                    contentType = Enum.Parse<InformationType>(InformationDefault.ContentType);
                ContentType = contentType;
            }
        }

        /// <summary>
        /// Valor de cache para: ContentType
        /// </summary>
        private InformationType _contentType = InformationType.Text;
        
        /// <summary>
        /// Tipo de conteúdo.
        /// </summary>
        public InformationType ContentType
        {
            get
            {
                if (CloneFromId != Guid.Empty && CloneFrom != null) _contentType = CloneFrom.ContentType;
                return _contentType;
            }
            set => _contentType = value;
        }
        
        /// <summary>
        /// Informação de onde esta é filha.
        /// </summary>
        public Guid ParentId { get; set; }

        /// <summary>
        /// Indicador de cache para: Parent
        /// </summary>
        private Guid _cacheForParentId;
        
        /// <summary>
        /// Cache para: Parent
        /// </summary>
        private IInformation? _cacheForParent;
        
        /// <summary>
        /// Informação de onde esta é filha.
        /// </summary>
        public IInformation? Parent
        {
            get
            {
                if (ParentId == Guid.Empty) return null;
                if (ParentId == _cacheForParentId) return _cacheForParent;
                _cacheForParentId = ParentId;
                _cacheForParent = Get(ParentId);
                return _cacheForParent;
            }
            set
            {
                if (value == null) ParentId = Guid.Empty;
                else if (value.Id != ParentId) _cacheForParentId = ParentId = value.Id;
                _cacheForParent = value;
            }
        }

        /// <summary>
        /// Relação de parentesco.
        /// </summary>
        public string ParentRelation { get; set; } = string.Empty;
        
        /// <summary>
        /// Informação de onde esta é um clone.
        /// </summary>
        public Guid CloneFromId { get; set; }

        /// <summary>
        /// Indicador de cache para: Clone
        /// </summary>
        private Guid _cacheForCloneFromId;
        
        /// <summary>
        /// Cache para: Clone
        /// </summary>
        private IInformation? _cacheForCloneFrom;

        /// <summary>
        /// Informação de onde esta é um clone.
        /// </summary>
        public IInformation? CloneFrom
        {
            get
            {
                if (CloneFromId == Guid.Empty) return null;
                if (CloneFromId == _cacheForCloneFromId) return _cacheForCloneFrom;
                _cacheForCloneFromId = CloneFromId;
                _cacheForCloneFrom = Get(CloneFromId);
                return _cacheForCloneFrom;
            }
            set
            {
                if (value == null) CloneFromId = Guid.Empty;
                else if (value.Id != CloneFromId) _cacheForCloneFromId = CloneFromId = value.Id;
                _cacheForCloneFrom = value;
            }
        }

        /// <summary>
        /// Ordem de exibição entre informações irmãs.
        /// </summary>
        public int SiblingOrder { get; set; }

        /// <summary>
        /// Lista de filhos imediatos.
        /// </summary>
        public IEnumerable<Guid> ChildrenId => DataAccessDefault != null ? DataAccessDefault.Children(Id) : throw new NullReferenceException();

        /// <summary>
        /// Lista de filhos imediatos.
        /// </summary>
        public IEnumerable<IInformation> Children => ChildrenId.Select(a => Get(a) ?? throw new NullReferenceException());

        /// <summary>
        /// Lista de clones.
        /// </summary>
        public IEnumerable<Guid> ClonesToId => DataAccessDefault != null ? DataAccessDefault.ClonesTo(Id) : throw new NullReferenceException();

        /// <summary>
        /// Lista de clones.
        /// </summary>
        public IEnumerable<IInformation> ClonesTo => ClonesToId.Select(a => Get(a) ?? throw new NullReferenceException());
        
        /// <summary>
        /// IDataAccess padrão para manipulação dos dados.
        /// É necessário definir este valor para trabalhar com instâncias dessa classe.
        /// </summary>
        public static IDataAccess? DataAccessDefault { get; set; }

        /// <summary>
        /// Obtem uma informação.
        /// </summary>
        /// <param name="informationId"></param>
        /// <returns>Informação.</returns>
        public static IInformation? Get(Guid informationId)
        {
            if (DataAccessDefault == null) throw new StrallDataAccessException();
            var informationRaw = DataAccessDefault.Get(informationId);
            return informationRaw == null ? null : new Information().Copy(informationRaw);
        }

        /// <summary>
        /// Copiar as propriedades de uma informação pura para este tipo de classe.
        /// </summary>
        /// <param name="informationRaw">Informação pura.</param>
        /// <param name="information">Informação deste tipo de classe.</param>
        private static void Copy(IInformationRaw informationRaw, IInformation information)
        {
            information.Id = informationRaw.Id;
            information.Description = informationRaw.Description;
            information.Content = informationRaw.Content;

            information.ContentType = 
                Enum.TryParse<InformationType>(informationRaw.ContentType, out var contentType) ? 
                    contentType :
                    InformationType.Text;
            
            information.ParentId = informationRaw.ParentId;
            information.ParentRelation = informationRaw.ParentRelation;
            information.CloneFromId = informationRaw.CloneFromId;
            information.SiblingOrder = informationRaw.SiblingOrder;
        }
        
        /// <summary>
        /// Carrega os dados de uma informação pura para esta instância.
        /// </summary>
        /// <param name="informationRaw">Informação pura.</param>
        /// <returns>Informação.</returns>
        public IInformation Copy(IInformationRaw informationRaw)
        {
            Copy(Raw = informationRaw, this);
            return this;
        }
        
        /// <summary>
        /// Informação pura, da forma como está gravada no banco de dados. 
        /// </summary>
        public IInformationRaw? Raw { get; private set; }

        /// <summary>
        /// Descarta o cache de informações consultadas.
        /// Por exemplo, para as propriedades Parent e Clone.
        /// Ao consultar essas propriedades uma nova consulta será realizada.
        /// </summary>
        public void DiscardCache()
        {
            _cacheForParent = null;
            _cacheForParentId = Guid.Empty;
            _cacheForCloneFrom = null;
            _cacheForCloneFromId = Guid.Empty;
        }

        /// <summary>
        /// Verifica esta informação existe.
        /// Equivalente a SELECT TOP 1
        /// </summary>
        /// <returns>Resposta de existência.</returns>
        public bool Exists()
        {
            if (DataAccessDefault == null) throw new StrallDataAccessException();
            return DataAccessDefault.Exists(Id);
        }

        /// <summary>
        /// Obtem os dados desta informação.
        /// </summary>
        public void Get()
        {
            if (DataAccessDefault == null) throw new StrallDataAccessException();
            var id = Id;
            Copy(DataAccessDefault.Get(id) ?? InformationDefault, this);
            Id = id;
        }

        /// <summary>
        /// Cria esta informação.
        /// Equivalente a INSERT.
        /// </summary>
        /// <returns>Total de registro afetados.</returns>
        public long Create()
        {
            if (DataAccessDefault == null) throw new StrallDataAccessException();
            Id = DataAccessDefault.Create(this);
            return 1;
        }

        /// <summary>
        /// Atualiza esta informação.
        /// Equivalente a UPDATE.
        /// </summary>
        /// <returns>Total de registro afetados.</returns>
        public long Update()
        {
            if (DataAccessDefault == null) throw new StrallDataAccessException();
            DataAccessDefault.Update(this);
            return 1;
        }

        /// <summary>
        /// Atualiza, ou cria se não existir, esta informação.
        /// </summary>
        /// <returns>Total de registro afetados.</returns>
        public long UpdateOrCreate()
        {
            if (DataAccessDefault == null) throw new StrallDataAccessException();
            return DataAccessDefault.Exists(Id) ? Update() : Create();
        }

        /// <summary>
        /// Apaga esta informação.
        /// Equivalente a DELETE.
        /// </summary>
        /// <param name="recursively">Apagar recursivamente todos os filhos.</param>
        /// <returns>Total de registro afetados.</returns>
        public long Delete(bool recursively = false)
        {
            if (DataAccessDefault == null) throw new StrallDataAccessException();
            DataAccessDefault.Delete(Id);
            return 1;
        }
    }
}