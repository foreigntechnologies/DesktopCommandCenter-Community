const fs = require('fs');
const path = require('path');
const resourcesDir = path.join('C:', 'Users', 'kogli', 'Desktop', 'Projetos', 'Windows', 'DesktopCommandCenter-Enterprise', 'Community', 'src', 'DesktopCommandCenter.UI', 'Resources');

const translations = {
    "pt-BR": {
        "Dashboard_Welcome": "Bem-vindo de volta!"
    },
    "en-US": {
        "Dashboard_Welcome": "Welcome back!"
    },
    "es-ES": {
        "Dashboard_Welcome": "¡Bienvenido de nuevo!"
    }
};

for (const [lang, newKeys] of Object.entries(translations)) {
    const filePath = path.join(resourcesDir, lang + '.json');
    if (fs.existsSync(filePath)) {
        let dict = JSON.parse(fs.readFileSync(filePath, 'utf8'));
        dict = { ...dict, ...newKeys };
        fs.writeFileSync(filePath, JSON.stringify(dict, null, 2), 'utf8');
    }
}
console.log('Dashboard Welcome JSON updated');
