const fs = require('fs');
const path = 'src/DesktopCommandCenter.UI/Views/DashboardPage.xaml.cs';
let content = fs.readFileSync(path, 'utf8');

const replacements = [
  ['"Atenção: Uso de memória RAM elevado."', 'DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("Dash_HealthWarn")'],
  ['"Alerta de Desempenho"', 'DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("Dash_PerfAlert")'],
  ['$"Sua RAM atingiu {memStatus.dwMemoryLoad}%. Deseja que eu analise e encerre processos ociosos para liberar espaço?"', 'string.Format(DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("Dash_PerfAlertDesc"), memStatus.dwMemoryLoad)'],
  ['"Encontrar processos pesados"', 'DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("Dash_BtnFindHeavy")'],
  ['"Limpar memória em cache"', 'DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("Dash_BtnClearCache")'],
  ['"Seu computador está saudável."', 'DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("Dash_HealthOk")'],
  ['"O que posso fazer por você?"', 'DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("Dash_AiAsk")'],
  ['"Tudo parece tranquilo no momento. Posso ajudar com alguma automação ou pesquisa?"', 'DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("Dash_AiAskDesc")'],
  ['"Criar nova Automação"', 'DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("Dash_BtnNewAuto")'],
  ['"Abrir ChatFT"', 'DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("Dash_BtnOpenChatFT")'],
  ['"Otimizar Rede (Flush DNS)"', 'DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("Dash_BtnFlushDns")'],
  ['"Limpando..."', 'DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("Dash_BtnClearing")'],
  ['"✓ Memória Otimizada!"', 'DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("Dash_BtnMemOptimized")'],
  ['"Limpando DNS..."', 'DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("Dash_BtnClearingDns")'],
  ['"✓ Rede Otimizada!"', 'DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("Dash_BtnNetOptimized")'],

  // DevTools
  ['t.Status = "Instalado";', 't.Status = DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("Dash_DevInstalled");'],
  ['tool.Status = "Instalado";', 'tool.Status = DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("Dash_DevInstalled");'],
  ['tool.Status = "Rodando";', 'tool.Status = DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("Dash_DevRunning");'],
  ['Status == "Rodando"', 'Status == DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("Dash_DevRunning")'],
  ['Status == "Instalado"', 'Status == DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("Dash_DevInstalled")'],
  ['Status == "Atualizar"', 'Status == DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("Dash_DevUpdate")'],

  ['"Nenhuma Ferramenta"', 'DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("Dash_DevNoTools")'],
  ['"Encontrada"', 'DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("Dash_DevFound")'],

  ['?? "Windows Update está Atualizado!"', ''],
  ['?? "FutureShell executou winget upgrade"', ''],
  ['?? "RAM chegou a 95%"', ''],
  ['?? "Bluetooth conectado"', ''],
  ['?? "ChatFT gerou um script PowerShell"', '']
];

for (const [search, replace] of replacements) {
  content = content.split(search).join(replace);
}

fs.writeFileSync(path, content, 'utf8');
console.log('Replaced texts in ' + path);
