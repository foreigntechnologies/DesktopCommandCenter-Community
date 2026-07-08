const fs = require('fs');
const path = require('path');
const resourcesDir = path.join('C:', 'Users', 'kogli', 'Desktop', 'Projetos', 'Windows', 'DesktopCommandCenter-Enterprise', 'Community', 'src', 'DesktopCommandCenter.UI', 'Resources');
const xamlPath = path.join('C:', 'Users', 'kogli', 'Desktop', 'Projetos', 'Windows', 'DesktopCommandCenter-Enterprise', 'Community', 'src', 'DesktopCommandCenter.UI', 'Views', 'SettingsPage.xaml');

const strings = [
    { old: 'Configurações', key: 'Settings_Title', attr: 'Text' },
    { old: 'PRO', key: 'Settings_ProBadge', attr: 'Text' },
    { old: 'COMMUNITY', key: 'Settings_CommunityBadge', attr: 'Text' },
    { old: 'Aparência e Temas', key: 'Settings_Appearance', attr: 'Text' },
    { old: 'Tema do Aplicativo', key: 'Settings_ThemeHeader', attr: 'Header' },
    { old: 'Claro', key: 'Settings_ThemeLight', attr: 'Content' },
    { old: 'Escuro', key: 'Settings_ThemeDark', attr: 'Content' },
    { old: 'Padrão do Sistema (Contraste)', key: 'Settings_ThemeSystem', attr: 'Content' },
    { old: 'O tema Padrão do Sistema respeitará o Alto Contraste caso esteja ativado no Windows.', key: 'Settings_ThemeDesc', attr: 'Text' },
    { old: 'Idioma do Aplicativo (Requer reinício)', key: 'Settings_LangHeader', attr: 'Header' },
    { old: 'Português - Brasil', key: 'Settings_LangPtBR', attr: 'Content' },
    { old: 'English', key: 'Settings_LangEn', attr: 'Content' },
    { old: 'Español', key: 'Settings_LangEs', attr: 'Content' },
    { old: 'Formato da Hora (Início)', key: 'Settings_TimeFormatHeader', attr: 'Header' },
    { old: '24 Horas (HH:mm) - Ex: 15:30', key: 'Settings_TimeFmt24', attr: 'Content' },
    { old: '24 Horas com Segundos (HH:mm:ss) - Ex: 15:30:45', key: 'Settings_TimeFmt24s', attr: 'Content' },
    { old: '12 Horas AM/PM (hh:mm tt) - Ex: 03:30 PM', key: 'Settings_TimeFmt12', attr: 'Content' },
    { old: '12 Horas com Segundos (hh:mm:ss tt) - Ex: 03:30:45 PM', key: 'Settings_TimeFmt12s', attr: 'Content' },
    { old: 'Formato da Data (Início)', key: 'Settings_DateFormatHeader', attr: 'Header' },
    { old: 'Longo - Ex: quarta-feira, 10 de junho de 2026', key: 'Settings_DateFmtLong', attr: 'Content' },
    { old: 'Curto (dd/MM/yyyy) - Ex: 10/06/2026', key: 'Settings_DateFmtShort', attr: 'Content' },
    { old: 'ISO (yyyy-MM-dd) - Ex: 2026-06-10', key: 'Settings_DateFmtIso', attr: 'Content' },
    { old: 'Inglês - Ex: Jun 10, 2026', key: 'Settings_DateFmtEn', attr: 'Content' },
    { old: 'Teclas de Atalho Global', key: 'Settings_HotkeysTitle', attr: 'Text' },
    { old: 'Configure atalhos que funcionam de qualquer lugar do Windows.', key: 'Settings_HotkeysDesc', attr: 'Text' },
    { old: 'Inteligência Artificial (ChatFT)', key: 'Settings_AITitle', attr: 'Text' },
    { old: 'Defina as chaves de API dos provedores desejados. Deixe em branco se for usar o Ollama (Local) gratuitamente.', key: 'Settings_AIDesc', attr: 'Text' },
    { old: 'OpenAI (ChatGPT)', key: 'Settings_AIOpenAI', attr: 'Text' },
    { old: 'sk-...', key: 'Settings_AIPlaceholderSk', attr: 'PlaceholderText' },
    { old: 'Google Gemini', key: 'Settings_AIGemini', attr: 'Text' },
    { old: 'AIza...', key: 'Settings_AIPlaceholderAiza', attr: 'PlaceholderText' },
    { old: 'Anthropic Claude', key: 'Settings_AIClaude', attr: 'Text' },
    { old: 'sk-ant...', key: 'Settings_AIPlaceholderAnt', attr: 'PlaceholderText' },
    { old: 'Editar Tecla de Atalho', key: 'Settings_HotkeysDialogTitle', attr: 'Title' },
    { old: 'Configurar atalho para: [Ação]', key: 'Settings_HotkeysDialogAction', attr: 'Text' },
    { old: 'Nenhum', key: 'Settings_HotkeysNone', attr: 'Text' },
    { old: 'Pressione a nova combinação de teclas desejada (ex: Ctrl + Alt + P) no seu teclado. O diálogo capturará as teclas automaticamente. Pressione ENTER para confirmar.', key: 'Settings_HotkeysDialogInst', attr: 'Text' },
    { old: 'OK', key: 'Settings_HotkeysDialogOk', attr: 'Text' },
    { old: 'Cancelar', key: 'Settings_HotkeysDialogCancel', attr: 'Text' },
    { old: 'Limpar', key: 'Settings_HotkeysDialogClear', attr: 'Content' }
];

