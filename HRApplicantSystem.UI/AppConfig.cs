using System;
using Microsoft.Extensions.Configuration;

namespace HRApplicantSystem.UI;

public static class AppConfig
{
    public static string ConnectionString { get; private set; } = string.Empty;

    public static void Load()
    {
        var config = new ConfigurationBuilder()
            .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: false)
            .Build();

        ConnectionString = config.GetConnectionString("DefaultConnection")
                           ?? throw new Exception("Connection string not found!");
    }
}