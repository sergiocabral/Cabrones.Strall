using System;
using System.Collections.Generic;
using Strall.Exceptions;

namespace Strall
{
    /// <summary>
    /// Classes de Extensions para funcionalidades relacionadas a IInformationRaw
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
        public static IInformationRaw? SetDataAccess(this IInformationRaw? toReturn, IDataAccess? dataAccess)
        {
            _dataAccess = dataAccess;
            return toReturn;
        }

        /// <summary>
        /// IDataAccess padrão para manipulação dos dados.
        /// É necessário definir este valor para trabalhar com instâncias dessa classe.
        /// </summary>
        // ReSharper disable once UnusedParameter.Global
        public static IDataAccess GetDataAccess(this IInformationRaw? _) => 
            DataAccess;
        
        /// <summary>
        /// Copia os valores dos campos entre duas informações.
        /// </summary>
        /// <param name="source">Origem.</param>
        /// <param name="destination">Destino.</param>
        /// <returns>Retorna a mesma referência de destination.</returns>
        public static TInformacao Copy<TInformacao>(this IInformationRaw source, TInformacao destination) 
            where TInformacao : IInformationRaw
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
        public static IInformationRaw Copy(this IInformationRaw source)
        {
            var type = source.GetType();
            var instance = (IInformationRaw?)type.GetConstructor(new Type[0])?.Invoke(new object[0]);
            if (instance == null) throw new NotImplementedException();
            return source.Copy(instance);
        }

        /// <summary>
        /// Verifica se uma informação existe.
        /// Equivalente a SELECT TOP 1
        /// </summary>
        /// <param name="information">Informação.</param>
        /// <returns>Resposta de existência.</returns>
        public static bool Exists(this IInformationRaw information) =>
            DataAccess.Exists(information.Id);

        /// <summary>
        /// Obtem uma informação.
        /// Se não existir apagar o valor do Id.
        /// </summary>
        /// <param name="information">Informação.</param>
        /// <returns>Informação.</returns>
        public static IInformationRaw Get(this IInformationRaw information)
        {
            var returned = DataAccess.Get(information.Id);
            if (returned == null) information.Id = Guid.Empty;
            else returned.Copy(information);
            return information;
        }

        /// <summary>
        /// Cria uma informação.
        /// Equivalente a INSERT.
        /// </summary>
        /// <param name="information">Informação.</param>
        /// <returns>Id.</returns>
        public static IInformationRaw Create(this IInformationRaw information)
        {
            information.Id = DataAccess.Create(information);
            return information;
        }

        /// <summary>
        /// Atualiza uma informação.
        /// Equivalente a UPDATE.
        /// </summary>
        /// <param name="information">Informação.</param>
        /// <returns>Resposta de sucesso.</returns>
        public static bool Update(this IInformationRaw information) =>
            DataAccess.Update(information);

        /// <summary>
        /// Apaga uma informação.
        /// Equivalente a DELETE.
        /// Não é recursivo para seus filhos.
        /// </summary>
        /// <param name="information">Informação.</param>
        /// <returns>Resposta de sucesso.</returns>
        public static bool Delete(this IInformationRaw information) =>
            DataAccess.Delete(information.Id);

        /// <summary>
        /// Verifica se tem clones.
        /// Equivalente a SELECT TOP 1
        /// </summary>
        /// <param name="information">Informação.</param>
        /// <returns></returns>
        public static bool HasContentTo(this IInformationRaw information) =>
            DataAccess.HasContentTo(information.Id);
        
        /// <summary>
        /// Retorna a lista de clones.
        /// </summary>
        /// <param name="information">Informação.</param>
        /// <returns>Lista</returns>
        public static IEnumerable<Guid> ContentTo(this IInformationRaw information) =>
            DataAccess.ContentTo(information.Id);

        /// <summary>
        /// Localiza a origem de um clone
        /// </summary>
        /// <param name="information">Informação.</param>
        /// <returns>
        /// Id da origem. Em caso de loop retorna Guid.Empty.
        /// Caso não seja clone retorna o mesmo id.
        /// </returns>
        public static Guid ContentFrom(this IInformationRaw information) =>
            DataAccess.ContentFrom(information.Id);

        /// <summary>
        /// Verifica se tem filhos.
        /// Equivalente a SELECT TOP 1
        /// </summary>
        /// <param name="information">Informação.</param>
        /// <returns></returns>
        public static bool HasChildren(this IInformationRaw information) =>
            DataAccess.HasChildren(information.Id);
        
        /// <summary>
        /// Retorna a lista de filhos imediatos.
        /// Não é recursivo.
        /// </summary>
        /// <param name="information">Informação.</param>
        /// <returns>Lista</returns>
        public static IEnumerable<Guid> Children(this IInformationRaw information) =>
            DataAccess.Children(information.Id);
    }
}