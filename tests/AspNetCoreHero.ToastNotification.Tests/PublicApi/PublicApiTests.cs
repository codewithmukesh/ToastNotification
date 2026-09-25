using AspNetCoreHero.ToastNotification.Abstractions;
using PublicApiGenerator;
using System.Runtime.CompilerServices;

namespace AspNetCoreHero.ToastNotification.Tests.PublicApi;

/// <summary>
/// Fails when the public API changes, so breaking changes are always deliberate.
/// If the change is intended, copy PublicApi.received.txt over PublicApi.approved.txt and note it in the CHANGELOG.
/// </summary>
public class PublicApiTests
{
    [Fact]
    public void Public_api_has_not_changed()
    {
        var api = typeof(INotyfService).Assembly.GeneratePublicApi(new ApiGeneratorOptions
        {
            // AddNotyf / UseNotyf live in the Microsoft.* DI and Builder namespaces on purpose.
            AllowNamespacePrefixes = ["Microsoft.AspNetCore.Builder", "Microsoft.Extensions.DependencyInjection"],
            ExcludeAttributes =
            [
                "System.Runtime.Versioning.TargetFrameworkAttribute",
                "System.Reflection.AssemblyMetadataAttribute"
            ]
        }).ReplaceLineEndings("\n");

        var folder = Folder();
        var approvedPath = Path.Combine(folder, "PublicApi.approved.txt");
        var receivedPath = Path.Combine(folder, "PublicApi.received.txt");
        var approved = File.Exists(approvedPath) ? File.ReadAllText(approvedPath).ReplaceLineEndings("\n") : string.Empty;

        if (approved != api)
        {
            File.WriteAllText(receivedPath, api);
            Assert.Fail($"Public API changed. Review {receivedPath} and, if intended, copy it over {approvedPath}.");
        }

        File.Delete(receivedPath);
    }

    private static string Folder([CallerFilePath] string path = "") => Path.GetDirectoryName(path)!;
}
