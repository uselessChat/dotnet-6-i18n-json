# I18n localization
The idea of this playground is to setup an external library which has the `json` files with the translations for different cultures. The directory is `Resources` on project `I18nJsonLibrary`.

To add other file remember to edit the `json` file with the following properties:
- Build Action: **Embedded resource**
- Copy to Output Directory: **Copy always**

The library exposes `I18nJsonService#AvailableCultures` to retrieve cultures from current embedded resources.

To change the strategy to retrieve the localized value go to `I18nJsonStringLocalizerFactory` and update accordingly.

## Initial setup will always read from `.json` file
- Install packages
  - `Microsoft.Extensions.Localization.Abstractions 6.0.14`
  - `Newtonsoft.Json 13.0.2`
- Add services
  ```csharp
  builder.Services.AddLocalization();
  builder.Services.AddSingleton<IStringLocalizerFactory, I18nJsonStringLocalizerFactory>();
  builder.Services.AddSingleton<I18nJsonService>();
  builder.Services.AddSingleton<I18nMiddleware>();
  ```
- Add to pipeline
  ```csharp
  // Middlewares
  app.UseMiddleware<I18nMiddleware>();
  // Localization
  var supportedCultures = app.Services.GetService<I18nJsonService>()!.AvailableCultures().ToList();
  var requestLocalizationOptions = new RequestLocalizationOptions()
  {
      // You must explicitly state which cultures your application supports.
      DefaultRequestCulture = new RequestCulture(supportedCultures.First()),
      // These are the cultures the app supports for formatting numbers, dates, etc.
      SupportedCultures = supportedCultures,
      // These are the cultures the app supports for UI strings (that we have localized resources for).
      SupportedUICultures = supportedCultures,
  };
  app.UseRequestLocalization(requestLocalizationOptions);
  ```

## Distribuited Cache - In-memory
- Install packages
  - `Microsoft.Extensions.Caching.Abstractions 6.0`
- Add service
  - `builder.Services.AddDistributedMemoryCache();`
- Update class `I18nJsonStringLocalizerFactory` and return the instance of `I18nJsonStringLocalizerCache`

Resources
- [Localization with JSON files in .NET 6](https://cloudcoders.xyz/blog/localization-with-json-files-in-net6/)
- [Using embedded files in dotnet core](https://josef.codes/using-embedded-files-in-dotnet-core/)
- [rails i18n es-US](https://github.com/svenfuchs/rails-i18n/blob/master/rails/locale/en-US.yml)
