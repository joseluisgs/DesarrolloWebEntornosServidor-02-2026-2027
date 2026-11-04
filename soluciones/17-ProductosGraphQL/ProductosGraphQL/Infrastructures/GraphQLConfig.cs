using HotChocolate.Execution.Configuration;
using Microsoft.EntityFrameworkCore;
using ProductosGraphQL.DataLoaders;
using ProductosGraphQL.Entity;
using ProductosGraphQL.GraphQL;
using ProductosGraphQL.GraphQL.Resolvers;
using ProductosGraphQL.GraphQL.Types;
using Serilog;

namespace ProductosGraphQL.Infrastructures;

public static class GraphQLConfig
{
    public static IRequestExecutorBuilder AddGraphQLConfig(
        this IServiceCollection services,
        IWebHostEnvironment environment)
    {
        Log.Information("Configurando GraphQL con HotChocolate...");

        return services
            .AddGraphQLServer()
            .AddAuthorization()
            .AddQueryType<ProductosQuery>()
            .AddMutationType<ProductoMutation>()
            .AddSubscriptionType<Subscription>()
            .AddInMemorySubscriptions()
            .AddType<ProductoType>()
            .AddType<CategoriaType>()
            .AddTypeExtension<ProductoResolvers>()
            .AddDataLoader<CategoriaByIdDataLoader>()
            .AddFiltering()
            .AddSorting()
            .AddProjections()
            .ModifyRequestOptions(opt =>
            {
                opt.IncludeExceptionDetails = environment.IsDevelopment();
            });
    }

    public static void MapGraphQLEndpoints(this WebApplication app)
    {
        app.MapGraphQL();
        app.MapGraphQLWebSocket();
    }
}
