using FluentAssertions;
using NUnit.Framework;
using TenistasSync.Console.Models;
using TenistasSync.Console.Utils;

namespace TenistasSync.Tests;

[TestFixture]
public class CollectionUtilsTests
{
    private List<Tenista> _tenistas = null!;

    [SetUp]
    public void SetUp()
    {
        _tenistas = new List<Tenista>
        {
            new() { Id = 1, Nombre = "Rafael Nadal", Ranking = 1, Pais = "España", Altura = 185, Peso = 85, Titulos = 22, FechaNacimiento = new DateTime(1986, 6, 3) },
            new() { Id = 2, Nombre = "Novak Djokovic", Ranking = 2, Pais = "Serbia", Altura = 188, Peso = 77, Titulos = 24, FechaNacimiento = new DateTime(1987, 5, 22) },
            new() { Id = 3, Nombre = "Carlos Alcaraz", Ranking = 3, Pais = "España", Altura = 183, Peso = 74, Titulos = 5, FechaNacimiento = new DateTime(2003, 5, 5) }
        };
    }

    [Test]
    public void OrdenarPorRanking_ShouldReturnSortedList()
    {
        // Arrange - insert in random order
        var desordenados = new List<Tenista> { _tenistas[2], _tenistas[0], _tenistas[1] };

        // Act
        var ordenados = CollectionUtils.OrdenarPorRanking(desordenados);

        // Assert
        ordenados[0].Ranking.Should().Be(1);
        ordenados[1].Ranking.Should().Be(2);
        ordenados[2].Ranking.Should().Be(3);
    }

    [Test]
    public void FiltrarPorPais_ShouldReturnMatchingTenistas()
    {
        // Act
        var españoles = CollectionUtils.FiltrarPorPais(_tenistas, "España");

        // Assert
        españoles.Should().HaveCount(2);
        españoles.Should().AllSatisfy(t => t.Pais.Should().Be("España"));
    }

    [Test]
    public void MapearANombres_ShouldReturnListOfNames()
    {
        // Act
        var nombres = CollectionUtils.MapearANombres(_tenistas);

        // Assert
        nombres.Should().HaveCount(3);
        nombres.Should().Contain("Rafael Nadal");
        nombres.Should().Contain("Novak Djokovic");
    }

    [Test]
    public void ContarConCondicion_ShouldReturnCorrectCount()
    {
        // Act
        var count = CollectionUtils.ContarConCondicion(_tenistas, t => t.Titulos > 10);

        // Assert
        count.Should().Be(2); // Nadal and Djokovic
    }

    [Test]
    public void EncontrarPrimero_ShouldReturnFirstMatch()
    {
        // Act
        var tenista = CollectionUtils.EncontrarPrimero(_tenistas, t => t.Pais == "Serbia");

        // Assert
        tenista.Should().NotBeNull();
        tenista!.Nombre.Should().Be("Novak Djokovic");
    }

    [Test]
    public void AlgunoCumple_ShouldReturnTrue_WhenConditionMet()
    {
        // Act
        var resultado = CollectionUtils.AlgunoCumple(_tenistas, t => t.Edad < 25);

        // Assert
        resultado.Should().BeTrue();
    }

    [Test]
    public void TodosCumplen_ShouldReturnFalse_WhenNotAllMatch()
    {
        // Act
        var resultado = CollectionUtils.TodosCumplen(_tenistas, t => t.Edad < 30);

        // Assert
        resultado.Should().BeFalse(); // Not all are under 30
    }

    [Test]
    public void AgruparPorPais_ShouldGroupCorrectly()
    {
        // Act
        var agrupados = CollectionUtils.AgruparPorPais(_tenistas);

        // Assert
        agrupados.Should().HaveCount(2);
        agrupados["España"].Should().HaveCount(2);
        agrupados["Serbia"].Should().HaveCount(1);
    }

    [Test]
    public void EstadisticasTitulos_ShouldCalculateCorrectly()
    {
        // Act
        var stats = CollectionUtils.EstadisticasTitulos(_tenistas);

        // Assert
        stats.Total.Should().Be(51); // 22 + 24 + 5
        stats.Promedio.Should().BeApproximately(17.0, 0.1);
        stats.Minimo.Should().Be(5);
        stats.Maximo.Should().Be(24);
    }

    [Test]
    public void TopN_ShouldReturnTopEntries()
    {
        // Act
        var top2 = CollectionUtils.TopN(_tenistas, 2);

        // Assert
        top2.Should().HaveCount(2);
        top2[0].Ranking.Should().Be(1);
        top2[1].Ranking.Should().Be(2);
    }

    [Test]
    public void Particionar_ShouldSplitCorrectly()
    {
        // Act
        var (jovenes, veteranos) = CollectionUtils.Particionar(_tenistas, t => t.Edad < 30);

        // Assert
        jovenes.Should().HaveCount(1); // Carlos Alcaraz
        veteranos.Should().HaveCount(2); // Nadal and Djokovic
    }
}
