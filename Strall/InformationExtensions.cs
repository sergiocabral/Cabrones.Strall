using System;

namespace Strall
{
    /// <summary>
    /// Classes de Extensions para funcionalidades relacionadas a IInformationRaw
    /// </summary>
    public static class InformationExtensions
    {
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
            destination.ParentId = source.ParentId;
            destination.ParentRelation = source.ParentRelation;
            destination.CloneFromId = source.CloneFromId;
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
    }
}