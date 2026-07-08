const fs = require('fs');

const items = [
    { key: "Settings_Home", pt: "Configurações", en: "Settings", es: "Configuraciones", ptd: "Início", end: "Home", esd: "Inicio" },
    { key: "Settings_System", pt: "Sistema", en: "System", es: "Sistema", ptd: "Configurações de Sistema", end: "System Settings", esd: "Configuraciones del Sistema" },
    { key: "Settings_Bluetooth", pt: "Bluetooth e dispositivos", en: "Bluetooth & devices", es: "Bluetooth y dispositivos", ptd: "Configurações de Bluetooth", end: "Bluetooth Settings", esd: "Configuraciones de Bluetooth" },
    { key: "Settings_Network", pt: "Rede e Internet", en: "Network & internet", es: "Red e internet", ptd: "Configurações de Rede", end: "Network Settings", esd: "Configuraciones de Red" },
    { key: "Settings_Personalization", pt: "Personalização", en: "Personalization", es: "Personalización", ptd: "Configurações de Personalização", end: "Personalization Settings", esd: "Configuraciones de Personalización" },
    { key: "Settings_Apps", pt: "Aplicativos", en: "Apps", es: "Aplicaciones", ptd: "Configurações de Aplicativos", end: "Apps Settings", esd: "Configuraciones de Aplicaciones" },
    { key: "Settings_Accounts", pt: "Contas", en: "Accounts", es: "Cuentas", ptd: "Configurações de Contas", end: "Accounts Settings", esd: "Configuraciones de Cuentas" },
    { key: "Settings_TimeLang", pt: "Hora e idioma", en: "Time & language", es: "Hora e idioma", ptd: "Configurações de Hora e Idioma", end: "Time and Language Settings", esd: "Configuraciones de Hora e Idioma" },
    { key: "Settings_Gaming", pt: "Jogos", en: "Gaming", es: "Juegos", ptd: "Configurações de Jogos", end: "Gaming Settings", esd: "Configuraciones de Juegos" },
    { key: "Settings_Accessibility", pt: "Acessibilidade", en: "Accessibility", es: "Accesibilidad", ptd: "Configurações de Acessibilidade", end: "Accessibility Settings", esd: "Configuraciones de Accesibilidad" },
    { key: "Settings_Privacy", pt: "Privacidade e segurança", en: "Privacy & security", es: "Privacidad y seguridad", ptd: "Configurações de Privacidade", end: "Privacy Settings", esd: "Configuraciones de Privacidad" },
    { key: "Settings_WindowsUpdate", pt: "Windows Update", en: "Windows Update", es: "Windows Update", ptd: "Configurações do Windows Update", end: "Windows Update Settings", esd: "Configuraciones de Windows Update" }
];

const keysToAdd = {};
for (const item of items) {
    keysToAdd[item.key + "_Title"] = { 'pt-BR': item.pt, 'en-US': item.en, 'es-ES': item.es };
    keysToAdd[item.key + "_Desc"] = { 'pt-BR': item.ptd, 'en-US': item.end, 'es-ES': item.esd };
}

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
