let term;
let fitAddon;

function initTerminal() {
    term = new Terminal({
        cursorBlink: true,
        theme: {
            background: 'transparent',
            foreground: '#CCCCCC',
            cursor: '#FFFFFF',
            selection: 'rgba(255, 255, 255, 0.3)',
            black: '#0C0C0C',
            red: '#C50F1F',
            green: '#13A10E',
            yellow: '#C19C00',
            blue: '#0037DA',
            magenta: '#881798',
            cyan: '#3A96DD',
            white: '#CCCCCC',
            brightBlack: '#767676',
            brightRed: '#E74856',
            brightGreen: '#16C60C',
            brightYellow: '#F9F1A5',
            brightBlue: '#3B78FF',
            brightMagenta: '#B4009E',
            brightCyan: '#61D6D6',
            brightWhite: '#F2F2F2'
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
        let msg = event.data;
        if (typeof msg === 'string') {
            if (msg.startsWith("B64:")) {
                const base64 = msg.substring(4);
                const binaryString = atob(base64);
                const bytes = new Uint8Array(binaryString.length);
                for (let i = 0; i < binaryString.length; i++) {
                    bytes[i] = binaryString.charCodeAt(i);
                }
                const decodedText = new TextDecoder('utf-8').decode(bytes);
                term.write(decodedText);
                return;
            }
            try { msg = JSON.parse(msg); } catch (e) {}
        }
        if (msg.type === 'output') {
            term.write(msg.data);
        } else if (msg.type === 'hud') {
            if (msg.cpu) document.getElementById('cpu-hud').innerText = `CPU: ${msg.cpu}`;
            if (msg.ram) document.getElementById('ram-hud').innerText = `RAM: ${msg.ram}`;
        }
    });
}

// Initialize on load
window.onload = initTerminal;
