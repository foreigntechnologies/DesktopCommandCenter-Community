# Desktop Command Center (DCC)

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
- 📋 **Smart Clipboard**: Un servicio en segundo plano que captura silenciosamente tu historial de portapapeles, permitiéndote buscar y recuperar copias anteriores al instante.
- 📝 **Notas Rápidas**: Un bloc de notas increíblemente rápido integrado en la barra lateral.
- 🌍 **Localización en Tiempo Real**: Interfaz completamente traducida (Inglés, Portugués, Español).

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

## Aviso de Seguridad
Este proyecto utiliza Firebase para la Gestión de Identidad. **No hagas commit** de claves de Cuentas de Servicio (`.json`) de Firebase ni archivos de entorno con credenciales confidenciales. La aplicación cliente solo requiere la Web API Key pública.

## Licencia
Software Propietario. Todos los derechos reservados.
