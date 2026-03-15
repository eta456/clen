using System;
using System.Threading.Tasks;
using AppsDBLib.Core.Models;
using AppsDBLib.Infrastructure.Contexts;
using AppsDBLib.Infrastructure.Entities;
using AppsDBLib.Infrastructure.Repositories;
using FluentAssertions;
using Mapster;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AppsDBLib.Tests.Repositories
{
    [TestClass]
    public class AppRepositoryTests
    {
        [TestInitialize]
        public void Setup()
        {
            // Initialise Mapster's global configuration before each test
            TypeAdapterConfig.GlobalSettings.Default.PreserveReference(true);
        }

        private AppDbContext GetInMemoryDbContext()
        {
            // Using a unique Guid ensures a fresh database for every single test
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            return new AppDbContext(options);
        }

        [TestMethod]
        public async Task AddAsync_ShouldPersistEntity_AndReturnMappedDomainModel()
        {
            // Arrange
            using var context = GetInMemoryDbContext();
            var repository = new AppRepository(context);
            var newAppModel = AppModel.CreateNew("Nexus Payment Gateway", "Finance Dept");

            // Act
            var result = await repository.AddAsync(newAppModel);

            // Assert: Verify the returned Domain Model
            result.Should().NotBeNull();
            result.Id.Should().BeGreaterThan(0, "because the database should generate a primary key");
            result.Name.Should().Be("Nexus Payment Gateway");

            // Assert: Verify the actual Entity Framework Database State
            var savedEntity = await context.Set<AppEntity>().FindAsync(result.Id);
            savedEntity.Should().NotBeNull("because the entity must exist in the database");
            savedEntity.AppName.Should().Be("Nexus Payment Gateway", "because Mapster mapped the Name property to AppName");
        }

        [TestMethod]
        public async Task GetByIdAsync_ShouldReturnMappedDomainModel_WhenEntityExists()
        {
            // Arrange
            using var context = GetInMemoryDbContext();
            
            // Manually seed the database with an entity
            var seededEntity = new AppEntity { AppName = "Legacy CRM", OwnerName = "Sales" };
            context.Set<AppEntity>().Add(seededEntity);
            await context.SaveChangesAsync();

            var repository = new AppRepository(context);

            // Act
            var result = await repository.GetByIdAsync(seededEntity.Id);

            // Assert
            result.Should().NotBeNull("because the entity exists in the seeded database");
            result.Id.Should().Be(seededEntity.Id);
            result.Name.Should().Be("Legacy CRM");
        }

        [TestMethod]
        public async Task GetAllAsync_ShouldReturnAllMappedDomainModels()
        {
            // Arrange
            using var context = GetInMemoryDbContext();
            
            context.Set<AppEntity>().AddRange(
                new AppEntity { AppName = "App One", OwnerName = "IT" },
                new AppEntity { AppName = "App Two", OwnerName = "HR" }
            );
            await context.SaveChangesAsync();

            var repository = new AppRepository(context);

            // Act
            var results = await repository.GetAllAsync();

            // Assert
            results.Should().NotBeNull();
            results.Should().HaveCount(2, "because we seeded exactly two entities");
            results.Should().Contain(a => a.Name == "App One");
            results.Should().Contain(a => a.Name == "App Two");
        }
    }
}