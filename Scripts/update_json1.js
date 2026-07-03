const fs = require('fs');
const path = require('path');

const resourcesDir = path.join('C:', 'Users', 'kogli', 'Desktop', 'Projetos', 'Windows', 'DesktopCommandCenter-Enterprise', 'Community', 'src', 'DesktopCommandCenter.UI', 'Resources');

const translations = {
    "pt-BR": {
        "AlwaysOnTop_StatusEnabled": "Always On Top está ATIVO. A janela ficará flutuando por cima das outras.",
        "AlwaysOnTop_StatusDisabled": "A janela se comporta normalmente, ficando atrás de outras janelas.",
        "Auth_LicenseCloud": "Licenciamento & Nuvem",
        "Auth_OfflineDesc": "O DCC funciona 100% offline e gratuito. Faça login apenas para desbloquear os recursos PRO.",
        "Auth_Processing": "Processando requisição... (se você fechou o navegador, clique em Cancelar)",
        "Auth_Cancel": "Cancelar",
        "Auth_ContinueGoogle": "Continuar com o Google",
        "Auth_ContinueGitHub": "Continuar com o GitHub",
        "Auth_LinkAccounts": "Vincular contas adicionais:",
        "Auth_LinkGoogle": "Vincular Google",
        "Auth_LinkGitHub": "Vincular GitHub",
        "Auth_LinkDesc": "Ao vincular, você poderá entrar na sua conta com qualquer um dos provedores associados.",
        "Auth_ProActiveTitle": "Plano PRO Ativo!",
        "Auth_ProActiveDesc": "Você tem acesso completo a todos os recursos exclusivos.",
        "Auth_ManageSub": "Gerenciar ou Cancelar Assinatura",
        "Auth_ProPausedTitle": "Plano Pausado",
        "Auth_ProPausedDesc": "Sua assinatura está pausada. Retome-a no portal para voltar a ter acesso PRO.",
        "Auth_ManagePausedSub": "Gerenciar Assinatura",
        "Auth_Logout": "Fazer Logout",
        "Auth_SubAccount": "Assinatura e Conta"
    },
    "en-US": {
        "AlwaysOnTop_StatusEnabled": "Always On Top is ACTIVE. The window will float above others.",
        "AlwaysOnTop_StatusDisabled": "The window behaves normally, staying behind other windows.",
        "Auth_LicenseCloud": "Licensing & Cloud",
        "Auth_OfflineDesc": "DCC is 100% offline and free. Log in only to unlock PRO features.",
        "Auth_Processing": "Processing request... (if you closed the browser, click Cancel)",
        "Auth_Cancel": "Cancel",
        "Auth_ContinueGoogle": "Continue with Google",
        "Auth_ContinueGitHub": "Continue with GitHub",
        "Auth_LinkAccounts": "Link additional accounts:",
        "Auth_LinkGoogle": "Link Google",
        "Auth_LinkGitHub": "Link GitHub",
        "Auth_LinkDesc": "By linking, you can log in to your account with any of the associated providers.",
        "Auth_ProActiveTitle": "PRO Plan Active!",
        "Auth_ProActiveDesc": "You have full access to all exclusive features.",
        "Auth_ManageSub": "Manage or Cancel Subscription",
        "Auth_ProPausedTitle": "Plan Paused",
        "Auth_ProPausedDesc": "Your subscription is paused. Resume it in the portal to regain PRO access.",
        "Auth_ManagePausedSub": "Manage Subscription",
        "Auth_Logout": "Log Out",
        "Auth_SubAccount": "Subscription and Account"
    },
    "es-ES": {
        "AlwaysOnTop_StatusEnabled": "Always On Top está ACTIVO. La ventana flotará sobre las demás.",
        "AlwaysOnTop_StatusDisabled": "La ventana se comporta normalmente, permaneciendo detrás de otras ventanas.",
        "Auth_LicenseCloud": "Licencias y Nube",
        "Auth_OfflineDesc": "DCC funciona 100% sin conexión y es gratuito. Inicia sesión solo para desbloquear funciones PRO.",
        "Auth_Processing": "Procesando solicitud... (si cerraste el navegador, haz clic en Cancelar)",
        "Auth_Cancel": "Cancelar",
        "Auth_ContinueGoogle": "Continuar con Google",
        "Auth_ContinueGitHub": "Continuar con GitHub",
        "Auth_LinkAccounts": "Vincular cuentas adicionales:",
        "Auth_LinkGoogle": "Vincular Google",
        "Auth_LinkGitHub": "Vincular GitHub",
        "Auth_LinkDesc": "Al vincular, podrás iniciar sesión en tu cuenta con cualquiera de los proveedores asociados.",
        "Auth_ProActiveTitle": "¡Plan PRO Activo!",
        "Auth_ProActiveDesc": "Tienes acceso completo a todas las funciones exclusivas.",
        "Auth_ManageSub": "Administrar o Cancelar Suscripción",
        "Auth_ProPausedTitle": "Plan Pausado",
        "Auth_ProPausedDesc": "Tu suscripción está pausada. Reanúdala en el portal para volver a tener acceso PRO.",
        "Auth_ManagePausedSub": "Administrar Suscripción",
        "Auth_Logout": "Cerrar Sesión",
        "Auth_SubAccount": "Suscripción y Cuenta"
    }
};

for (const [lang, newKeys] of Object.entries(translations)) {
    const filePath = path.join(resourcesDir, lang + '.json');
    if (fs.existsSync(filePath)) {
        let content = fs.readFileSync(filePath, 'utf8');
        let dict = JSON.parse(content);
        dict = { ...dict, ...newKeys };
        fs.writeFileSync(filePath, JSON.stringify(dict, null, 2), 'utf8');
        console.log('Updated', lang);
    }
}
