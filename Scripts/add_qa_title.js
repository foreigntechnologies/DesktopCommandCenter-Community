const fs = require('fs');
const keysToAdd = {
    'QuickAccessTitleElement': { 'pt-BR': 'DCC - Acesso Rápido', 'en-US': 'DCC - Quick Access', 'es-ES': 'DCC - Acceso Rápido' }
};

for (const lang of ['pt-BR', 'en-US', 'es-ES']) {
    const filePath = `Resources/${lang}.json`;
    if (!fs.existsSync(filePath)) continue;
    
    let content = fs.readFileSync(filePath, 'utf8');
    let data;
    try {
        data = JSON.parse(content);
    } catch(e) {
        content = content.replace(/^\uFEFF/, '');
        data = JSON.parse(content);
    }
    
    let modified = false;
    for (const key in keysToAdd) {
        if (!data[key]) {
            data[key] = keysToAdd[key][lang];
            modified = true;
        }
    }
    
    if (modified) {
        fs.writeFileSync(filePath, JSON.stringify(data, null, 2), 'utf8');
    }
}
