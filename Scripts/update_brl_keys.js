const fs = require('fs');

const keysToUpdate = {
    'Auth_ProYearlySubtitle': { 'pt-BR': 'R$ 429,90 BRL / ano', 'en-US': 'R$ 429.90 BRL / year', 'es-ES': 'R$ 429,90 BRL / año' },
    'Auth_ProMonthlySubtitle': { 'pt-BR': 'R$ 39,90 BRL / mês · Cancele quando quiser', 'en-US': 'R$ 39.90 BRL / month · Cancel anytime', 'es-ES': 'R$ 39,90 BRL / mes · Cancela cuando quieras' }
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
    
    for (const key in keysToUpdate) {
        data[key] = keysToUpdate[key][lang];
    }
    
    fs.writeFileSync(filePath, JSON.stringify(data, null, 2), 'utf8');
}
