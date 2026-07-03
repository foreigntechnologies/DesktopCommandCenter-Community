const fs = require('fs');
const path = require('path');

const viewsDir = path.join('C:', 'Users', 'kogli', 'Desktop', 'Projetos', 'Windows', 'DesktopCommandCenter-Enterprise', 'Community', 'src', 'DesktopCommandCenter.UI', 'Views');

const xamlFiles = fs.readdirSync(viewsDir).filter(f => f.endsWith('.xaml'));

xamlFiles.forEach(file => {
    const filePath = path.join(viewsDir, file);
    let content = fs.readFileSync(filePath, 'utf8');
    
    // Check if helpers namespace exists
    if (!content.includes('xmlns:helpers="using:DesktopCommandCenter.UI.Helpers"')) {
        // Insert it in the root element (usually <Page or <Window)
        content = content.replace(/(<Page|<ContentDialog|<Window)/, '$1\n    xmlns:helpers="using:DesktopCommandCenter.UI.Helpers"');
        fs.writeFileSync(filePath, content, 'utf8');
        console.log(`Added xmlns:helpers to ${file}`);
    }
});
