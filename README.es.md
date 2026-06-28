# Desktop Command Center (DCC)

<p align="center">
  <img src="src/DesktopCommandCenter.UI/Assets/StoreLogo.png" alt="Desktop Command Center Logo" width="200" />
</p>

[![Language: English](https://img.shields.io/badge/Language-English-blue.svg)](README.md)
[![Idioma: Português](https://img.shields.io/badge/Idioma-Português-green.svg)](README.pt-BR.md)
[![Idioma: Español](https://img.shields.io/badge/Idioma-Español-yellow.svg)](README.es.md)

## Visión del Producto
El **Desktop Command Center (DCC)** es un centro de productividad en formato de barra lateral inteligente, siempre accesible y fijado en el borde de la pantalla de Windows 11. El objetivo es eliminar el cambio constante entre aplicaciones, reuniendo las herramientas de uso diario más frecuentes en un solo lugar elegante.

Diseñado para profesionales, desarrolladores, creadores de contenido, analistas y usuarios avanzados.

## Filosofía: Local First + Cloud Light
- **Local First**: Todos los datos del usuario (Notas, Historial del Portapapeles, Configuraciones) permanecen estrictamente en la máquina local utilizando SQLite. Tu contenido nunca se sube a la nube.
- **Cloud Light**: La conectividad en la nube (Firebase Auth) se utiliza exclusivamente como Proveedor de Identidad (Google / GitHub) para vincular un ID de Usuario único para la validación de licencias/suscripciones a través de Stripe.

## Características
- ✨ **Fluent Design System**: Construido nativamente para Windows 11 con fondos translúcidos (Mica) y animaciones fluidas.
- 🤖 **ChatFT (Agente IA)**: Un agente autónomo "Local First" impulsado por Microsoft Semantic Kernel y Ollama. Capaz de ejecutar complementos locales de C# (por ejemplo, leer tus archivos, obtener la hora del sistema) completamente sin conexión.
- 🎙️ **Voz y Visión**: Incluye Whisper.net para transcripción de audio sin conexión (micrófono y archivos) y soporte de Visión multimodal utilizando Ollama.
- 📋 **Smart Clipboard**: Un servicio en segundo plano que captura silenciosamente tu historial de portapapeles, permitiéndote buscar y recuperar copias anteriores al instante.
- 📝 **Notas Rápidas**: Un bloc de notas increíblemente rápido integrado en la barra lateral.
- 🌍 **Localización en Tiempo Real**: Interfaz completamente traducida (Inglés, Portugués, Español).
- 💻 **FutureShell**: Un terminal robusto PTY (Pseudo Console) integrado, capaz de ejecutar PowerShell, CMD, Bash y cualquier CLI global instalada en su sistema. Soporta un sistema de ayuda inteligente personalizado escribiendo `help` o `principal-commands`.

## Arquitectura y Tecnologías
- **Framework**: .NET 9, Windows App SDK (WinUI 3)
- **Patrón de Diseño**: Clean Architecture + MVVM
- **Base de Datos**: Entity Framework Core + SQLite
- **Mensajería**: MediatR (Patrón CQRS)
- **Inyección de Dependencias**: Microsoft.Extensions.DependencyInjection
- **Autenticación**: FirebaseAuthentication.net (OAuth)

## Guía de Inicio
### Requisitos Previos
- Windows 10 (19041) o Windows 11
- SDK de .NET 9
- Visual Studio 2022 (con carga de trabajo de Windows App SDK)

### Compilar y Ejecutar
```bash
# Restaurar dependencias
dotnet restore DesktopCommandCenter.slnx

# Compilar la solución
dotnet build DesktopCommandCenter.slnx

# Ejecutar el proyecto UI
dotnet run --project src/DesktopCommandCenter.UI/DesktopCommandCenter.UI.csproj
```

### Empaquetado y Lanzamientos
Para generar el instalador y la versión portátil del DCC, puedes utilizar los scripts automatizados:
```powershell
# Compilar versión Community
./build_community.ps1 -Version "0.0.1"
```

### Docker y Stack de IA Local
Este repositorio incluye un archivo `docker-compose.yml` para orquestar el servidor de IA local (Ollama) y un contenedor para compilar la aplicación.

**1. Ejecutar el Agente IA Local (Ollama)**
```powershell
docker-compose up -d ollama
```

**2. Compilar mediante Docker (Requiere Windows Containers)**
Asegúrate de que Docker Desktop esté configurado en "Windows Containers".
```powershell
# Compilar Edición Community
docker-compose run --rm build-exe 0.0.2 COMMUNITY
```

## Aviso de Seguridad
Este proyecto utiliza Firebase para la Gestión de Identidad. **No hagas commit** de claves de Cuentas de Servicio (`.json`) de Firebase ni archivos de entorno con credenciales confidenciales. La aplicación cliente solo requiere la Web API Key pública.

## Licencia
Software Propietario. Todos los derechos reservados.
