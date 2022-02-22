using System;
using System.CommandLine.Parsing;
using System.Linq;

namespace Shadowsocks.CLI.Utils;

public static class SystemCommandLineExtensions
{
    public static bool ContainsSymbolWithName(this CommandResult commandResult, string name) =>
        commandResult.Children.Any(x => x.Symbol.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
}
