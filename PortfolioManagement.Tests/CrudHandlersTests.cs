using AutoMapper;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using PortfolioManagement.Application.Common;
using PortfolioManagement.Application.Common.Cqrs;
using PortfolioManagement.Domain.Entities;
using PortfolioManagement.Infrastructure.Persistence;
using PortfolioManagement.Infrastructure.Persistence.Repositories;
using Xunit;

namespace PortfolioManagement.Tests;

public class CrudHandlersTests
{
    private readonly ApplicationDbContext _context;
    private readonly UnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public CrudHandlersTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
            
        _context = new ApplicationDbContext(options);
        _unitOfWork = new UnitOfWork(_context);
        
        var mapperConfig = new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<Skill, SkillDto>();
            cfg.CreateMap<SkillRequest, Skill>();
        });
        _mapper = mapperConfig.CreateMapper();
    }

    [Fact]
    public async Task GetPagedQueryHandler_ShouldReturnPagedResults_WhenSortingIsApplied()
    {
        // Arrange
        var handler = new GetPagedQueryHandler<Skill, SkillDto>(_unitOfWork, _mapper);
        _context.Skills.AddRange(
            new Skill { Id = Guid.NewGuid(), Name = "C#", Percentage = 90, Category = "Backend", CreatedAt = DateTimeOffset.UtcNow },
            new Skill { Id = Guid.NewGuid(), Name = "React", Percentage = 80, Category = "Frontend", CreatedAt = DateTimeOffset.UtcNow.AddMinutes(-5) }
        );
        await _context.SaveChangesAsync();

        var request = new GetPagedQuery<Skill, SkillDto>(new QueryParameters { SortBy = "Name", SortDescending = true });

        // Act
        var result = await handler.Handle(request, CancellationToken.None);

        // Assert
        result.Succeeded.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value!.Items.Should().HaveCount(2);
        result.Value.Items[0].Name.Should().Be("React");
        result.Value.Items[1].Name.Should().Be("C#");
    }

    [Fact]
    public async Task GetPagedQueryHandler_ShouldReturnFilteredResults_WhenSearchIsApplied()
    {
        // Arrange
        var handler = new GetPagedQueryHandler<Skill, SkillDto>(_unitOfWork, _mapper);
        _context.Skills.AddRange(
            new Skill { Id = Guid.NewGuid(), Name = "Python", Percentage = 90, Category = "Backend", CreatedAt = DateTimeOffset.UtcNow },
            new Skill { Id = Guid.NewGuid(), Name = "TypeScript", Percentage = 80, Category = "Frontend", CreatedAt = DateTimeOffset.UtcNow }
        );
        await _context.SaveChangesAsync();

        var request = new GetPagedQuery<Skill, SkillDto>(new QueryParameters { Search = "type" });

        // Act
        var result = await handler.Handle(request, CancellationToken.None);

        // Assert
        result.Succeeded.Should().BeTrue();
        result.Value!.Items.Should().HaveCount(1);
        result.Value.Items[0].Name.Should().Be("TypeScript");
    }
}

public class SkillDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int Percentage { get; set; }
    public string Category { get; set; } = string.Empty;
}

public class SkillRequest
{
    public string Name { get; set; } = string.Empty;
    public int Percentage { get; set; }
    public string Category { get; set; } = string.Empty;
}
