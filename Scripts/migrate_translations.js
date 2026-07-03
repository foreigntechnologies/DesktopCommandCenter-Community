const fs = require('fs');
const path = require('path');

const viewsDir = path.join('C:', 'Users', 'kogli', 'Desktop', 'Projetos', 'Windows', 'DesktopCommandCenter-Enterprise', 'Community', 'src', 'DesktopCommandCenter.UI', 'Views');

function getAllXamlFiles(dir) {
    const results = [];
    for (const file of fs.readdirSync(dir)) {
        const fullPath = path.join(dir, file);
        if (fs.statSync(fullPath).isDirectory()) {
            results.push(...getAllXamlFiles(fullPath));
        } else if (file.endsWith('.xaml') && !file.endsWith('DashboardPage.xaml') && !file.endsWith('SettingsPage.xaml')) {
            results.push(fullPath);
        }
    }
    return results;
}

const xamlFiles = getAllXamlFiles(viewsDir);

for (const xamlFile of xamlFiles) {
    let xamlContent = fs.readFileSync(xamlFile, 'utf8');
    const csFile = xamlFile + '.cs';
    let csContent = fs.existsSync(csFile) ? fs.readFileSync(csFile, 'utf8') : '';

    if (!xamlContent.includes('helpers:Translate.Key')) continue;

    console.log(`Processing ${path.basename(xamlFile)}...`);

    const translationMappings = []; // { xName, key, type }

    // Regex to match XML tags with helpers:Translate.Key
    const tagRegex = /<([a-zA-Z0-9_:\.]+)([^>]*?)helpers:Translate\.Key="([^"]+)"([^>]*?)(\/?)>/g;
    
    xamlContent = xamlContent.replace(tagRegex, (match, tagName, beforeAttrs, keyName, afterAttrs, selfClosing) => {
        let newAttrs = beforeAttrs + afterAttrs;
        let xName = '';
        
        // Check if it already has x:Name
        const nameMatch = newAttrs.match(/x:Name="([^"]+)"/);
        if (nameMatch) {
            xName = nameMatch[1];
        } else {
            // Generate a safe x:Name
            xName = keyName.replace(/[^a-zA-Z0-9]/g, '') + 'Element';
            // Insert x:Name right after the tag name
            newAttrs = ` x:Name="${xName}"` + newAttrs;
        }

        // Clean up multiple spaces
        newAttrs = newAttrs.replace(/\s+/g, ' ');

        translationMappings.push({ xName, key: keyName, type: tagName });

        return `<${tagName}${newAttrs}${selfClosing ? ' />' : '>'}`;
    });

    // Remove xmlns:helpers if not needed
    xamlContent = xamlContent.replace(/\s*xmlns:helpers="using:DesktopCommandCenter\.UI\.Helpers"/, '');

    fs.writeFileSync(xamlFile, xamlContent, 'utf8');

    // Now update code-behind
    if (csContent && translationMappings.length > 0) {
        // Generate UpdateTranslations method body
        let updateLines = [];
        for (const mapping of translationMappings) {
            let line = '';
            if (mapping.type === 'TextBlock') {
                line = `${mapping.xName}.Text = Helpers.LocalizationHelper.Instance.GetString("${mapping.key}");`;
            } else if (mapping.type === 'Button' || mapping.type === 'ToggleButton' || mapping.type === 'HyperlinkButton') {
                line = `if (${mapping.xName}.Content is string || ${mapping.xName}.Content == null) ${mapping.xName}.Content = Helpers.LocalizationHelper.Instance.GetString("${mapping.key}");`;
            } else if (mapping.type === 'ToolTip') {
                line = `${mapping.xName}.Content = Helpers.LocalizationHelper.Instance.GetString("${mapping.key}");`;
            } else if (mapping.type === 'ContentDialog') {
                if (mapping.key.endsWith('_PrimaryBtn')) {
                    line = `${mapping.xName}.PrimaryButtonText = Helpers.LocalizationHelper.Instance.GetString("${mapping.key}");`;
                } else if (mapping.key.endsWith('_CloseBtn')) {
                    line = `${mapping.xName}.CloseButtonText = Helpers.LocalizationHelper.Instance.GetString("${mapping.key}");`;
                } else {
                    line = `${mapping.xName}.Title = Helpers.LocalizationHelper.Instance.GetString("${mapping.key}");`;
                }
            } else if (mapping.type === 'TextBox') {
                if (mapping.key.endsWith('_Placeholder')) {
                    line = `${mapping.xName}.PlaceholderText = Helpers.LocalizationHelper.Instance.GetString("${mapping.key}");`;
                } else {
                    line = `${mapping.xName}.Header = Helpers.LocalizationHelper.Instance.GetString("${mapping.key}");`;
                    // Assume placeholder could be there
                    line += `\n            var p_${mapping.xName} = Helpers.LocalizationHelper.Instance.GetString("${mapping.key}_Placeholder");`;
                    line += `\n            if (!string.IsNullOrEmpty(p_${mapping.xName}) && p_${mapping.xName} != "${mapping.key}_Placeholder") ${mapping.xName}.PlaceholderText = p_${mapping.xName};`;
                }
            } else if (mapping.type === 'ComboBox') {
                line = `${mapping.xName}.Header = Helpers.LocalizationHelper.Instance.GetString("${mapping.key}");`;
            } else if (mapping.type === 'ComboBoxItem') {
                line = `${mapping.xName}.Content = Helpers.LocalizationHelper.Instance.GetString("${mapping.key}");`;
            } else if (mapping.type === 'ToggleSwitch') {
                line = `${mapping.xName}.Header = Helpers.LocalizationHelper.Instance.GetString("${mapping.key}");`;
                line += `\n            var onText_${mapping.xName} = Helpers.LocalizationHelper.Instance.GetString("Toggle_On");`;
                line += `\n            if (!string.IsNullOrEmpty(onText_${mapping.xName}) && onText_${mapping.xName} != "Toggle_On") ${mapping.xName}.OnContent = onText_${mapping.xName};`;
                line += `\n            var offText_${mapping.xName} = Helpers.LocalizationHelper.Instance.GetString("Toggle_Off");`;
                line += `\n            if (!string.IsNullOrEmpty(offText_${mapping.xName}) && offText_${mapping.xName} != "Toggle_Off") ${mapping.xName}.OffContent = offText_${mapping.xName};`;
            } else {
                line = `// TODO: Implement translation for ${mapping.xName} of type ${mapping.type}`;
            }
            updateLines.push(line);
        }

        const methodCode = `
        private void UpdateTranslations()
        {
            ${updateLines.join('\n            ')}
        }`;

        // Insert method before the last closing brace
        const lastBraceIndex = csContent.lastIndexOf('}');
        if (lastBraceIndex !== -1) {
            // Ensure we don't duplicate
            if (!csContent.includes('private void UpdateTranslations()')) {
                csContent = csContent.substring(0, lastBraceIndex) + methodCode + '\n' + csContent.substring(lastBraceIndex);
            }
        }

        // Add call in constructor
        const constructorRegex = /(public\s+[a-zA-Z0-9_]+\s*\([^)]*\)\s*{)/;
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
