using System.CommandLine.Parsing;
using System.Linq;
using System.Net;

namespace Shadowsocks.CLI
{
    /// <summary>
    /// Class for common utility parsers.
    /// Do not put command-specific parsers here.
    /// </summary>
    public static class Parsers
    {
        /// <summary>
        /// Parses the string representation of the port number
        /// into an integer representation.
        /// </summary>
        /// <param name="argumentResult">The argument result.</param>
        /// <returns>The integer representation of the port number.</returns>
        public static int ParsePortNumber(ArgumentResult argumentResult)
        {
            var portString = argumentResult.Tokens.Single().Value;
            if (int.TryParse(portString, out var port))
            {
                if (port is > 0 and <= 65535)
                {
                    return port;
                }
                else
                {
                    argumentResult.ErrorMessage = "Port out of range: (0, 65535]";
                    return default;
                }
            }
            else
            {
                argumentResult.ErrorMessage = $"Invalid port number: {portString}";
                return default;
            }
        }

        public static IPEndPoint? ParseIPEndPoint(ArgumentResult argumentResult)
        {
            if (!argumentResult.Tokens.Any())
                return null;

            var epString = argumentResult.Tokens.Single().Value;

            if (IPEndPoint.TryParse(epString, out var ep))
            {
                return ep;
            }
            else
            {
                argumentResult.ErrorMessage = $"Invalid IP endpoint: {epString}";
                return null;
            }
        }
    }
}
