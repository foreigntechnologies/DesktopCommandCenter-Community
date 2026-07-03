const fs = require('fs');
const path = require('path');

// All the XAML files with WMC0010 errors and the keys used
const viewsDir = path.join('C:', 'Users', 'kogli', 'Desktop', 'Projetos', 'Windows', 'DesktopCommandCenter-Enterprise', 'Community', 'src', 'DesktopCommandCenter.UI', 'Views');

function getAllXamlFiles(dir) {
    const results = [];
    for (const file of fs.readdirSync(dir)) {
        const fullPath = path.join(dir, file);
        if (fs.statSync(fullPath).isDirectory()) {
            results.push(...getAllXamlFiles(fullPath));
        } else if (file.endsWith('.xaml')) {
            results.push(fullPath);
        }
    }
    return results;
}

const xamlFiles = getAllXamlFiles(viewsDir);
const report = {};

for (const file of xamlFiles) {
    const content = fs.readFileSync(file, 'utf8');
    const matches = [...content.matchAll(/helpers:Translate\.Key="([^"]+)"/g)];
    if (matches.length > 0) {
        report[path.basename(file)] = matches.map(m => m[1]);
    }
}

// Print report
for (const [file, keys] of Object.entries(report)) {
    console.log(`\n${file}:`);
    for (const k of keys) console.log(`  - ${k}`);
}
