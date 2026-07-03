const fs = require('fs');
const path = require('path');

function walk(dir, done) {
  let results = [];
  fs.readdir(dir, function(err, list) {
    if (err) return done(err);
    let i = 0;
    (function next() {
      let file = list[i++];
      if (!file) return done(null, results);
      file = path.resolve(dir, file);
      fs.stat(file, function(err, stat) {
        if (stat && stat.isDirectory()) {
          walk(file, function(err, res) {
            results = results.concat(res);
            next();
          });
        } else {
          results.push(file);
          next();
        }
      });
    })();
  });
}

const viewsDir = path.join('C:', 'Users', 'kogli', 'Desktop', 'Projetos', 'Windows', 'DesktopCommandCenter-Enterprise', 'Community', 'src', 'DesktopCommandCenter.UI', 'Views');

walk(viewsDir, function(err, results) {
  if (err) throw err;
  const xamlFiles = results.filter(f => f.endsWith('.xaml'));
  
  xamlFiles.forEach(file => {
      let content = fs.readFileSync(file, 'utf8');
      let changed = false;

      // Self-closing tags <... />
      let newContent = content.replace(/<([a-zA-Z0-9:]+)([^>]*?)\/>/g, (match, tag, attrs) => {
          const translateMatches = [...attrs.matchAll(/helpers:Translate\.Key="([^"]+)"/g)];
          if (translateMatches.length > 1) {
              const firstMatch = translateMatches[0][0];
              let newAttrs = attrs.replace(/helpers:Translate\.Key="[^"]+"/g, '');
              changed = true;
              return `<${tag}${newAttrs} ${firstMatch}/>`;
          }
          return match;
      });

      // Opening tags <... >
      newContent = newContent.replace(/<([a-zA-Z0-9:]+)([^>]*?)>/g, (match, tag, attrs) => {
          // Exclude self-closing which we already handled (though > matches /> if we aren't careful)
          if (match.endsWith('/>')) return match;
          const translateMatches = [...attrs.matchAll(/helpers:Translate\.Key="([^"]+)"/g)];
          if (translateMatches.length > 1) {
              const firstMatch = translateMatches[0][0];
              let newAttrs = attrs.replace(/helpers:Translate\.Key="[^"]+"/g, '');
              changed = true;
              return `<${tag}${newAttrs} ${firstMatch}>`;
          }
          return match;
      });

      if (changed) {
          fs.writeFileSync(file, newContent, 'utf8');
          console.log(`Fixed duplicates in ${path.basename(file)}`);
      }
  });
});
