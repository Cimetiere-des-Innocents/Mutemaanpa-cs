/// dependency injection:
/// https://learn.microsoft.com/en-us/dotnet/core/extensions/dependency-injection
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);

/// <summary>
///
/// App does dependency injection for mutemaanpa-cs. That helps to decouple our
/// modules.
///
/// </summary>
///
public class App {

}
