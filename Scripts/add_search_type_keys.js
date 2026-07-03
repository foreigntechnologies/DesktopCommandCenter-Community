const fs = require('fs');

const keysToAdd = {
    'Search_TypeSetting': { 'pt-BR': 'Configuração', 'en-US': 'Setting', 'es-ES': 'Configuración' },
    'Search_TypeFile': { 'pt-BR': 'Arquivo', 'en-US': 'File', 'es-ES': 'Archivo' },
    'Search_TypeMath': { 'pt-BR': 'Cálculo', 'en-US': 'Calculation', 'es-ES': 'Cálculo' },
    'Search_TypeTerminal': { 'pt-BR': 'Terminal', 'en-US': 'Terminal', 'es-ES': 'Terminal' },
    'Search_TypeApp': { 'pt-BR': 'Aplicativo', 'en-US': 'App', 'es-ES': 'Aplicación' }
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
