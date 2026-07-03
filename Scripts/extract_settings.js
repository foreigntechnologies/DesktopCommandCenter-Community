const fs = require('fs');
const path = require('path');
const file = path.join('C:', 'Users', 'kogli', 'Desktop', 'Projetos', 'Windows', 'DesktopCommandCenter-Enterprise', 'Community', 'src', 'DesktopCommandCenter.UI', 'Views', 'SettingsPage.xaml');

if (fs.existsSync(file)) {
    const content = fs.readFileSync(file, 'utf8');
    const matches = content.match(/(?:Text|Header|Content|PlaceholderText|Title)="([^"{}]*?[a-zA-ZáàãâéêíóôõúçÁÀÃÂÉÊÍÓÔÕÚÇ]+[^"{}]*?)"/g);
    
    if (matches) {
        const unique = [...new Set(matches)];
        console.log("Settings strings found:");
        unique.forEach(m => console.log(m));
    } else {
        console.log("No strings found.");
    }
}
