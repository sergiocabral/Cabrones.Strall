using System;
using System.Collections.Generic;
using Strall.Exceptions;

namespace Strall
{
    /// <summary>
    /// Classes de Extensions para funcionalidades relacionadas a IInformation
    /// </summary>
    public static class InformationExtensions
    {
        /// <summary>
        /// Atributo para GetDataAccess e SetDataAccess.
        /// </summary>
        private static IDataAccess? _dataAccess;

        /// <summary>
        /// IDataAccess padrão para manipulação dos dados.
        /// É necessário definir este valor para trabalhar com instâncias dessa classe.
        /// </summary>
        // ReSharper disable once UnusedParameter.Global
        private static IDataAccess DataAccess =>
            _dataAccess ?? throw new StrallDataAccessIsNullException();

        /// <summary>
        /// Define um IDataAccess padrão para manipulação dos dados.
        /// É necessário definir este valor para trabalhar com instâncias dessa classe.
        /// </summary>
        /// <returns>O retorno é o mesmo que toReturn.</returns>
        public static IInformation? SetDataAccess(this IInformation? toReturn, IDataAccess? dataAccess)
        {
            _dataAccess = dataAccess;
            return toReturn;
        }

        /// <summary>
        /// IDataAccess padrão para manipulação dos dados.
        /// É necessário definir este valor para trabalhar com instâncias dessa classe.
        /// </summary>
        // ReSharper disable once UnusedParameter.Global
        public static IDataAccess GetDataAccess(this IInformation? _) => 
            DataAccess;
        
        /// <summary>
        /// Copia os valores dos campos entre duas informações.
        /// </summary>
        /// <param name="source">Origem.</param>
        /// <param name="destination">Destino.</param>
        /// <returns>Retorna a mesma referência de destination.</returns>
        public static TInformacao CopyTo<TInformacao>(this IInformation source, TInformacao destination) 
            where TInformacao : IInformation
        {
            destination.Id = source.Id;
            destination.Description = source.Description;
            destination.Content = source.Content;
            destination.ContentType = source.ContentType;
            destination.ContentFromId = source.ContentFromId;
            destination.ParentId = source.ParentId;
            destination.ParentRelation = source.ParentRelation;
            destination.SiblingOrder = source.SiblingOrder;
            return destination;
        }
        
        /// <summary>
        /// Cria uma nova instância e copia os campos.
        /// </summary>
        /// <param name="source">Origem.</param>
        /// <returns>Retorna a referência para a nova instância.</returns>
        public static IInformation CopyTo(this IInformation source)
        {
            var type = source.GetType();
            var instance = (IInformation)type.GetConstructor(new Type[0])?.Invoke(new object[0])!;
            return source.CopyTo(instance);
        }

        /// <summary>
        /// Verifica se uma informação existe.
        /// Equivalente a SELECT TOP 1
        /// </summary>
        /// <param name="information">Informação.</param>
        /// <returns>Resposta de existência.</returns>
        public static bool Exists(this IInformation information) =>
            DataAccess.Exists(information.Id);

        /// <summary>
        /// Obtem uma informação.
        /// </summary>
        /// <param name="informationId">Informação.</param>
        /// <param name="contentSynchronize">Sincroniza o conteúdo com base nas réplicas.</param>
        /// <returns>Informação.</returns>
        public static IInformation? GetInformation(this Guid informationId, bool contentSynchronize = true) =>
            contentSynchronize ? DataAccess.Get(informationId)?.ContentLoad(false) : DataAccess.Get(informationId);

        /// <summary>
        /// Obtem uma informação.
        /// Se não existir apagar o valor do Id.
        /// </summary>
        /// <param name="information">Informação.</param>
        /// <param name="contentSynchronize">Sincroniza o conteúdo com base nas réplicas.</param>
        /// <returns>Informação.</returns>
        public static IInformation Get(this IInformation information, bool contentSynchronize = true)
        {
            var returned = information.Id.GetInformation(contentSynchronize);
            if (returned == null) information.Id = Guid.Empty;
            else returned.CopyTo(information);
            return information;
        }

        /// <summary>
        /// Cria uma informação.
        /// Equivalente a INSERT.
        /// </summary>
        /// <param name="information">Informação.</param>
        /// <param name="contentSynchronize">Sincroniza o conteúdo com base nas réplicas.</param>
        /// <returns>Id.</returns>
        public static IInformation Create(this IInformation information, bool contentSynchronize = true)
        {
            information.Id = DataAccess.Create(information);
            if (contentSynchronize) information.ContentSave(false);
            return information;
        }

