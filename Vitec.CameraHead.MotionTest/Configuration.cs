namespace Vitec.CameraHead.MotionTest {
    using System;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using Microsoft.Extensions.Configuration;

    /// <summary>
    ///     Extension methods for Building and accessing configurations
    /// </summary>
    public static class Configuration {

        /// <summary>
        ///     Helper method for building a Configuration from multiple sources
        ///     Especially useful from non-Web projects
        ///     NOTE: Uses same order of precedence as IWebHost.CreateDefaultBuilder
        /// </summary>
        /// <param name="executingAssembly">assembly used for loading User Secrets</param>
        /// <param name="args">optional command line arguments</param>
        /// <param name="jsonFiles">optional additional JSON files to add</param>
        /// <returns></returns>
        public static IConfiguration BuildConfiguration(Assembly executingAssembly, string[] args = null, string[] jsonFiles = null) {

            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", true, true);

            if (jsonFiles != null) {
                foreach (var jsonFile in jsonFiles) {
                    builder.AddJsonFile(jsonFile);
                }
            }

            builder.AddUserSecrets(executingAssembly, true);

            builder.AddEnvironmentVariables();

            if (args != null && args.Any()) {
                builder.AddCommandLine(args);
            }

            return builder.Build();
        }

        /// <summary>
        ///     Get a Section from the Configuration and deserialize to type T
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="configuration"></param>
        /// <param name="sectionName"></param>
        /// <returns></returns>
        public static T GetConfigurationSection<T>(this IConfiguration configuration, string sectionName) {
            var configurationSection = configuration.GetSection(sectionName);

            if (configurationSection == null)
                throw new ApplicationException(
                    $"Section {sectionName} not defined - Add User secrets or update appsettings.json");

            var configurationObjects = configurationSection.Get<T>();

            return configurationObjects;
        }

    }
}