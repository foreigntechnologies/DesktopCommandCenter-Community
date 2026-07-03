const fs = require('fs');
const path = 'Views/AuthPage.xaml';
let content = fs.readFileSync(path, 'utf8');

// Some of them have ':</Run><Run', some have ':</Run> <Run'
// And some might have a leading space inside the second run, e.g. '> Mica...'
// And some might have a trailing space in the first run, e.g. 'Premium: </Run>'

// 1. Remove trailing spaces before </Run> if they are preceded by ':'
content = content.replace(/:\s+<\/Run>/g, ':</Run>');

// 2. Remove leading spaces after the start of the second run tag
content = content.replace(/(<Run[^>]+DElement"\s*>)\s+/g, '$1');

// 3. Insert <Run Text=" "/> between the closing of the first run and the opening of the second run
content = content.replace(/<\/Run>\s*(<Run x:Name="Settings(Comm|Pro)F\d+DElement")/g, '</Run><Run Text=" " />$1');

fs.writeFileSync(path, content, 'utf8');
