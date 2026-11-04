using HotChocolate.Types;
using ProductosGraphQL.Models;

namespace ProductosGraphQL.GraphQL.Types;

public class CategoriaType : ObjectType<Categoria>
{
    protected override void Configure(IObjectTypeDescriptor<Categoria> descriptor)
    {
        descriptor.Name("Categoria");
        descriptor.Description("Categoria de productos");

        descriptor.Field(c => c.Id)
            .Type<NonNullType<IdType>>()
            .Description("ID unico de la categoria");

        descriptor.Field(c => c.Nombre)
            .Type<NonNullType<StringType>>()
            .Description("Nombre de la categoria");

        descriptor.Field(c => c.Descripcion)
            .Type<StringType>()
            .Description("Descripcion de la categoria");
    }
}
