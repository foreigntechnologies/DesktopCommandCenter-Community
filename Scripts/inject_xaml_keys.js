const fs = require('fs');
const path = require('path');
const viewsDir = path.join('C:', 'Users', 'kogli', 'Desktop', 'Projetos', 'Windows', 'DesktopCommandCenter-Enterprise', 'Community', 'src', 'DesktopCommandCenter.UI', 'Views');

// Helper: inject Translate.Key before a Text/Content/Header/PlaceholderText attribute
function injectKey(content, oldAttrValue, key, attrName = 'Text') {
  const escaped = oldAttrValue.replace(/[.*+?^${}()|[\]\\]/g, '\\$&');
  const pattern = new RegExp(`(${attrName}="${escaped}")`, 'g');
  return content.replace(pattern, `helpers:Translate.Key="${key}" $1`);
}

// Map of file -> list of {oldAttrValue, key, attrName} replacements
const fileUpdates = {
  'CapturaPage.xaml': [
    { old: 'Captura de Tela', key: 'Captura_PageTitle' },
    { old: 'Estúdio de Captura', key: 'Captura_SubTitle' },
    { old: 'Selecione abaixo para iniciar a ferramenta de captura de tela inteligente do Windows e salvar imagens diretamente no seu Clipboard.', key: 'Captura_Desc' },
    { old: '📸 Nova Captura', key: 'Captura_BtnNew', attrName: 'Content' },
  ],
  'ClipboardPage.xaml': [
    { old: 'Smart Clipboard', key: 'Clipboard_PageTitle' },
    { old: 'Limpar Histórico', key: 'Clipboard_BtnClear', attrName: 'Content' },
    { old: 'Copiar novamente', key: 'Clipboard_ItemCopy', attrName: 'Content' },
    { old: 'Excluir', key: 'Clipboard_ItemDelete', attrName: 'Content' },
  ],
  'ColorPickerPage.xaml': [
    { old: 'Color Picker', key: 'ColorPicker_PageTitle' },
    { old: 'HEX', key: 'ColorPicker_HexLabel' },
    { old: 'Copiar HEX', key: 'ColorPicker_BtnCopyHex', attrName: 'Content' },
    { old: 'RGB', key: 'ColorPicker_RgbLabel' },
    { old: 'Copiar RGB', key: 'ColorPicker_BtnCopyRgb', attrName: 'Content' },
    { old: 'Ativar Conta-gotas (Mouse)', key: 'ColorPicker_EyedropperBtn', attrName: 'Content' },
    { old: 'Mova o mouse pela tela para capturar cores de qualquer janela. Clique em copiar para salvar.', key: 'ColorPicker_EyedropperDesc' },
  ],
  'ComingSoonPage.xaml': [
    { old: 'Em Breve!', key: 'ComingSoon_Title' },
    { old: 'Estamos construindo este módulo para você.', key: 'ComingSoon_Desc' },
  ],
  'NotesPage.xaml': [
    { old: 'Notas Rápidas', key: 'Notes_PageTitle' },
    { old: 'Nova Nota', key: 'Notes_BtnNew', attrName: 'Content' },
  ],
  'OobeDialog.xaml': [
    { old: 'Bem-vindo ao Desktop Command Center!', key: 'OOBE_Title' },
    { old: 'Continuar', key: 'OOBE_BtnContinue', attrName: 'Content' },
    { old: 'Antes de começarmos, escolha o seu tema preferido:', key: 'OOBE_ThemeLabel' },
    { old: 'Claro', key: 'OOBE_ThemeLight', attrName: 'Content' },
    { old: 'Escuro', key: 'OOBE_ThemeDark', attrName: 'Content' },
    { old: 'Padrão do Sistema (Contraste)', key: 'OOBE_ThemeSystem', attrName: 'Content' },
    { old: 'Você poderá alterar isso nas Configurações posteriormente.', key: 'OOBE_ThemeNote' },
  ],
  'PesquisaUniversalPage.xaml': [
    { old: 'Pesquisa Universal Avançada', key: 'Search_PageTitle' },
    { old: 'Abrir / Executar', key: 'Search_BtnOpen', attrName: 'Content' },
    { old: 'Enviar para IA', key: 'Search_BtnSendAI', attrName: 'Content' },
  ],
  'ProcessManagerPage.xaml': [
    { old: 'Gerenciador de Processos Avançado', key: 'ProcessMgr_PageTitle' },
    { old: 'Nome do Processo', key: 'ProcessMgr_ColName' },
    { old: 'PID', key: 'ProcessMgr_ColPid' },
    { old: 'CPU (%)', key: 'ProcessMgr_ColCpu' },
    { old: 'RAM (MB)', key: 'ProcessMgr_ColRam' },
    { old: 'Status', key: 'ProcessMgr_ColStatus' },
    { old: 'Encerrar Árvore de Processos', key: 'ProcessMgr_BtnKillTree', attrName: 'Content' },
    { old: 'Órfão', key: 'ProcessMgr_StatusOrphan' },
  ],
  'PromptsPage.xaml': [
    { old: 'Biblioteca de Prompts', key: 'Prompts_PageTitle' },
    { old: 'Copiar', key: 'Prompts_BtnCopy', attrName: 'Content' },
    { old: 'Editar', key: 'Prompts_BtnEdit', attrName: 'Content' },
    { old: 'Excluir', key: 'Prompts_BtnDelete', attrName: 'Content' },
    { old: 'Nenhum prompt salvo ainda.', key: 'Prompts_EmptyTitle' },
    { old: 'Crie um novo prompt usando o formulário à direita.', key: 'Prompts_EmptyDesc' },
    { old: 'Salvar', key: 'Prompts_BtnSave', attrName: 'Content' },
    { old: 'Cancelar', key: 'Prompts_BtnCancel', attrName: 'Content' },
  ],
  'TemporizadorPage.xaml': [
    { old: 'Temporizador (Pomodoro)', key: 'Temporizador_PageTitle' },
    { old: 'Anos', key: 'Temporizador_Years' },
    { old: 'Meses', key: 'Temporizador_Months' },
    { old: 'Dias', key: 'Temporizador_Days' },
    { old: 'Horas', key: 'Temporizador_Hours' },
    { old: 'Minutos', key: 'Temporizador_Minutes' },
    { old: 'Segundos', key: 'Temporizador_Seconds' },
    { old: 'Milissegundos', key: 'Temporizador_Ms' },
    { old: 'Iniciar', key: 'Temporizador_BtnStart', attrName: 'Content' },
    { old: 'Pausar', key: 'Temporizador_BtnPause', attrName: 'Content' },
    { old: 'Resetar', key: 'Temporizador_BtnReset', attrName: 'Content' },
  ],
};

