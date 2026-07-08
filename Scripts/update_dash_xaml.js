const fs = require('fs');
const path = require('path');
const filePath = path.join('C:', 'Users', 'kogli', 'Desktop', 'Projetos', 'Windows', 'DesktopCommandCenter-Enterprise', 'Community', 'src', 'DesktopCommandCenter.UI', 'Views', 'DashboardPage.xaml');

const replacements = [
    { old: 'Ferramentas', key: 'Dashboard_MetricTools' },
    { old: 'Notas salvas', key: 'Dashboard_MetricNotes' },
    { old: 'No Clipboard', key: 'Dashboard_MetricClip' },
    { old: 'Plano atual', key: 'Dashboard_MetricPlan' },
    { old: 'Ferramentas da Comunidade', key: 'Dashboard_SectionComm' },
    { old: 'Recursos PRO (IA &amp; Automação)', key: 'Dashboard_SectionPro' },
    { old: 'Seletor de Cores', key: 'Dashboard_ToolColorTitle' },
    { old: 'Captura cores da tela', key: 'Dashboard_ToolColorDesc' },
    { old: 'Clipboard', key: 'Dashboard_ToolClipTitle' },
    { old: 'Histórico de cópias', key: 'Dashboard_ToolClipDesc' },
    { old: 'Notas', key: 'Dashboard_ToolNotesTitle' },
    { old: 'Anotações rápidas', key: 'Dashboard_ToolNotesDesc' },
    { old: 'Captura de Tela', key: 'Dashboard_ToolCaptureTitle' },
    { old: 'Tirar prints da tela', key: 'Dashboard_ToolCaptureDesc' },
    { old: 'Modo Ativo', key: 'Dashboard_ToolAwakeTitle' },
    { old: 'Mantém o PC acordado', key: 'Dashboard_ToolAwakeDesc' },
    { old: 'Sempre no Topo', key: 'Dashboard_ToolTopTitle' },
    { old: 'Fixa janelas na frente', key: 'Dashboard_ToolTopDesc' },
    { old: 'Tradutor', key: 'Dashboard_ToolTransTitle' },
    { old: 'Tradução instantânea', key: 'Dashboard_ToolTransDesc' },
    { old: 'Temporizador', key: 'Dashboard_ToolTimerTitle' },
    { old: 'Cronômetro e timer', key: 'Dashboard_ToolTimerDesc' },
    { old: 'Update Center', key: 'Dashboard_ToolUpdateTitle' },
    { old: 'Atualizações de apps', key: 'Dashboard_ToolUpdateDesc' },
    { old: 'Pesquisa Universal', key: 'Dashboard_ToolSearchTitle' },
    { old: 'Busca em tudo', key: 'Dashboard_ToolSearchDesc' },
    { old: 'Paleta de Comandos', key: 'Dashboard_ToolPalleteTitle' },
    { old: 'Comandos CLI rápidos', key: 'Dashboard_ToolPalleteDesc' },
    { old: 'FutureShell', key: 'Dashboard_ToolShellTitle' },
    { old: 'Terminal Independente', key: 'Dashboard_ToolShellDesc' },
    { old: 'ChatFT', key: 'Dashboard_Pro1Title' },
    { old: 'Transcrição e chat offline', key: 'Dashboard_Pro1Desc' },
    { old: 'Automações', key: 'Dashboard_Pro2Title' },
    { old: 'Workflows automatizados', key: 'Dashboard_Pro2Desc' },
    { old: 'Cloud Sync', key: 'Dashboard_Pro3Title' },
    { old: 'Sincronize notas e dados', key: 'Dashboard_Pro3Desc' },
    { old: 'Perfis', key: 'Dashboard_Pro4Title' },
    { old: 'Múltiplos perfis de uso', key: 'Dashboard_Pro4Desc' },
    { old: 'Prompts de IA', key: 'Dashboard_PromptsTitle' },
    { old: 'Biblioteca de prompts', key: 'Dashboard_PromptsDesc' },
    { old: 'Documentação', key: 'Dashboard_BtnDoc', attrName: 'Content' },
    { old: 'Repositório GitHub', key: 'Dashboard_BtnGitHub', attrName: 'Content' },
    { old: 'Sobre o Projeto', key: 'Dashboard_BtnAbout', attrName: 'Content' },
    { old: 'Reportar Bug', key: 'Dashboard_BtnBugReport', attrName: 'Content' },
    { old: '🚀 Conhecer o PRO', key: 'Dashboard_BtnUpgradePro', attrName: 'Content' },
    { old: '💳 Checkout Stripe', key: 'Dashboard_BtnStripe', attrName: 'Content' },
    { old: 'Assinatura Mensal | R$ 39,90', key: 'Dashboard_StripeMonthly' },
    { old: 'Assinatura Anual | R$ 429,90', key: 'Dashboard_StripeYearly' },
    { old: '© 2026 Foreign Technologies®. All rights reserved', key: 'Dashboard_Copyright' }
];

let content = fs.readFileSync(filePath, 'utf8');
let changed = 0;

for (const r of replacements) {
    const attrName = r.attrName || 'Text';
    const escaped = r.old.replace(/[.*+?^${}()|[\]\\]/g, '\\$&');
    
    // Check if already injected
    const checkPattern = new RegExp(`helpers:Translate\\.Key="${r.key}"\\s+${attrName}="${escaped}"`);
    if (checkPattern.test(content)) { continue; }
    
    const pattern = new RegExp(`(${attrName}="${escaped}")`, 'g');
    const newContent = content.replace(pattern, `helpers:Translate.Key="${r.key}" $1`);
    if (newContent !== content) {
        content = newContent;
        changed++;
    }
}

fs.writeFileSync(filePath, content, 'utf8');
console.log(`Updated DashboardPage.xaml: ${changed} keys injected`);
