const fs = require('fs');
const path = require('path');
const resourcesDir = path.join('C:', 'Users', 'kogli', 'Desktop', 'Projetos', 'Windows', 'DesktopCommandCenter-Enterprise', 'Community', 'src', 'DesktopCommandCenter.UI', 'Resources');

const translations = {
    "pt-BR": {
        "Awake_StatusDisabled": "O PC pode dormir normalmente baseado nas configurações do Windows.",
        "Awake_StatusEnabled": "Awake está ATIVO. Sua tela e o PC não vão desligar nem dormir."
    },
    "en-US": {
        "Awake_StatusDisabled": "The PC can sleep normally based on Windows settings.",
        "Awake_StatusEnabled": "Awake is ACTIVE. Your screen and PC will not turn off or sleep."
    },
    "es-ES": {
        "Awake_StatusDisabled": "El PC puede suspenderse normalmente según la configuración de Windows.",
        "Awake_StatusEnabled": "Awake está ACTIVO. Tu pantalla y PC no se apagarán ni suspenderán."
    }
};

for (const [lang, newKeys] of Object.entries(translations)) {
    const filePath = path.join(resourcesDir, lang + '.json');
    if (fs.existsSync(filePath)) {
        let content = fs.readFileSync(filePath, 'utf8');
        let dict = JSON.parse(content);
        dict = { ...dict, ...newKeys };
        fs.writeFileSync(filePath, JSON.stringify(dict, null, 2), 'utf8');
    }
}
console.log('Awake JSON updated');
