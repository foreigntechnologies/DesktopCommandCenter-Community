const fs = require('fs');
const path = require('path');
const resourcesDir = path.join('C:', 'Users', 'kogli', 'Desktop', 'Projetos', 'Windows', 'DesktopCommandCenter-Enterprise', 'Community', 'src', 'DesktopCommandCenter.UI', 'Resources');

const translations = {
    "pt-BR": {
        "GreetingMorning": "Bom dia",
        "GreetingAfternoon": "Boa tarde",
        "GreetingEvening": "Boa noite",
        "Dash_ProBadge": "Desbloqueie ferramentas exclusivas",
        "Dialog_LoginRequired_Title": "Requer Login",
        "Dialog_LoginRequired_Content": "Você precisa fazer login no aplicativo antes de prosseguir com a assinatura para vincular seu perfil à licença.",
        "Dialog_GotIt": "Entendi"
    },
    "en-US": {
        "GreetingMorning": "Good morning",
        "GreetingAfternoon": "Good afternoon",
        "GreetingEvening": "Good evening",
        "Dash_ProBadge": "Unlock exclusive tools",
        "Dialog_LoginRequired_Title": "Login Required",
        "Dialog_LoginRequired_Content": "You need to log into the application before proceeding with the subscription to link your profile to the license.",
        "Dialog_GotIt": "Got it"
    },
    "es-ES": {
        "GreetingMorning": "Buenos días",
        "GreetingAfternoon": "Buenas tardes",
        "GreetingEvening": "Buenas noches",
        "Dash_ProBadge": "Desbloquea herramientas exclusivas",
        "Dialog_LoginRequired_Title": "Inicio de Sesión Requerido",
        "Dialog_LoginRequired_Content": "Debes iniciar sesión en la aplicación antes de continuar con la suscripción para vincular tu perfil a la licencia.",
        "Dialog_GotIt": "Entendido"
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
console.log('Dashboard Dynamic JSON updated');
