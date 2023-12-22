# About
---

legallead.logging is a custom implementation of ILogger   
the writes log entries to RDBMS.

## How to Use
---

This package is built targeting NETCORE projects.  

There are two basic steps needed to get started.  
1. Import package
2. Register the `ILoggingService` interface in your DI framework
3. Register the `ILoggingService` interface in your DI framework

## Usage
---   

1. Setup service provider.
	```C#
	// get the internal `IServiceProvider`
	var provider = legallead.logging.LoggingDbServiceProvider.Provider;
	
	// provider object can be null, in case of an unexpected exception
	// so perform a null check
	if (provider == null ) { return; }
	var provider = legallead.logging.LoggingDbServiceProvider.Provider;
	
    builder.AddScoped<ILogConfiguration>(a => provider.GetRequiredService<ILogConfiguration>());
    builder.AddScoped<ILoggingService>(a => provider.GetRequiredService<ILoggingService>());

	```
1. Public members - `ILogConfiguration` - interface
	```C#

	LogConfigurationLevel LogLevel { get; }
	void SetLoggingLevel(LogConfigurationLevel level);

	```
1. Public members - `ILoggingService` - interface
	```C#

	Task LogCritical(string message);
	Task LogDebug(string message);
	Task LogError(Exception exception);
	Task LogInformation(string message);
	Task LogVerbose(string message);
	Task LogWarning(string message);

	```

## Feedback
---  
We hope that you have as much fun using this package as we had creating it! 
You're feedback is welcome, please reach out to LegalLead Tech Team with your comments.

#### Release Notes:

1.0.0 - 20231115 - Initial package creation