const fs = require('fs');
const path = require('path');

const uiDir = path.join('C:', 'Users', 'kogli', 'Desktop', 'Projetos', 'Windows', 'DesktopCommandCenter-Enterprise', 'Community', 'src', 'DesktopCommandCenter.UI');
const xamlFiles = [
    path.join(uiDir, 'MainPage.xaml'),
    path.join(uiDir, 'MainWindow.xaml')
];

for (const xamlFile of xamlFiles) {
    if (!fs.existsSync(xamlFile)) continue;
    let xamlContent = fs.readFileSync(xamlFile, 'utf8');
    const csFile = xamlFile + '.cs';
    let csContent = fs.existsSync(csFile) ? fs.readFileSync(csFile, 'utf8') : '';

    if (!xamlContent.includes('helpers:Translate.Key')) continue;

    console.log(`Processing ${path.basename(xamlFile)}...`);

    const translationMappings = []; // { xName, key, type }

    const tagRegex = /<([a-zA-Z0-9_:\.]+)([^>]*?)helpers:Translate\.Key="([^"]+)"([^>]*?)(\/?)>/g;
    
    xamlContent = xamlContent.replace(tagRegex, (match, tagName, beforeAttrs, keyName, afterAttrs, selfClosing) => {
        let newAttrs = beforeAttrs + afterAttrs;
        let xName = '';
        
        const nameMatch = newAttrs.match(/x:Name="([^"]+)"/);
        if (nameMatch) {
            xName = nameMatch[1];
        } else {
            xName = keyName.replace(/[^a-zA-Z0-9]/g, '') + 'Element';
            newAttrs = ` x:Name="${xName}"` + newAttrs;
        }

        newAttrs = newAttrs.replace(/\s+/g, ' ');

        translationMappings.push({ xName, key: keyName, type: tagName });

        return `<${tagName}${newAttrs}${selfClosing ? ' />' : '>'}`;
    });

    xamlContent = xamlContent.replace(/\s*xmlns:helpers="using:DesktopCommandCenter\.UI\.Helpers"/, '');

    fs.writeFileSync(xamlFile, xamlContent, 'utf8');

    if (csContent && translationMappings.length > 0) {
        let updateLines = [];
        for (const mapping of translationMappings) {
            let line = '';
            if (mapping.type === 'TextBlock' || mapping.type.includes('ItemHeader')) {
                line = `${mapping.xName}.Text = Helpers.LocalizationHelper.Instance.GetString("${mapping.key}");`;
            } else if (mapping.type === 'Button' || mapping.type === 'ToggleButton' || mapping.type === 'HyperlinkButton') {
                line = `if (${mapping.xName}.Content is string || ${mapping.xName}.Content == null) ${mapping.xName}.Content = Helpers.LocalizationHelper.Instance.GetString("${mapping.key}");`;
            } else if (mapping.type === 'ToolTip') {
                line = `${mapping.xName}.Content = Helpers.LocalizationHelper.Instance.GetString("${mapping.key}");`;
            } else if (mapping.type === 'NavigationViewItem') {
                line = `${mapping.xName}.Content = Helpers.LocalizationHelper.Instance.GetString("${mapping.key}");`;
            } else if (mapping.type === 'MenuFlyoutItem') {
                line = `${mapping.xName}.Text = Helpers.LocalizationHelper.Instance.GetString("${mapping.key}");`;
            } else {
                line = `// TODO: Implement translation for ${mapping.xName} of type ${mapping.type} for key ${mapping.key}`;
                // Fallback to text/content based on common naming
                line += `\n            try { ((dynamic)${mapping.xName}).Content = Helpers.LocalizationHelper.Instance.GetString("${mapping.key}"); } catch {}`;
                line += `\n            try { ((dynamic)${mapping.xName}).Text = Helpers.LocalizationHelper.Instance.GetString("${mapping.key}"); } catch {}`;
            }
            updateLines.push(line);
        }

        const methodCode = `
        private void UpdateTranslations()
        {
            ${updateLines.join('\n            ')}
        }`;

        const lastBraceIndex = csContent.lastIndexOf('}');
        if (lastBraceIndex !== -1) {
            if (!csContent.includes('private void UpdateTranslations()')) {
                csContent = csContent.substring(0, lastBraceIndex) + methodCode + '\n' + csContent.substring(lastBraceIndex);
            }
        }

        const constructorRegex = /(public\s+[a-zA-Z0-9_]+\s*\([^)]*\)\s*(?::\s*base\([^)]*\)\s*)?{)/;
        const match = constructorRegex.exec(csContent);
        if (match && !csContent.includes('UpdateTranslations();')) {
            const insertIndex = match.index + match[0].length;
            const initCall = `
            UpdateTranslations();
            Helpers.LocalizationHelper.Instance.PropertyChanged += (s, e) => UpdateTranslations();`;
            csContent = csContent.substring(0, insertIndex) + initCall + csContent.substring(insertIndex);
        }

        fs.writeFileSync(csFile, csContent, 'utf8');
    }
}
console.log('Done!');
