const fs = require('fs');
const path = require('path');
const resourcesDir = path.join('C:', 'Users', 'kogli', 'Desktop', 'Projetos', 'Windows', 'DesktopCommandCenter-Enterprise', 'Community', 'src', 'DesktopCommandCenter.UI', 'Resources');

const translations = {
    "pt-BR": {
        "Auto_Trigger_1": "Ao copiar um link do YouTube",
        "Auto_Trigger_2": "Ao tirar um printscreen",
        "Auto_Trigger_3": "Ao plugar Pendrive",
        "Auto_Trigger_4": "A cada X minutos/horas",
        "Auto_Trigger_5": "Ao ligar o computador",
        "Auto_Trigger_6": "Ao abrir um aplicativo específico",
        "Auto_Action_1": "Abrir programa",
        "Auto_Action_2": "Executar script personalizado",
        "Auto_Action_3": "Extrair ID do vídeo",
        "Auto_Action_4": "Extrair texto de imagem via OCR",
        "Auto_Action_5": "Falar texto (Text-to-Speech)",
        "Auto_Action_6": "Executar script PowerShell ou CMD",
        "Auto_Action_7": "Limpar Área de Transferência",
        "Auto_Action_8": "Exibir notificação do sistema (Toast)",
        "Auto_DefRule_ActionParam": "Novo dispositivo USB conectado!"
    },
    "en-US": {
        "Auto_Trigger_1": "When copying a YouTube link",
        "Auto_Trigger_2": "When taking a screenshot",
        "Auto_Trigger_3": "When plugging in a USB drive",
        "Auto_Trigger_4": "Every X minutes/hours",
        "Auto_Trigger_5": "When turning on the computer",
        "Auto_Trigger_6": "When opening a specific application",
        "Auto_Action_1": "Open program",
        "Auto_Action_2": "Run custom script",
        "Auto_Action_3": "Extract video ID",
        "Auto_Action_4": "Extract text from image via OCR",
        "Auto_Action_5": "Speak text (Text-to-Speech)",
        "Auto_Action_6": "Run PowerShell or CMD script",
        "Auto_Action_7": "Clear Clipboard",
        "Auto_Action_8": "Show system notification (Toast)",
        "Auto_DefRule_ActionParam": "New USB device connected!"
    },
    "es-ES": {
        "Auto_Trigger_1": "Al copiar un enlace de YouTube",
        "Auto_Trigger_2": "Al tomar una captura de pantalla",
        "Auto_Trigger_3": "Al conectar una unidad USB",
        "Auto_Trigger_4": "Cada X minutos/horas",
        "Auto_Trigger_5": "Al encender la computadora",
        "Auto_Trigger_6": "Al abrir una aplicación específica",
        "Auto_Action_1": "Abrir programa",
        "Auto_Action_2": "Ejecutar script personalizado",
        "Auto_Action_3": "Extraer ID del video",
        "Auto_Action_4": "Extraer texto de la imagen vía OCR",
        "Auto_Action_5": "Hablar texto (Text-to-Speech)",
        "Auto_Action_6": "Ejecutar script de PowerShell o CMD",
        "Auto_Action_7": "Limpiar Portapapeles",
        "Auto_Action_8": "Mostrar notificación del sistema (Toast)",
        "Auto_DefRule_ActionParam": "¡Nuevo dispositivo USB conectado!"
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
console.log('Automacoes JSON updated');
