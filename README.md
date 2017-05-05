# Webpack.NET

Integrating webpack into ASP.NET MVC projects.

## Asset Caching
Caching is good <sup>[citation needed]</sup>.  The [webpack documentation](https://webpack.js.org/guides/caching/#components/sidebar/sidebar.jsx) has a good description of the problem and proposed solution for caching of webpack assets so this focuses on how that can be integrated into your ASP.NET MVC project.

Simplified approach:
 1. Configure webpack to include a `[chunkhash]` in the output filenames
 2. Use a plugin to output a manifest file containing the generated asset names
 3. Configure Webpack.NET to look for the asset manifest
 4. Update the MVC project to include both manifest and asset files in publish.
 5. Use `UrlHelper` extension methods to reference generated assets in Razor views

### Configure webpack
Start with your existing webpack configuration.  It may look something like this:

```javascript
const outputPath = path.resolve(__dirname, 'Scripts')
const app = {
  entry: {
    app: 'app.ts'
  },
  output: {
    path: outputPath,
    filename: '[name].js'
  },
  resolve: {
    extensions: ['.ts', '.js'],
    modules: ['Scripts/src', 'node_modules']
  },
  module: {
    loaders: [
      { test: /\.tsx?$/, use: ['awesome-typescript-loader'] }
    ]
  }
};
module.exports = app;
```
> Note: this example uses a named output of `app`, meaning we can export multiple assets as required

Update the `output.filename` to be `[name].[chunkhash].js`.  This instructs webpack to calculate a hash of the file contents and insert it into the generated filename, so your final file will be named e.g. `app.18bd82ec9735e5b196af.js`

#### Dev mode
Generating that chunk hash takes a bit of time so you can gain a minor performance improvement during development by removing `[chunkhash]` from the filename:

```javascript
if (process.env.NODE_ENV === 'development') {
  //no need to hash output in development!
  app.output.filename = '[name].js';
}
```

### Set up Manifest Generation
When running in production that bundle name is going to change pretty regularly and it's a pain to manually update script tags whenever that happens.  Instead, we can use the `assets-webpack-plugin` plugin to automatically generate a manifest that we can use later to automatically update.

We're also going to install the `clean-webpack-plugin` to tidy up some of the mess we'll create with every build.

1. `npm install --save-dev assets-webpack-plugin clean-webpack-plugin`
2. In `webpack.config.js`, add the 2 new plugins
```javascript
const AssetsPlugin = require('assets-webpack-plugin');
const CleanPlugin  = require('clean-webpack-plugin');

const app = {
  //...
  plugins: [
    new AssetsPlugin({
      fileName: 'webpack.assets.json',
      path: outputPath
    }),
    new CleanPlugin([
      path.resolve(outputPath, '*.js'),
      path.resolve(outputPath, '*.js.map')
    ])
  ]
```
This is configuring 2 things:
* Output asset information to `~/scripts/webpack.assets.json`
* Delete any existing `*.js` or `*.js.map` that may have been created by previous builds

Now webpack will output everything we need to embed the resources in the ASP.NET MVC project

### Configure Webpack.NET
First off, install the Webpack.NET nuget package
```powershell
Install-Package Webpack.NET
```
Now we need to tell Webpack.NET where to look for the webpack assets & manifest.  We do this by calling `ConfigureWebpack` in `Global.asax.cs`

```csharp
using Webpack.NET;
public class MvcApplication : System.Web.HttpApplication
{
  protected void Application_Start()
  {
    //...normal MVC setup
    this.ConfigureWebpack(new WebpackConfig
    {
      AssetManifestPath = "~/scripts/webpack-assets.json",
      AssetOutputPath = "~/scripts"
    });
  }
}
```
Both paths are server-relative and will be resolved to absolute paths at runtime.

### Embed Assets with UrlHelper
Now you can use extension methods on `UrlHelper` to get a link to the webpack assets.  Two extensions are available: one to generate a relative and one to generate an absolute URL:

```razor
<script src="@Url.WebpackAsset("app")"></script>
<script src="@Url.AbsoluteWebpackAsset("app")"></script>
```

### Publishing the Project
All of the above will work fine in development but at some point we need to push this live.  We don't want to include compiled assets in our source control so we need a way to get everything building as part of an MsBuild Publish.

To do this we are going to:
1. Add all generated assets to `.gitignore` to exclude them from source control (including `webpack.assets.json`)
2. Include `webpack.assets.json` in the project, even though it won't be in source control.  Provided that it _is_ present by the time we publish then it will be included in the published content
3. Set up a pre-build event (release only) to `npm install --only=dev` and `webpack -p` to generate our compiled output
4. Change the `.csproj` to include the generated assets

Steps 1 & 2 are self-explanatory, so let's jump straight to...

#### 3. Pre-Build event: Release
```cmd
if "$(ConfigurationName)" == "Release" (
  cd $(ProjectDir)
  call npm install --only=dev
  call node_modules/.bin/webpack -p
)
```
i.e.
1. if we are not in Release mode, do nothing
2. go to the project directory
3. install all our dev dependencies
4. run webpack (one of our dev dependencies) in production mode

This will generate all assets and the manifest ready for us to use.

### 4. Include Assets in Publish
By adding the `webpack.assets.json` file to the project we automatically include it in the published content.  The generated bundle files are trickier though: they will always have a different name so we can't just add them to the project file.

To get around this we manually add the following `BeforeBuild` target to the `.csproj` file to include any generated `.js` files into the project.
```xml
<Target Name="BeforeBuild">
  <ItemGroup>
    <Content Include="Scripts\*.js" />
  </ItemGroup>
</Target>
```
