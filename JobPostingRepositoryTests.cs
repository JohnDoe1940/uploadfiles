using DevSpot.Data;
using DevSpot.Models;
using DevSpot.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.Blazor;

namespace DevSpot.tests;

public class JobPostingRepositoryTests
{
    private readonly DbContextOptions<ApplicationDbContext> _options;

    public JobPostingRepositoryTests()
    {
        _options = new DbContextOptionsBuilder<ApplicationDbContext>().UseInMemoryDatabase("JobPostingDb").Options;
    }
    
    private ApplicationDbContext CreateDbContext() => new ApplicationDbContext(_options);

    [Fact]
    public async Task AddAsync_ShouldAddJobPosting()
    {
        var db = CreateDbContext();

        var repository = new JobPostingRepository(db);

        var jobPosting = new JobPosting
        {
            Title = "Test Job Posting",
            Description = "Test Description",
            Company = "Test Company",
            Location = "Test Location",
            PostedDate = DateTime.UtcNow,
            IsApproved = true,
            UserId = "Test User Id"
        };
        
        
        repository.AddAsync(jobPosting);
        
        
        var result = db.JobPostings.Find(jobPosting.Id);
        
        Assert.NotNull(result);
        Assert.Equal("Test Job Posting", result.Title);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnJobPosting()
    {
        var db = CreateDbContext();

        var repository = new JobPostingRepository(db);

        var jobPosting = new JobPosting
        {
            Title = "Test Job Posting",
            Description = "Test Description",
            Company = "Test Company",
            Location = "Test Location",
            PostedDate = DateTime.UtcNow,
            IsApproved = true,
            UserId = "Test User Id"
        };
        
        await db.JobPostings.AddAsync(jobPosting);
        await db.SaveChangesAsync();
        
        var result = await repository.GetByIdAsync(jobPosting.Id);
        
        Assert.NotNull(result);
        Assert.Equal("Test Job Posting", result.Title);
    }
    
    
    [Fact]
    public async Task GetByIdAsync_ShouldReturnThrowsException()
    {
        var db = CreateDbContext();
        var repository = new JobPostingRepository(db);
        await  Assert.ThrowsAsync<KeyNotFoundException>(() => repository.GetByIdAsync(123));
    }
    
    [Fact]
    public async Task GetAllAsync_ShouldReturnJobPostings()
    {
        var db = CreateDbContext();
        
        var repository = new JobPostingRepository(db);

        var jobPosting1 = new JobPosting
        {
            Title = "Test Job Posting",
            Description = "Test Description",
            Company = "Test Company",
            Location = "Test Location",
            PostedDate = DateTime.UtcNow,
            IsApproved = true,
            UserId = "Test User Id"
        };
        
        var jobPosting2 = new JobPosting
        {
            Title = "Second Job Posting",
            Description = "Second Description",
            Company = "Second Company",
            Location = "Second Location",
            PostedDate = DateTime.UtcNow,
            IsApproved = true,
            UserId = "Second User Id"
        };
        
        await db.JobPostings.AddRangeAsync(jobPosting1, jobPosting2);
        await db.SaveChangesAsync();
        
        var result = await repository.GetAllAsync();
        
        Assert.NotNull(result);
        Assert.True(result.Count() > 1);
        
    }

    [Fact]
    public async Task UpdateAsync_ShouldUpdateJobPosting()
    {
        var db = CreateDbContext();
        var repository = new JobPostingRepository(db);
        var jobPosting = new JobPosting
        {
            Title = "Test Job Posting",
            Description = "Test Description",
            Company = "Test Company",
            Location = "Test Location",
            PostedDate = DateTime.UtcNow,
            IsApproved = true,
            UserId = "Test User Id"
        };
        await db.JobPostings.AddAsync(jobPosting);
        await db.SaveChangesAsync();
        jobPosting.Title = "Updated Job Posting";
        await repository.UpdateAsync(jobPosting);
        var result = db.JobPostings.Find(jobPosting.Id);
        Assert.NotNull(result);
        Assert.Equal("Updated Job Posting", result.Title);
    }

    [Fact]
    public async Task DeleteAsync_ShouldDeleteJobPosting()
    {
        var db = CreateDbContext();

        var repository = new JobPostingRepository(db);

        var jobPosting = new JobPosting
        {
            Title = "Test Job Posting",
            Description = "Test Description",
            Company = "Test Company",
            Location = "Test Location",
            PostedDate = DateTime.UtcNow,
            IsApproved = true,
            UserId = "Test User Id"
        };
        
        await db.JobPostings.AddAsync(jobPosting);
        await db.SaveChangesAsync();
        
        await repository.DeleteAsync(jobPosting.Id);
        
        var result = db.JobPostings.Find(jobPosting.Id);
        
        Assert.Null(result);
    }
    
}