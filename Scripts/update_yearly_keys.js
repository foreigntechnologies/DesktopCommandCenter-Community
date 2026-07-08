const fs = require('fs');

const keysToAdd = {
    'Auth_ProYearlySubtitle': { 'pt-BR': 'R$ 429,90 / ano', 'en-US': 'R$ 429.90 / year', 'es-ES': 'R$ 429,90 / año' },
    'Auth_ProYearlyBadge': { 'pt-BR': 'DESCONTO DE 10%', 'en-US': '10% OFF', 'es-ES': '10% DE DESCUENTO' }
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
    
    for (const key in keysToAdd) {
        data[key] = keysToAdd[key][lang];
    }
    
    fs.writeFileSync(filePath, JSON.stringify(data, null, 2), 'utf8');
}
