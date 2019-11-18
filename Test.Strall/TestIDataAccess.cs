using Cabrones.Test;
using Xunit;

namespace Strall
{
    public class TestIDataAccess
    {
        [Fact]
        public void verificações_declarativas()
        {
            // Arrange, Given
            // Act, When

            var sut = typeof(IDataAccess);

            // Assert, Then

            sut.AssertMyImplementations();
            sut.AssertMyOwnImplementations();
            sut.AssertMyOwnPublicPropertiesCount(0);
            sut.AssertMyOwnPublicMethodsCount(10);
            sut.AssertPublicMethodPresence("Boolean Exists(Guid)");
            sut.AssertPublicMethodPresence("IInformationRaw Get(Guid)");
            sut.AssertPublicMethodPresence("Guid Create(IInformationRaw)");
            sut.AssertPublicMethodPresence("Boolean Update(IInformationRaw)");
            sut.AssertPublicMethodPresence("Boolean Delete(Guid)");
            sut.AssertPublicMethodPresence("Boolean HasContentTo(Guid)");
            sut.AssertPublicMethodPresence("IEnumerable<Guid> ContentTo(Guid)");
            sut.AssertPublicMethodPresence("Guid ContentFrom(Guid)");
            sut.AssertPublicMethodPresence("Boolean HasChildren(Guid)");
            sut.AssertPublicMethodPresence("IEnumerable<Guid> Children(Guid)");
        }
    }
}