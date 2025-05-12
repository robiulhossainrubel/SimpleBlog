using System.Reflection;
using SimpleBlog.Application.DTOs;
using SimpleBlog.Domain.Entities;
using SimpleBlog.Infrastructure.Data;
using SimpleBlog.Presentation;

namespace SimpleBlog.Test.Layers;

public class BaseTest
{
    protected static readonly Assembly DomainAssembly = typeof(Post).Assembly;
    protected static readonly Assembly ApplicationAssembly = typeof(PostDTO).Assembly;
    protected static readonly Assembly InfrastructureAssembly = typeof(BlogDbContext).Assembly;
    protected static readonly Assembly PresentationAssembly = typeof(Program).Assembly;
}
