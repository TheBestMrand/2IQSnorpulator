using System;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core;
using Avalonia.Data.Core.Plugins;
using System.Linq;
using Avalonia.Markup.Xaml;
using Core.Services;
using Data.Database;
using Data.Repositories;
using Desktop.ViewModels;
using Desktop.Views;
using Microsoft.Extensions.DependencyInjection;

namespace Desktop;

public partial class App : Application
{
    public static IServiceProvider? Services { get; private set; }
    
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        // Setup Dependency Injection
        var services = new ServiceCollection();
        ConfigureServices(services);
        Services = services.BuildServiceProvider();

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            DisableAvaloniaDataAnnotationValidation();
            
            // Create MainWindow with DI
            var mainViewModel = Services.GetRequiredService<MainWindowViewModel>();
            desktop.MainWindow = new MainWindow
            {
                DataContext = mainViewModel
            };
        }

        base.OnFrameworkInitializationCompleted();
    }
    
    private void ConfigureServices(ServiceCollection services)
    {
        services.AddSingleton<DbService>();
        services.AddSingleton<RequestDbRepository>();
        services.AddSingleton<HistoryDbRepository>();
        services.AddSingleton<CollectionDbRepository>();
        services.AddSingleton<SessionDbRepository>();
        services.AddSingleton<EnvironmentDbRepository>();
        services.AddSingleton<ScriptRunnerService>();
        services.AddSingleton<StandardVariablesProvider>();
        services.AddSingleton<VariableResolver>();
        services.AddSingleton<GreatRequestExecutor>();
        services.AddSingleton<CollectionService>();
        services.AddSingleton<MainWindowViewModel>();
    }

    private void DisableAvaloniaDataAnnotationValidation()
    {
        // Get an array of plugins to remove
        var dataValidationPluginsToRemove =
            BindingPlugins.DataValidators.OfType<DataAnnotationsValidationPlugin>().ToArray();

        // remove each entry found
        foreach (var plugin in dataValidationPluginsToRemove)
        {
            BindingPlugins.DataValidators.Remove(plugin);
        }
    }
}