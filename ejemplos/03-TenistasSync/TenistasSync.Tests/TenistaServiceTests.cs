using FluentAssertions;
using NUnit.Framework;
using TenistasSync.Console.Services;

namespace TenistasSync.Tests;

[TestFixture]
public class TenistaServiceTests
{
    private TenistaService _service = null!;

    [SetUp]
    public void SetUp()
    {
        _service = new TenistaService();
    }

    [Test]
    public void ObtenerTodos_ShouldReturnAllTenistas()
    {
        // Act
        var tenistas = _service.ObtenerTodos();

        // Assert
        tenistas.Should().NotBeEmpty();
        tenistas.Count.Should().BeGreaterThan(5);
    }

    [Test]
    public void BuscarPorId_ExistingId_ShouldReturnTenista()
    {
        // Act
        var tenista = _service.BuscarPorId(1);

        // Assert
        tenista.Should().NotBeNull();
        tenista!.Id.Should().Be(1);
    }

    [Test]
    public void BuscarPorId_NonExistingId_ShouldReturnNull()
    {
        // Act
        var tenista = _service.BuscarPorId(999);

        // Assert
        tenista.Should().BeNull();
    }

    [Test]
    public void ObtenerPorPais_ShouldReturnMatchingTenistas()
    {
        // Act
        var españoles = _service.ObtenerPorPais("España");

        // Assert
        españoles.Should().NotBeEmpty();
        españoles.Should().AllSatisfy(t => t.Pais.Should().Be("España"));
    }

    [Test]
    public void ObtenerTopN_ShouldReturnCorrectNumber()
    {
        // Act
        var top3 = _service.ObtenerTopN(3);

        // Assert
        top3.Should().HaveCount(3);
        top3[0].Ranking.Should().Be(1);
        top3[1].Ranking.Should().Be(2);
        top3[2].Ranking.Should().Be(3);
    }

    [Test]
    public void ObtenerConMasDeTitulos_ShouldFilterCorrectly()
    {
        // Act
        var conMuchosGrandSlams = _service.ObtenerConMasDeTitulos(10);

        // Assert
        conMuchosGrandSlams.Should().NotBeEmpty();
        conMuchosGrandSlams.Should().AllSatisfy(t => t.Titulos.Should().BeGreaterThan(10));
    }

    [Test]
    public void AgruparPorPais_ShouldGroupCorrectly()
    {
        // Act
        var agrupados = _service.AgruparPorPais();

        // Assert
        agrupados.Should().NotBeEmpty();
        agrupados.Should().ContainKey("España");
    }

    [Test]
    public void ObtenerEstadisticasTitulos_ShouldCalculateCorrectly()
    {
        // Act
        var stats = _service.ObtenerEstadisticasTitulos();

        // Assert
        stats.Total.Should().BeGreaterThan(0);
        stats.Promedio.Should().BeGreaterThan(0);
        stats.Minimo.Should().BeGreaterOrEqualTo(0);
        stats.Maximo.Should().BeGreaterThan(0);
    }

    [Test]
    public void ObtenerEdadPromedio_ShouldReturnValidAverage()
    {
        // Act
        var promedio = _service.ObtenerEdadPromedio();

        // Assert
        promedio.Should().BeGreaterThan(0);
        promedio.Should().BeLessThan(100);
    }

    [Test]
    public void ObtenerJovenes_ShouldReturnYoungPlayers()
    {
        // Act
        var jovenes = _service.ObtenerJovenes();

        // Assert
        jovenes.Should().AllSatisfy(t => t.Edad.Should().BeLessThan(25));
    }

    [Test]
    public void ObtenerTenistaConMasTitulos_ShouldReturnTopChampion()
    {
        // Act
        var campeon = _service.ObtenerTenistaConMasTitulos();

        // Assert
        campeon.Should().NotBeNull();
        campeon!.Titulos.Should().BeGreaterOrEqualTo(20);
    }

    [Test]
    public void ContarPorCondicion_ShouldReturnCorrectCount()
    {
        // Act
        var count = _service.ContarPorCondicion(t => t.Pais == "España");

        // Assert
        count.Should().BeGreaterThan(0);
    }

    [Test]
    public void ExisteConCondicion_ShouldReturnTrueWhenExists()
    {
        // Act
        var existe = _service.ExisteConCondicion(t => t.Pais == "España");

        // Assert
        existe.Should().BeTrue();
    }

    [Test]
    public void ExisteConCondicion_ShouldReturnFalseWhenNotExists()
    {
        // Act
        var existe = _service.ExisteConCondicion(t => t.Pais == "PaisInexistente");

        // Assert
        existe.Should().BeFalse();
    }

    [Test]
    public void ObtenerNombres_ShouldReturnAllNames()
    {
        // Act
        var nombres = _service.ObtenerNombres();

        // Assert
        nombres.Should().NotBeEmpty();
        nombres.Should().AllSatisfy(n => n.Should().NotBeNullOrEmpty());
    }

    [Test]
    public void AgruparPorRangoEdad_ShouldGroupCorrectly()
    {
        // Act
        var agrupados = _service.AgruparPorRangoEdad();

        // Assert
        agrupados.Should().NotBeEmpty();
        agrupados.Values.Sum(list => list.Count).Should().Be(_service.ObtenerTodos().Count);
    }
}