let totalChanges = 0;

for (const [filename, replacements] of Object.entries(fileUpdates)) {
  const filePath = path.join(viewsDir, filename);
  if (!fs.existsSync(filePath)) { console.log('SKIP (not found):', filename); continue; }
  
  let content = fs.readFileSync(filePath, 'utf8');
  let changed = 0;
  
  for (const r of replacements) {
    const attrName = r.attrName || 'Text';
    const escaped = r.old.replace(/[.*+?^${}()|[\]\\]/g, '\\$&');
    
    // Only inject if not already having Translate.Key before this attribute
    const checkPattern = new RegExp(`helpers:Translate\\.Key="${r.key}"\\s+${attrName}="${escaped}"`);
    if (checkPattern.test(content)) { continue; } // already done
    
    const pattern = new RegExp(`(${attrName}="${escaped}")`, 'g');
    const newContent = content.replace(pattern, `helpers:Translate.Key="${r.key}" $1`);
    if (newContent !== content) {
      content = newContent;
      changed++;
    }
  }
  
  if (changed > 0) {
    fs.writeFileSync(filePath, content, 'utf8');
    console.log(`Updated ${filename}: ${changed} keys injected`);
    totalChanges += changed;
  } else {
    console.log(`No changes needed in ${filename}`);
  }
}

console.log(`\nTotal: ${totalChanges} keys injected across all files`);
