using System;
using System.Collections.Generic;

namespace Strall
{
    /// <summary>
    /// Representa uma informação.
    /// </summary>
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
        InformationType ContentType { get; set; }
        
        /// <summary>
        /// Informação de onde esta é filha.
        /// </summary>
        Guid ParentId { get; set; }
        
        /// <summary>
        /// Informação de onde esta é filha.
        /// </summary>
        IInformation? Parent { get; set; }

        /// <summary>
        /// Relação de parentesco.
        /// </summary>
        string ParentRelation { get; set; }
        
        /// <summary>
        /// Informação de onde esta é um clone.
        /// </summary>
        Guid CloneFromId { get; set; }
        
        /// <summary>
        /// Informação de onde esta é um clone.
        /// </summary>
        IInformation? CloneFrom { get; set; }
        
        /// <summary>
        /// Ordem de exibição entre informações irmãs.
        /// </summary>
        int SiblingOrder { get; set; }
        
        /// <summary>
        /// Lista de filhos imediatos.
        /// </summary>
        IEnumerable<Guid> ChildrenId { get; }
        
        /// <summary>
        /// Lista de filhos imediatos.
        /// </summary>
        IEnumerable<IInformation> Children { get; }
        
        /// <summary>
        /// Lista de clones.
        /// </summary>
        IEnumerable<Guid> ClonesToId { get; }
        
        /// <summary>
        /// Lista de clones.
        /// </summary>
        IEnumerable<IInformation> ClonesTo { get; }

        /// <summary>
        /// Informação pura, da forma como está gravada no banco de dados. 
        /// </summary>
        IInformationRaw? Raw { get; }
        
        /// <summary>
        /// Copia os dados de uma informação pura para esta instância.
        /// </summary>
        /// <param name="informationRaw">Informação pura.</param>
        /// <returns>Informação.</returns>
        IInformation Copy(IInformationRaw informationRaw);

        /// <summary>
        /// Descarta o cache de informações consultadas.
        /// Por exemplo, para as propriedades Parent e Clone.
        /// Ao consultar essas propriedades uma nova consulta será realizada.
        /// </summary>
        void DiscardCache();
        
        /// <summary>
        /// Verifica esta informação existe.
        /// Equivalente a SELECT TOP 1
        /// </summary>
        /// <returns>Resposta de existência.</returns>
        bool Exists();
        
        /// <summary>
        /// Obtem os dados desta informação.
        /// </summary>
        void Get();

        /// <summary>
        /// Cria esta informação.
        /// Equivalente a INSERT.
        /// </summary>
        /// <returns>Total de registro afetados.</returns>
        long Create();

        /// <summary>
        /// Atualiza esta informação.
        /// Equivalente a UPDATE.
        /// </summary>
        /// <returns>Total de registro afetados.</returns>
        long Update();

        /// <summary>
        /// Atualiza, ou cria se não existir, esta informação.
        /// </summary>
        /// <returns>Total de registro afetados.</returns>
        long UpdateOrCreate();

        /// <summary>
        /// Apaga esta informação.
        /// Equivalente a DELETE.
        /// </summary>
        /// <param name="recursively">Apagar recursivamente todos os filhos.</param>
        /// <returns>Total de registro afetados.</returns>
        long Delete(bool recursively = false);
    }
}