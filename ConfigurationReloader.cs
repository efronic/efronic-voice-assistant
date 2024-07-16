using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using System;

public class ConfigurationReloader
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<ConfigurationReloader> _logger;
    private IDisposable _changeTokenRegistration;

    public ConfigurationReloader(IConfiguration configuration, ILogger<ConfigurationReloader> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    public void StartListening()
    {
        _changeTokenRegistration = ChangeToken.OnChange(
            () => _configuration.GetReloadToken(),
            OnConfigurationChanged);
    }

    private void OnConfigurationChanged()
    {
        _logger.LogInformation("Configuration changed. Reloading...");
        // Handle your configuration changes here
        // For example, you might want to reload some specific settings or notify other parts of the application
    }
}
