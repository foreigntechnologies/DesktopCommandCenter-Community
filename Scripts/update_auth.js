const fs = require('fs');
const file = 'C:/Users/kogli/Desktop/Projetos/Windows/DesktopCommandCenter-Enterprise/Community/src/DesktopCommandCenter.UI/Views/AuthPage.xaml';
let c = fs.readFileSync(file, 'utf8');

const replacements = [
    { search: 'Text="Licenciamento &amp; Nuvem"', replace: 'helpers:Translate.Key="Auth_LicenseCloud" Text="Licenciamento &amp; Nuvem"' },
    { search: 'Text="O DCC funciona 100% offline e gratuito. Faça login apenas para desbloquear os recursos PRO."', replace: 'helpers:Translate.Key="Auth_OfflineDesc" Text="O DCC funciona 100% offline e gratuito. Faça login apenas para desbloquear os recursos PRO."' },
    { search: 'Text="Processando requisição... (se você fechou o navegador, clique em Cancelar)"', replace: 'helpers:Translate.Key="Auth_Processing" Text="Processando requisição... (se você fechou o navegador, clique em Cancelar)"' },
    { search: 'Content="Cancelar"', replace: 'helpers:Translate.Key="Auth_Cancel" Content="Cancelar"' },
    { search: 'Content="Continuar com o Google"', replace: 'helpers:Translate.Key="Auth_ContinueGoogle" Content="Continuar com o Google"' },
    { search: 'Content="Continuar com o GitHub"', replace: 'helpers:Translate.Key="Auth_ContinueGitHub" Content="Continuar com o GitHub"' },
    { search: 'Text="Vincular contas adicionais:"', replace: 'helpers:Translate.Key="Auth_LinkAccounts" Text="Vincular contas adicionais:"' },
    { search: 'Content="Vincular Google"', replace: 'helpers:Translate.Key="Auth_LinkGoogle" Content="Vincular Google"' },
    { search: 'Content="Vincular GitHub"', replace: 'helpers:Translate.Key="Auth_LinkGitHub" Content="Vincular GitHub"' },
    { search: 'Text="Ao vincular, você poderá entrar na sua conta com qualquer um dos provedores associados."', replace: 'helpers:Translate.Key="Auth_LinkDesc" Text="Ao vincular, você poderá entrar na sua conta com qualquer um dos provedores associados."' },
    { search: 'Text="Plano PRO Ativo!"', replace: 'helpers:Translate.Key="Auth_ProActiveTitle" Text="Plano PRO Ativo!"' },
    { search: 'Text="Você tem acesso completo a todos os recursos exclusivos."', replace: 'helpers:Translate.Key="Auth_ProActiveDesc" Text="Você tem acesso completo a todos os recursos exclusivos."' },
    { search: 'Content="Gerenciar ou Cancelar Assinatura"', replace: 'helpers:Translate.Key="Auth_ManageSub" Content="Gerenciar ou Cancelar Assinatura"' },
    { search: 'Text="Plano Pausado"', replace: 'helpers:Translate.Key="Auth_ProPausedTitle" Text="Plano Pausado"' },
    { search: 'Text="Sua assinatura está pausada. Retome-a no portal para voltar a ter acesso PRO."', replace: 'helpers:Translate.Key="Auth_ProPausedDesc" Text="Sua assinatura está pausada. Retome-a no portal para voltar a ter acesso PRO."' },
    { search: 'Content="Gerenciar Assinatura"', replace: 'helpers:Translate.Key="Auth_ManagePausedSub" Content="Gerenciar Assinatura"' },
    { search: 'Content="Fazer Logout"', replace: 'helpers:Translate.Key="Auth_Logout" Content="Fazer Logout"' },
    { search: 'Text="Assinatura e Conta"', replace: 'helpers:Translate.Key="Auth_SubAccount" Text="Assinatura e Conta"' }
];

for (const r of replacements) {
    c = c.replace(new RegExp(r.search.replace(/[.*+?^${}()|[\]\\]/g, '\\$&'), 'g'), r.replace);
}
fs.writeFileSync(file, c, 'utf8');
console.log('AuthPage updated');
