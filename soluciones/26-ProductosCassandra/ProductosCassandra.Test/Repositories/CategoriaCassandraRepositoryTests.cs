using FluentAssertions;
using NUnit.Framework;

namespace ProductosCassandra.Test.Repositories;

/// <summary>
/// Tests del repositorio de categorias contra un Cassandra real (TestContainers).
/// </summary>
[TestFixture]
public class CategoriaCassandraRepositoryTests : IntegrationTestBase
{
    [Test]
    public async Task GetCategorias_DevuelveElCatalogo()
    {
        // Act
        var categorias = await CategoriaRepositorio.GetCategoriasAsync();

        // Assert
        categorias.Should().HaveCount(2);
        categorias.Select(c => c.Nombre).Should().Contain(new[] { "Periféricos", "Monitores" });
    }

    [Test]
    public async Task GetCategoria_Existente_DevuelveLaCategoria()
    {
        // Act
        var categoria = await CategoriaRepositorio.GetCategoriaAsync(1);

        // Assert
        categoria.Should().NotBeNull();
        categoria!.Nombre.Should().Be("Periféricos");
    }

    [Test]
    public async Task GetCategoria_Inexistente_DevuelveNull()
    {
        // Act: en Cassandra, una clave que no existe no es un error: es un resultado vacio
        var categoria = await CategoriaRepositorio.GetCategoriaAsync(999);

        // Assert
        categoria.Should().BeNull();
    }
}
