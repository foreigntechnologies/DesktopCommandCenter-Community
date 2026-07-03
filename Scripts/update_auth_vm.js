const fs = require('fs');
const path = require('path');

const resourcesDir = path.join('C:', 'Users', 'kogli', 'Desktop', 'Projetos', 'Windows', 'DesktopCommandCenter-Enterprise', 'Community', 'src', 'DesktopCommandCenter.UI', 'Resources');

const translations = {
    "pt-BR": {
        "Auth_PlanProActive": "✔ Plano PRO ativo",
        "Auth_PlanPaused": "⏸ Plano Pausado",
        "Auth_PlanCommunity": "Plano Community (Gratuito)",
        "Auth_LinkSuccessGoogle": "Conta do Google vinculada com sucesso!",
        "Auth_LinkSuccessGitHub": "Conta do GitHub vinculada com sucesso!",
        "Auth_LinkSuccessMicrosoft": "Conta da Microsoft vinculada com sucesso!",
        "Auth_LinkError": "Erro ao vincular: ",
        "Auth_ProcessInterrupted": "Processo interrompido.",
        "Auth_WaitingPayment": "Aguardando confirmação de pagamento... (Finalize no navegador)",
        "Auth_WaitingSubChange": "Aguardando alterações na assinatura... (Finalize no navegador)",
        "Auth_PlanUpdated": "Plano atualizado com sucesso!",
        "Auth_VerificationInterrupted": "Verificação interrompida."
    },
    "en-US": {
        "Auth_PlanProActive": "✔ PRO Plan active",
        "Auth_PlanPaused": "⏸ Plan Paused",
        "Auth_PlanCommunity": "Community Plan (Free)",
        "Auth_LinkSuccessGoogle": "Google account linked successfully!",
        "Auth_LinkSuccessGitHub": "GitHub account linked successfully!",
        "Auth_LinkSuccessMicrosoft": "Microsoft account linked successfully!",
        "Auth_LinkError": "Error linking account: ",
        "Auth_ProcessInterrupted": "Process interrupted.",
        "Auth_WaitingPayment": "Waiting for payment confirmation... (Finish in browser)",
        "Auth_WaitingSubChange": "Waiting for subscription changes... (Finish in browser)",
        "Auth_PlanUpdated": "Plan updated successfully!",
        "Auth_VerificationInterrupted": "Verification interrupted."
    },
    "es-ES": {
        "Auth_PlanProActive": "✔ Plan PRO activo",
        "Auth_PlanPaused": "⏸ Plan Pausado",
        "Auth_PlanCommunity": "Plan Community (Gratis)",
        "Auth_LinkSuccessGoogle": "¡Cuenta de Google vinculada con éxito!",
        "Auth_LinkSuccessGitHub": "¡Cuenta de GitHub vinculada con éxito!",
        "Auth_LinkSuccessMicrosoft": "¡Cuenta de Microsoft vinculada con éxito!",
        "Auth_LinkError": "Error al vincular: ",
        "Auth_ProcessInterrupted": "Proceso interrumpido.",
        "Auth_WaitingPayment": "Esperando confirmación de pago... (Finaliza en el navegador)",
        "Auth_WaitingSubChange": "Esperando cambios en la suscripción... (Finaliza en el navegador)",
        "Auth_PlanUpdated": "¡Plan actualizado con éxito!",
        "Auth_VerificationInterrupted": "Verificación interrumpida."
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