const translations = {
    "pt-BR": {}, "en-US": {}, "es-ES": {}
};

strings.forEach(s => {
    translations["pt-BR"][s.key] = s.old;
    // We will do a generic translation for English and Spanish for now, 
    // or manually fill them based on keys.
});

// Fill en-US
translations["en-US"] = {
    "Settings_Title": "Settings",
    "Settings_ProBadge": "PRO",
    "Settings_CommunityBadge": "COMMUNITY",
    "Settings_Appearance": "Appearance and Themes",
    "Settings_ThemeHeader": "Application Theme",
    "Settings_ThemeLight": "Light",
    "Settings_ThemeDark": "Dark",
    "Settings_ThemeSystem": "System Default (Contrast)",
    "Settings_ThemeDesc": "The System Default theme respects High Contrast if enabled on Windows.",
    "Settings_LangHeader": "Application Language (Requires restart)",
    "Settings_LangPtBR": "Português - Brasil",
    "Settings_LangEn": "English",
    "Settings_LangEs": "Español",
    "Settings_TimeFormatHeader": "Time Format (Home)",
    "Settings_TimeFmt24": "24 Hours (HH:mm)",
    "Settings_TimeFmt24s": "24 Hours with Seconds (HH:mm:ss)",
    "Settings_TimeFmt12": "12 Hours AM/PM (hh:mm tt)",
    "Settings_TimeFmt12s": "12 Hours with Seconds (hh:mm:ss tt)",
    "Settings_DateFormatHeader": "Date Format (Home)",
    "Settings_DateFmtLong": "Long - Ex: Wednesday, June 10, 2026",
    "Settings_DateFmtShort": "Short (MM/dd/yyyy)",
    "Settings_DateFmtIso": "ISO (yyyy-MM-dd)",
    "Settings_DateFmtEn": "English - Ex: Jun 10, 2026",
    "Settings_HotkeysTitle": "Global Hotkeys",
    "Settings_HotkeysDesc": "Configure shortcuts that work from anywhere in Windows.",
    "Settings_AITitle": "Artificial Intelligence (ChatFT)",
    "Settings_AIDesc": "Set the API keys of the desired providers. Leave blank to use Ollama (Local) for free.",
    "Settings_AIOpenAI": "OpenAI (ChatGPT)",
    "Settings_AIPlaceholderSk": "sk-...",
    "Settings_AIGemini": "Google Gemini",
    "Settings_AIPlaceholderAiza": "AIza...",
    "Settings_AIClaude": "Anthropic Claude",
    "Settings_AIPlaceholderAnt": "sk-ant...",
    "Settings_HotkeysDialogTitle": "Edit Hotkey",
    "Settings_HotkeysDialogAction": "Configure shortcut for: [Action]",
    "Settings_HotkeysNone": "None",
    "Settings_HotkeysDialogInst": "Press the new desired key combination (e.g., Ctrl + Alt + P) on your keyboard. The dialog will capture the keys automatically. Press ENTER to confirm.",
    "Settings_HotkeysDialogOk": "OK",
    "Settings_HotkeysDialogCancel": "Cancel",
    "Settings_HotkeysDialogClear": "Clear"
};

