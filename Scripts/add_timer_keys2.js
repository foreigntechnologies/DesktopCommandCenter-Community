const fs = require('fs');

const keysToAdd = {
    'Temporizador_InputYears': { 'pt-BR': 'Anos', 'en-US': 'Years', 'es-ES': 'Años' },
    'Temporizador_InputMonths': { 'pt-BR': 'Meses', 'en-US': 'Months', 'es-ES': 'Meses' },
    'Temporizador_InputDays': { 'pt-BR': 'Dias', 'en-US': 'Days', 'es-ES': 'Días' },
    'Temporizador_InputHours': { 'pt-BR': 'Horas', 'en-US': 'Hours', 'es-ES': 'Horas' },
    'Temporizador_InputMinutes': { 'pt-BR': 'Minutos', 'en-US': 'Minutes', 'es-ES': 'Minutos' },
    'Temporizador_InputSeconds': { 'pt-BR': 'Segundos', 'en-US': 'Seconds', 'es-ES': 'Segundos' },
    'Temporizador_InputMilliseconds': { 'pt-BR': 'Milissegundos', 'en-US': 'Milliseconds', 'es-ES': 'Milisegundos' }
};

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
