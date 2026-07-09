const fs = require('fs');
let xaml = fs.readFileSync('Community/src/DesktopCommandCenter.UI/Views/AppsWorkspacesPage.xaml', 'utf8');

// Also add helpers namespace if missing
if (!xaml.includes('xmlns:helpers="using:DesktopCommandCenter.UI.Helpers"')) {
    xaml = xaml.replace(/xmlns:controls="using:DesktopCommandCenter.UI.Controls"/, 'xmlns:controls="using:DesktopCommandCenter.UI.Controls"\n    xmlns:helpers="using:DesktopCommandCenter.UI.Helpers"');
}

xaml = xaml.replace(/\{Binding Source=\{x:Null\}, Converter=\{StaticResource TranslateConverter\}, ConverterParameter='([^']+)'\}/g, '{x:Bind helpers:LocalizationHelper.GetLocalized(\'$1\')}');

// Specific fix for the New Workspace button
xaml = xaml.replace(/GetLocalized\('Apps_NewWorkspace'\)/, 'GetLocalized(\'Apps_CreateWorkspace\')');

fs.writeFileSync('Community/src/DesktopCommandCenter.UI/Views/AppsWorkspacesPage.xaml', xaml);
console.log('Replaced all TranslateConverters in AppsWorkspacesPage.xaml');
