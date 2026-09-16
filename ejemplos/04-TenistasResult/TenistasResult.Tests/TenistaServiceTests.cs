using FluentAssertions;
using NUnit.Framework;
using TenistasResult.Console.Services;

namespace TenistasResult.Tests;

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
    public void FindById_ExistingId_ShouldReturnSuccess()
    {
        var result = _service.FindById(1);
        
        result.IsSuccess.Should().BeTrue();
        result.Value.Id.Should().Be(1);
    }

    [Test]
    public void FindById_NonExistingId_ShouldReturnFailure()
    {
        var result = _service.FindById(999);
        
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("no encontrado");
    }

    [Test]
    public void CreateTenista_ValidData_ShouldReturnSuccess()
    {
        var result = _service.CreateTenista("Roger Federer", 10, "Suiza", 20);
        
        result.IsSuccess.Should().BeTrue();
        result.Value.Nombre.Should().Be("Roger Federer");
    }

    [Test]
    public void CreateTenista_InvalidName_ShouldReturnFailure()
    {
        var result = _service.CreateTenista("", 10, "Suiza", 20);
        
        result.IsFailure.Should().BeTrue();
    }

    [Test]
    public void CreateTenista_DuplicateRanking_ShouldReturnFailure()
    {
        var result = _service.CreateTenista("Test Player", 1, "Test", 0);
        
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("Ya existe");
    }

    [Test]
    public void UpdateRanking_ValidData_ShouldReturnSuccess()
    {
        var result = _service.UpdateRanking(1, 5);
        
        result.IsSuccess.Should().BeTrue();
        result.Value.Ranking.Should().Be(5);
    }

    [Test]
    public void UpdateRanking_InvalidRanking_ShouldReturnFailure()
    {
        var result = _service.UpdateRanking(1, 0);
        
        result.IsFailure.Should().BeTrue();
    }

    [Test]
    public void GetTopN_ValidN_ShouldReturnSuccess()
    {
        var result = _service.GetTopN(2);
        
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(2);
    }

    [Test]
    public void GetTopN_InvalidN_ShouldReturnFailure()
    {
        var result = _service.GetTopN(0);
        
        result.IsFailure.Should().BeTrue();
    }

    [Test]
    public void GetTopN_NTooLarge_ShouldReturnFailure()
    {
        var result = _service.GetTopN(1000);
        
        result.IsFailure.Should().BeTrue();
    }
}