        /// <summary>
        /// Atualiza uma informação.
        /// Equivalente a UPDATE.
        /// </summary>
        /// <param name="information">Informação.</param>
        /// <param name="contentSynchronize">Sincroniza o conteúdo com base nas réplicas.</param>
        /// <returns>Resposta de sucesso.</returns>
        public static bool Update(this IInformation information, bool contentSynchronize = true)
        {
            var updated = DataAccess.Update(information);
            if (updated && contentSynchronize) information.ContentSave(false);
            return updated;
        }

        /// <summary>
        /// Atualiza, ou cria se não existir, esta informação.
        /// </summary>
        /// <param name="information">Informação.</param>
        /// <param name="contentSynchronize">Sincroniza o conteúdo com base nas réplicas.</param>
        /// <returns>Total de registro afetados.</returns>
        public static IInformation UpdateOrCreate(this IInformation information, bool contentSynchronize = true)
        {
            if (!information.Exists()) return information.Create(contentSynchronize);
            information.Update(contentSynchronize);
            return information;
        }

        /// <summary>
        /// Apaga uma informação.
        /// Equivalente a DELETE.
        /// </summary>
        /// <param name="information">Informação.</param>
        /// <param name="recursively">Apagar recursivamente todos os filhos.</param>
        /// <returns>Total de registros apagados.</returns>
        public static int Delete(this IInformation information, bool recursively = false)
        {
            if (recursively) return DataAccess.DeleteAll(information.Id);
            return DataAccess.Delete(information.Id) ? 1 : 0;
        }

        /// <summary>
        /// Verifica se tem clones.
        /// Equivalente a SELECT TOP 1
        /// </summary>
        /// <param name="information">Informação.</param>
        /// <returns></returns>
        public static bool HasContentTo(this IInformation information) =>
            DataAccess.HasContentTo(information.Id);
        
        /// <summary>
        /// Retorna a lista de clones.
        /// </summary>
        /// <param name="information">Informação.</param>
        /// <returns>Lista</returns>
        public static IEnumerable<Guid> ContentTo(this IInformation information) =>
            DataAccess.ContentTo(information.Id);

        /// <summary>
        /// Localiza a origem de um clone
        /// </summary>
        /// <param name="information">Informação.</param>
        /// <returns>
        /// Id da origem. Em caso de loop retorna Guid.Empty.
        /// Caso não seja clone retorna o mesmo id.
        /// </returns>
        public static Guid ContentFrom(this IInformation information) =>
            DataAccess.ContentFrom(information.Id);

        /// <summary>
        /// Localiza o conteúdo no clone de origem
        /// e atualiza o valor do conteúdo atual.
        /// </summary>
        /// <param name="information">Informação.</param>
        /// <param name="queryPersistence">Faz consulta no banco de dados para confirmar as informações.</param>
        /// <returns>
        /// Mesma referência de information.
        /// </returns>
        public static IInformation ContentLoad(this IInformation information, bool queryPersistence = true)
        {
            var contentFromId = queryPersistence ? information.ContentFrom() : information.ContentFromId;
            information.Content = 
                contentFromId == Guid.Empty
                    ? information.Content
                    : contentFromId.GetInformation(false)?.Content
                      ?? information.Content;
            return information;
        }

        /// <summary>
        /// Localiza o conteúdo no clone de origem
        /// e atualiza o valor na origem.
        /// </summary>
        /// <param name="information">Informação.</param>
        /// <param name="queryPersistence">Faz consulta no banco de dados para confirmar as informações.</param>
        /// <returns>
        /// Mesma referência de information.
        /// </returns>
        public static IInformation ContentSave(this IInformation information, bool queryPersistence = true)
        {
            if (!queryPersistence && information.ContentFromId == Guid.Empty) return information;

            var informationSource = queryPersistence
                ? information.ContentFrom().GetInformation(false)
                : information.ContentFromId.GetInformation(false);
            
            if (informationSource == null) return information;
            
            informationSource.Content = information.Content;
            informationSource.Update(false);
            
            return information;
        }

        /// <summary>
        /// Verifica se tem filhos.
        /// Equivalente a SELECT TOP 1
        /// </summary>
        /// <param name="information">Informação.</param>
        /// <returns></returns>
        public static bool HasChildren(this IInformation information) =>
            DataAccess.HasChildren(information.Id);
        
        /// <summary>
        /// Retorna a lista de filhos imediatos.
        /// Não é recursivo.
        /// </summary>
        /// <param name="information">Informação.</param>
        /// <returns>Lista</returns>
        public static IEnumerable<Guid> Children(this IInformation information) =>
            DataAccess.Children(information.Id);
    }
}