// Fill es-ES
translations["es-ES"] = {
    "Settings_Title": "Configuraciones",
    "Settings_ProBadge": "PRO",
    "Settings_CommunityBadge": "COMUNIDAD",
    "Settings_Appearance": "Apariencia y Temas",
    "Settings_ThemeHeader": "Tema de la Aplicación",
    "Settings_ThemeLight": "Claro",
    "Settings_ThemeDark": "Oscuro",
    "Settings_ThemeSystem": "Predeterminado del Sistema (Contraste)",
    "Settings_ThemeDesc": "El tema Predeterminado del Sistema respeta el Alto Contraste si está activado en Windows.",
    "Settings_LangHeader": "Idioma de la Aplicación (Requiere reinicio)",
    "Settings_LangPtBR": "Português - Brasil",
    "Settings_LangEn": "English",
    "Settings_LangEs": "Español",
    "Settings_TimeFormatHeader": "Formato de Hora (Inicio)",
    "Settings_TimeFmt24": "24 Horas (HH:mm)",
    "Settings_TimeFmt24s": "24 Horas con Segundos (HH:mm:ss)",
    "Settings_TimeFmt12": "12 Horas AM/PM (hh:mm tt)",
    "Settings_TimeFmt12s": "12 Horas con Segundos (hh:mm:ss tt)",
    "Settings_DateFormatHeader": "Formato de Fecha (Inicio)",
    "Settings_DateFmtLong": "Largo - Ej: miércoles, 10 de junio de 2026",
    "Settings_DateFmtShort": "Corto (dd/MM/yyyy)",
    "Settings_DateFmtIso": "ISO (yyyy-MM-dd)",
    "Settings_DateFmtEn": "Inglés - Ej: Jun 10, 2026",
    "Settings_HotkeysTitle": "Atajos Globales",
    "Settings_HotkeysDesc": "Configura atajos que funcionen desde cualquier parte de Windows.",
    "Settings_AITitle": "Inteligencia Artificial (ChatFT)",
    "Settings_AIDesc": "Establece las claves API de los proveedores deseados. Déjalo en blanco para usar Ollama (Local) gratis.",
    "Settings_AIOpenAI": "OpenAI (ChatGPT)",
    "Settings_AIPlaceholderSk": "sk-...",
    "Settings_AIGemini": "Google Gemini",
    "Settings_AIPlaceholderAiza": "AIza...",
    "Settings_AIClaude": "Anthropic Claude",
    "Settings_AIPlaceholderAnt": "sk-ant...",
    "Settings_HotkeysDialogTitle": "Editar Atajo",
    "Settings_HotkeysDialogAction": "Configurar atajo para: [Acción]",
    "Settings_HotkeysNone": "Ninguno",
    "Settings_HotkeysDialogInst": "Presione la combinación de teclas deseada (ej. Ctrl + Alt + P) en su teclado. El diálogo capturará las teclas automáticamente. Presione ENTER para confirmar.",
    "Settings_HotkeysDialogOk": "Aceptar",
    "Settings_HotkeysDialogCancel": "Cancelar",
    "Settings_HotkeysDialogClear": "Limpiar"
};

// Update JSON files
for (const [lang, newKeys] of Object.entries(translations)) {
    const jsonPath = path.join(resourcesDir, lang + '.json');
    if (fs.existsSync(jsonPath)) {
        let dict = JSON.parse(fs.readFileSync(jsonPath, 'utf8'));
        dict = { ...dict, ...newKeys };
        fs.writeFileSync(jsonPath, JSON.stringify(dict, null, 2), 'utf8');
    }
}

// Update XAML
let content = fs.readFileSync(xamlPath, 'utf8');
let changed = 0;

for (const s of strings) {
    const escaped = s.old.replace(/[.*+?^${}()|[\]\\]/g, '\\$&');
    
    // Check if already injected
    const checkPattern = new RegExp(`helpers:Translate\\.Key="${s.key}"\\s+${s.attr}="${escaped}"`);
    if (checkPattern.test(content)) { continue; }
    
    // Handle TextBlocks correctly when they don't have existing explicit Text attributes vs those that do.
    // In our regex we only matched attributes.
    const pattern = new RegExp(`(${s.attr}="${escaped}")`, 'g');
    const newContent = content.replace(pattern, `helpers:Translate.Key="${s.key}" $1`);
    if (newContent !== content) {
        content = newContent;
        changed++;
    }
}
fs.writeFileSync(xamlPath, content, 'utf8');
console.log(`Updated Settings JSON and XAML: injected ${changed} elements.`);
