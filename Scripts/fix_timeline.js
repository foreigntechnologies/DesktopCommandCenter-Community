const fs = require('fs');
const path = 'src/DesktopCommandCenter.UI/Views/DashboardPage.xaml.cs';
let content = fs.readFileSync(path, 'utf8');

content = content.replace(
  'Message = DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("Dash_Timeline3") ',
  'Message = string.Format(DesktopCommandCenter.UI.Helpers.LocalizationHelper.Instance.GetString("Dash_Timeline3"), "95") '
);

fs.writeFileSync(path, content, 'utf8');

const pt = {
  "Dash_Removable": "Removível",
  "Dash_LocalDisk": "Disco Local",
  "Dash_Used": "Usado: {0} GB",
  "Dash_Free": "Livre: {0} GB",
  "Dash_Total": "Total: {0} GB"
};

const en = {
  "Dash_Removable": "Removable",
  "Dash_LocalDisk": "Local Disk",
  "Dash_Used": "Used: {0} GB",
  "Dash_Free": "Free: {0} GB",
  "Dash_Total": "Total: {0} GB"
};

const es = {
  "Dash_Removable": "Extraíble",
  "Dash_LocalDisk": "Disco Local",
  "Dash_Used": "Usado: {0} GB",
  "Dash_Free": "Libre: {0} GB",
  "Dash_Total": "Total: {0} GB"
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
