const fs = require('fs');
const path = require('path');
const resourcesDir = path.join('C:', 'Users', 'kogli', 'Desktop', 'Projetos', 'Windows', 'DesktopCommandCenter-Enterprise', 'Community', 'src', 'DesktopCommandCenter.UI', 'Resources');

const translations = {
    "pt-BR": {
        "Prompts_EditPrompt": "Editar Prompt",
        "Prompts_NewPrompt": "Novo Prompt",
        "Prompts_CategoryGeneral": "Geral",
        "Temporizador_TimeUp": "TEMPO ESGOTADO!"
    },
    "en-US": {
        "Prompts_EditPrompt": "Edit Prompt",
        "Prompts_NewPrompt": "New Prompt",
        "Prompts_CategoryGeneral": "General",
        "Temporizador_TimeUp": "TIME IS UP!"
    },
    "es-ES": {
        "Prompts_EditPrompt": "Editar Prompt",
        "Prompts_NewPrompt": "Nuevo Prompt",
        "Prompts_CategoryGeneral": "General",
        "Temporizador_TimeUp": "¡TIEMPO AGOTADO!"
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
console.log('Prompts/Temporizador JSON updated');
