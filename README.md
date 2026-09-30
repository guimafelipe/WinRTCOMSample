# WinRTCOMSample

This branch uses single-project MSIX packaging instead of a Windows Application Packaging project.
The self-contained `WinRTCOMServer` is an independent deployable under its own package subfolder:

```xml
<ProjectReference Include="..\WinRTCOMServer\WinRTCOMServer.csproj">
  <PackageSubfolder>WinRTCOMServer</PackageSubfolder>
</ProjectReference>
```

The package manifest registers the server at `WinRTCOMServer\WinRTCOMServer.exe`. The contract WinMD
remains at the package root so the WinUI client can generate and use the CsWinRT projection.

> [!NOTE]
> `PackageSubfolder` is currently under review in Microsoft.Windows.SDK.BuildTools.MSIX. The package
> version in this branch identifies the validation build and must be updated to the first published
> version containing that feature.

Build the WinUI project directly:

```powershell
msbuild .\WinRTCOMSample\WinRTCOMSample\WinRTCOMSample.csproj `
  /restore /t:Publish /p:Configuration=Release /p:Platform=x64 `
  /p:GenerateAppxPackageOnBuild=true
```