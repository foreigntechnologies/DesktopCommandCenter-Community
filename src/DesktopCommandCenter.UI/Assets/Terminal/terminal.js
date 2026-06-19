let term;
let fitAddon;

function initTerminal() {
    term = new Terminal({
        cursorBlink: true,
        theme: {
            background: 'transparent',
            foreground: '#e0e0e0',
            cursor: '#00d2ff',
            selection: 'rgba(0, 210, 255, 0.3)',
            black: '#000000',
            red: '#e06c75',
            green: '#98c379',
            yellow: '#d19a66',
            blue: '#61afef',
            magenta: '#c678dd',
            cyan: '#56b6c2',
            white: '#dcdfe4',
            brightBlack: '#5c6370',
            brightRed: '#e06c75',
            brightGreen: '#98c379',
            brightYellow: '#d19a66',
            brightBlue: '#61afef',
            brightMagenta: '#c678dd',
            brightCyan: '#56b6c2',
            brightWhite: '#ffffff'
        },
        fontFamily: "'Cascadia Code', 'Consolas', 'Courier New', monospace",
        fontSize: 14,
        allowTransparency: true
    });

    fitAddon = new FitAddon.FitAddon();
    term.loadAddon(fitAddon);

    term.open(document.getElementById('terminal-container'));
    fitAddon.fit();

    // Send keystrokes to C# host
    term.onData(data => {
        if (window.chrome && window.chrome.webview) {
            window.chrome.webview.postMessage({ type: 'input', data: data });
        }
    });

    // Handle resize
    window.addEventListener('resize', () => {
        fitAddon.fit();
        if (window.chrome && window.chrome.webview) {
            window.chrome.webview.postMessage({ 
                type: 'resize', 
                cols: term.cols, 
                rows: term.rows 
            });
        }
    });
    
    // Initial size push
    setTimeout(() => {
        if (window.chrome && window.chrome.webview) {
            window.chrome.webview.postMessage({ 
                type: 'resize', 
                cols: term.cols, 
                rows: term.rows 
            });
        }
    }, 100);
}

// Receive messages from C# host
if (window.chrome && window.chrome.webview) {
    window.chrome.webview.addEventListener('message', event => {
        const msg = event.data;
        if (msg.type === 'output') {
            term.write(msg.data);
        } else if (msg.type === 'hud') {
            if (msg.cpu) document.getElementById('cpu-hud').innerText = `CPU: ${msg.cpu}`;
            if (msg.ram) document.getElementById('ram-hud').innerText = `RAM: ${msg.ram}`;
            if (msg.ai) document.getElementById('ai-status').innerText = `AI: ${msg.ai}`;
        }
    });
}

// Initialize on load
window.onload = initTerminal;
