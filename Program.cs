using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using ICSharpCode.Decompiler;
using ICSharpCode.Decompiler.CSharp;
using ICSharpCode.Decompiler.TypeSystem;

public class Program
{
    public static void Main(string[] args)
    {
        using (var host = WebHost.Start((IRouteBuilder router) 
            => router.MapGet("api/decompile/csharp", async (req, res, data) =>
                {
                    if (req.Query.TryGetValue("assemblyFileName", out var assemblyFileName)
                        && req.Query.TryGetValue("typeName", out var typeName))
                    {
                        var decompiler = new CSharpDecompiler(assemblyFileName, new DecompilerSettings { ThrowOnAssemblyResolveErrors = true });

                        var decompiledType = decompiler.DecompileType(new FullTypeName(typeName));
                        await res.WriteAsync(decompiledType.ToString());
                    }
                    else
                    {
                        res.StatusCode = 400;
                        await res.WriteAsync($"Missing query string parameters: '{nameof(assemblyFileName)}', '{nameof(typeName)}'.");
                    }
                })))
        {
            host.WaitForShutdown();
        }
    }
}
