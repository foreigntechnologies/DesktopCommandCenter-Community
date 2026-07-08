const fs = require('fs');

const pt = {
  'Dash_HealthWarn': 'Atenção: Uso de memória RAM elevado.',
  'Dash_PerfAlert': 'Alerta de Desempenho',
  'Dash_PerfAlertDesc': 'Sua RAM atingiu {0}%. Deseja que eu analise e encerre processos ociosos para liberar espaço?',
  'Dash_BtnFindHeavy': 'Encontrar processos pesados',
  'Dash_BtnClearCache': 'Limpar memória em cache',
  'Dash_HealthOk': 'Seu computador está saudável.',
  'Dash_AiAsk': 'O que posso fazer por você?',
  'Dash_AiAskDesc': 'Tudo parece tranquilo no momento. Posso ajudar com alguma automação ou pesquisa?',
  'Dash_BtnNewAuto': 'Criar nova Automação',
  'Dash_BtnOpenChatFT': 'Abrir ChatFT',
  'Dash_BtnFlushDns': 'Otimizar Rede (Flush DNS)',
  'Dash_BtnClearing': 'Limpando...',
  'Dash_BtnMemOptimized': '✓ Memória Otimizada!',
  'Dash_BtnClearingDns': 'Limpando DNS...',
  'Dash_BtnNetOptimized': '✓ Rede Otimizada!',
  'Dash_DevInstalled': 'Instalado',
  'Dash_DevNoTools': 'Nenhuma Ferramenta',
  'Dash_DevFound': 'Encontrada',
  'Dash_DevRunning': 'Rodando',
  'Dash_DevUpdate': 'Atualizar',
  'Dash_CmdWinget': 'winget upgrade',
  'Dash_CmdGit': 'git status',
  'Dash_CmdNpm': 'npm run dev',
  'Dash_Chat1': '✓ Criar Dockerfile',
  'Dash_Chat2': '✓ Revisar código C#',
  'Dash_Chat3': '✓ Gerar Prompt de IA',
  'Dash_Timeline1': 'Windows Update está Atualizado!',
  'Dash_Timeline2': 'FutureShell executou winget upgrade',
  'Dash_Timeline3': 'RAM chegou a {0}%',
  'Dash_Timeline4': 'Bluetooth conectado',
  'Dash_Timeline5': 'ChatFT gerou um script PowerShell'
};

const en = {
  'Dash_HealthWarn': 'Attention: High RAM usage.',
  'Dash_PerfAlert': 'Performance Alert',
  'Dash_PerfAlertDesc': 'Your RAM reached {0}%. Do you want me to analyze and terminate idle processes to free up space?',
  'Dash_BtnFindHeavy': 'Find heavy processes',
  'Dash_BtnClearCache': 'Clear cache memory',
  'Dash_HealthOk': 'Your computer is healthy.',
  'Dash_AiAsk': 'What can I do for you?',
  'Dash_AiAskDesc': 'Everything seems quiet at the moment. Can I help with an automation or search?',
  'Dash_BtnNewAuto': 'Create new Automation',
  'Dash_BtnOpenChatFT': 'Open ChatFT',
  'Dash_BtnFlushDns': 'Optimize Network (Flush DNS)',
  'Dash_BtnClearing': 'Clearing...',
  'Dash_BtnMemOptimized': '✓ Memory Optimized!',
  'Dash_BtnClearingDns': 'Clearing DNS...',
  'Dash_BtnNetOptimized': '✓ Network Optimized!',
  'Dash_DevInstalled': 'Installed',
  'Dash_DevNoTools': 'No Tools',
  'Dash_DevFound': 'Found',
  'Dash_DevRunning': 'Running',
  'Dash_DevUpdate': 'Update',
  'Dash_CmdWinget': 'winget upgrade',
  'Dash_CmdGit': 'git status',
  'Dash_CmdNpm': 'npm run dev',
  'Dash_Chat1': '✓ Create Dockerfile',
  'Dash_Chat2': '✓ Review C# code',
  'Dash_Chat3': '✓ Generate AI Prompt',
  'Dash_Timeline1': 'Windows Update is Up to Date!',
  'Dash_Timeline2': 'FutureShell executed winget upgrade',
  'Dash_Timeline3': 'RAM reached {0}%',
  'Dash_Timeline4': 'Bluetooth connected',
  'Dash_Timeline5': 'ChatFT generated a PowerShell script'
};

const es = {
  'Dash_HealthWarn': 'Atención: Alto uso de memoria RAM.',
  'Dash_PerfAlert': 'Alerta de Rendimiento',
  'Dash_PerfAlertDesc': 'Su RAM alcanzó el {0}%. ¿Desea que analice y cierre procesos inactivos para liberar espacio?',
  'Dash_BtnFindHeavy': 'Encontrar procesos pesados',
  'Dash_BtnClearCache': 'Limpiar memoria caché',
  'Dash_HealthOk': 'Su computadora está sana.',
  'Dash_AiAsk': '¿Qué puedo hacer por ti?',
  'Dash_AiAskDesc': 'Todo parece tranquilo en este momento. ¿Puedo ayudar con alguna automatización o búsqueda?',
  'Dash_BtnNewAuto': 'Crear nueva Automatización',
  'Dash_BtnOpenChatFT': 'Abrir ChatFT',
  'Dash_BtnFlushDns': 'Optimizar Red (Flush DNS)',
  'Dash_BtnClearing': 'Limpiando...',
  'Dash_BtnMemOptimized': '✓ ¡Memoria Optimizada!',
  'Dash_BtnClearingDns': 'Limpiando DNS...',
  'Dash_BtnNetOptimized': '✓ ¡Red Optimizada!',
  'Dash_DevInstalled': 'Instalado',
  'Dash_DevNoTools': 'Sin Herramientas',
  'Dash_DevFound': 'Encontrada',
  'Dash_DevRunning': 'En ejecución',
  'Dash_DevUpdate': 'Actualizar',
  'Dash_CmdWinget': 'winget upgrade',
  'Dash_CmdGit': 'git status',
  'Dash_CmdNpm': 'npm run dev',
  'Dash_Chat1': '✓ Crear Dockerfile',
  'Dash_Chat2': '✓ Revisar código C#',
  'Dash_Chat3': '✓ Generar Prompt de IA',
  'Dash_Timeline1': '¡Windows Update está actualizado!',
  'Dash_Timeline2': 'FutureShell ejecutó winget upgrade',
  'Dash_Timeline3': 'La RAM llegó a {0}%',
  'Dash_Timeline4': 'Bluetooth conectado',
  'Dash_Timeline5': 'ChatFT generó un script de PowerShell'
};

const updateFile = (file, additions) => {
  if (fs.existsSync(file)) {
    let raw = fs.readFileSync(file, 'utf8');
    let json = JSON.parse(raw);
    for (const [k, v] of Object.entries(additions)) {
      if (!json[k]) json[k] = v;
    }
    fs.writeFileSync(file, JSON.stringify(json, null, 2), 'utf8');
    console.log('Updated ' + file);
  }
};

updateFile('src/DesktopCommandCenter.UI/Resources/pt-BR.json', pt);
updateFile('src/DesktopCommandCenter.UI/Resources/en-US.json', en);
updateFile('src/DesktopCommandCenter.UI/Resources/es-ES.json', es);
