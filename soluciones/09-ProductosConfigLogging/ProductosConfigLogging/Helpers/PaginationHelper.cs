namespace ProductosConfigLogging.Helpers;

public static class PaginationHelper
{
    public static string CreateLinkHeader(int page, int totalPages, int pageSize, string baseUrl)
    {
        var links = new List<string>
        {
            $"<{baseUrl}?page=1&pageSize={pageSize}>; rel=\"first\""
        };

        if (page > 1)
        {
            links.Add($"<{baseUrl}?page={page - 1}&pageSize={pageSize}>; rel=\"prev\"");
        }

        if (page < totalPages)
        {
            links.Add($"<{baseUrl}?page={page + 1}&pageSize={pageSize}>; rel=\"next\"");
        }

        var lastPage = Math.Max(totalPages, 1);
        links.Add($"<{baseUrl}?page={lastPage}&pageSize={pageSize}>; rel=\"last\"");

        return string.Join(", ", links);
    }
}
