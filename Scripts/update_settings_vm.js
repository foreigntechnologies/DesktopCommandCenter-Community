const fs = require('fs');
const path = require('path');
const resourcesDir = path.join('C:', 'Users', 'kogli', 'Desktop', 'Projetos', 'Windows', 'DesktopCommandCenter-Enterprise', 'Community', 'src', 'DesktopCommandCenter.UI', 'Resources');

const translations = {
    "pt-BR": {
        "Settings_ModelLocal": "Ex: llama3.1",
        "Settings_ModelGPT4": "Ex: gpt-4o",
        "Settings_ModelGemini": "Ex: gemini-1.5-pro",
        "Settings_ModelClaude": "Ex: claude-3-5-sonnet",
        "Settings_ModelDefault": "Insira o nome do modelo",
        "Settings_PlanProActive": "Plano PRO Ativo",
        "Settings_PlanCommunity": "Versão Community"
    },
    "en-US": {
        "Settings_ModelLocal": "Ex: llama3.1",
        "Settings_ModelGPT4": "Ex: gpt-4o",
        "Settings_ModelGemini": "Ex: gemini-1.5-pro",
        "Settings_ModelClaude": "Ex: claude-3-5-sonnet",
        "Settings_ModelDefault": "Enter the model name",
        "Settings_PlanProActive": "PRO Plan Active",
        "Settings_PlanCommunity": "Community Version"
    },
    "es-ES": {
        "Settings_ModelLocal": "Ej: llama3.1",
        "Settings_ModelGPT4": "Ej: gpt-4o",
        "Settings_ModelGemini": "Ej: gemini-1.5-pro",
        "Settings_ModelClaude": "Ej: claude-3-5-sonnet",
        "Settings_ModelDefault": "Introduce el nombre del modelo",
        "Settings_PlanProActive": "Plan PRO Activo",
        "Settings_PlanCommunity": "Versión Community"
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
console.log('Settings VM JSON updated');
