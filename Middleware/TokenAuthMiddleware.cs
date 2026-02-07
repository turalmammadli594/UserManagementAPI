namespace UserManagementAPI.Middleware;

public class TokenAuthMiddleware
{
    private readonly RequestDelegate _next;
    private readonly string _expectedToken;

    public TokenAuthMiddleware(RequestDelegate next, IConfiguration config)
    {
        _next = next;
        _expectedToken = config["Auth:Token"] ?? "";
    }

    public async Task Invoke(HttpContext context)
    {
        // Swagger və root-u auth-dan çıxarırıq ki rahat test edəsən
        var path = context.Request.Path.Value?.ToLowerInvariant() ?? "";
        if (path.StartsWith("/swagger") || path == "/")
        {
            await _next(context);
            return;
        }

        if (!context.Request.Headers.TryGetValue("Authorization", out var authHeader))
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await context.Response.WriteAsJsonAsync(new { error = "Unauthorized" });
            return;
        }

        var auth = authHeader.ToString();
        const string prefix = "Bearer ";

        if (!auth.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await context.Response.WriteAsJsonAsync(new { error = "Unauthorized" });
            return;
        }

        var token = auth.Substring(prefix.Length).Trim();
        if (string.IsNullOrWhiteSpace(_expectedToken) || token != _expectedToken)
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await context.Response.WriteAsJsonAsync(new { error = "Unauthorized" });
            return;
        }

        await _next(context);
    }
}
