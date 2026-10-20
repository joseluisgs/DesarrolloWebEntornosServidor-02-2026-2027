namespace ProductosEFCore.Helpers;

public static class PaginationHelper
{
    public static string CreateLinkHeader(int page, int totalPages, int pageSize, string baseUrl)
    {
        var links = new List<string>();

        if (page > 1)
        {
            links.Add($"<{baseUrl}?page=1&pageSize={pageSize}>; rel=\"first\"");
            links.Add($"<{baseUrl}?page={page - 1}&pageSize={pageSize}>; rel=\"prev\"");
        }

        if (page < totalPages)
        {
            links.Add($"<{baseUrl}?page={page + 1}&pageSize={pageSize}>; rel=\"next\"");
            links.Add($"<{baseUrl}?page={totalPages}&pageSize={pageSize}>; rel=\"last\"");
        }

        return string.Join(", ", links);
    }
}
