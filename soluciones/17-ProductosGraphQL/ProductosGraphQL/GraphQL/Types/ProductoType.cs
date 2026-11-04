using HotChocolate.Types;
using ProductosGraphQL.Models;

namespace ProductosGraphQL.GraphQL.Types;

public class ProductoType : ObjectType<Producto>
{
    protected override void Configure(IObjectTypeDescriptor<Producto> descriptor)
    {
        descriptor.Name("Producto");
        descriptor.Description("Producto de la tienda");

        descriptor.Field(p => p.Id)
            .Type<NonNullType<IdType>>()
            .Description("ID unico del producto");

        descriptor.Field(p => p.Nombre)
            .Type<NonNullType<StringType>>()
            .Description("Nombre del producto");

        descriptor.Field(p => p.Precio)
            .Type<NonNullType<DecimalType>>()
            .Description("Precio en euros");

        descriptor.Field(p => p.Imagen)
            .Type<StringType>()
            .Description("URL de la imagen");

        descriptor.Field(p => p.CategoriaId)
            .Type<NonNullType<LongType>>()
            .Description("ID de la categoria");

        descriptor.Field(p => p.UsuarioId)
            .Type<LongType>()
            .Description("ID del usuario propietario");

        descriptor.Field(p => p.IsDeleted)
            .Type<NonNullType<BooleanType>>()
            .Description("Indica si esta eliminado");

        descriptor.Field(p => p.CreatedAt)
            .Type<NonNullType<DateTimeType>>()
            .Description("Fecha de creacion");

        descriptor.Field(p => p.UpdatedAt)
            .Type<DateTimeType>()
            .Description("Fecha de ultima actualizacion");

        descriptor.Field(p => p.Categoria)
            .Type<NonNullType<CategoriaType>>()
            .Description("Categoria del producto");
    }
}
