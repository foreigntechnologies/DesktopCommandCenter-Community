# Scripts de Automacao / Automation Scripts / Scripts de Automatizacion

Este diretorio contem todos os scripts criados durante a grande refatoracao do sistema de localizacao do Desktop Command Center. Estes arquivos foram usados temporariamente para automatizar tarefas repetitivas no XAML, C# e JSONs.

This directory contains all the scripts created during the major localization refactoring of Desktop Command Center. These files were used temporarily to automate repetitive tasks in XAML, C#, and JSONs.

Este directorio contiene todos los scripts creados durante la gran refactorizacion del sistema de localizacion del Desktop Command Center. Estos archivos se usaron temporalmente para automatizar tareas repetitivas en XAML, C# y JSONs.

---

## 1. migrate_translations.js / migrate_main_translations.js

**PT:** Scripts responsaveis por ler arquivos XAML, remover helpers:Translate.Key e mover a logica para os metodos UpdateTranslations() em C#. Foram o ponto de partida da migracao de localizacao.

**EN:** Scripts responsible for reading XAML files, removing helpers:Translate.Key attached properties, and moving localization logic to UpdateTranslations() C# code-behind methods. These were the starting point of the localization migration.

**ES:** Scripts responsables de leer archivos XAML, eliminar las propiedades helpers:Translate.Key y mover la logica de localizacion a los metodos UpdateTranslations() en C#. Estos fueron el punto de partida de la migracion de localizacion.

---

## 2. update_auth.js / update_auth_vm.js

**PT:** Atualiza a tela de autenticacao (AuthPage) e seu ViewModel para usar o LocalizationHelper dinamicamente, convertendo textos hardcoded para chaves nos arquivos JSON.

**EN:** Updates the authentication screen (AuthPage) and its ViewModel to use LocalizationHelper dynamically, converting hardcoded texts to JSON resource keys.

**ES:** Actualiza la pantalla de autenticacion (AuthPage) y su ViewModel para usar LocalizationHelper dinamicamente, convirtiendo textos codificados en claves de los archivos JSON.

---

## 3. update_auto_vm.js / update_awake_vm.js

**PT:** Atualiza os ViewModels das paginas de Automacoes e Awake (Caffeine) para suportar traducoes em tempo de execucao.

**EN:** Updates the ViewModels for Automations and Awake (Caffeine) pages to support runtime translations.

**ES:** Actualiza los ViewModels de las paginas de Automatizaciones y Awake (Caffeine) para admitir traducciones en tiempo de ejecucion.

---

## 4. update_batch1.js

**PT:** Script em lote que processa multiplos arquivos de uma vez, atualizando propriedades de traducao em varias paginas simultaneamente. Gerou atualizacoes em massa do UpdateTranslations().

**EN:** Batch script that processes multiple files at once, updating translation properties across several pages simultaneously. Generated mass UpdateTranslations() updates.

**ES:** Script por lotes que procesa multiples archivos a la vez, actualizando propiedades de traduccion en varias paginas simultaneamente. Genero actualizaciones masivas de UpdateTranslations().

---

## 5. update_dashboard_json.js / update_dashboard_dyn.js / update_dashboard_vm.js / update_dash_xaml.js

**PT:** Conjunto de scripts dedicados a migrar a pagina principal (Dashboard), seu ViewModel e o XAML para o sistema de traducao dinamica.

**EN:** Set of scripts dedicated to migrating the main page (Dashboard), its ViewModel, and the XAML to the dynamic translation system.

**ES:** Conjunto de scripts dedicados a migrar la pagina principal (Dashboard), su ViewModel y el XAML al sistema de traduccion dinamica.

---

## 6. update_settings.js / update_settings_vm.js

**PT:** Migra a pagina de Configuracoes e seu ViewModel, convertendo todos os textos estaticos para chaves de traducao.

**EN:** Migrates the Settings page and its ViewModel, converting all static texts to translation keys.

**ES:** Migra la pagina de Configuracion y su ViewModel, convirtiendo todos los textos estaticos en claves de traduccion.

---

## 7. update_json1.js / update_prompts_temp.js

**PT:** Adiciona chaves iniciais de traducao nos arquivos JSON (pt-BR, en-US, es-ES) e popula as entradas da pagina de Prompts.

**EN:** Adds initial translation keys to the JSON files (pt-BR, en-US, es-ES) and populates Prompts page entries.

**ES:** Agrega claves de traduccion iniciales a los archivos JSON (pt-BR, en-US, es-ES) y completa las entradas de la pagina de Prompts.

---

## 8. add_search_type_keys.js

**PT:** Adiciona chaves de traducao para os tipos de resultado da Pesquisa Universal (Configuracao, Arquivo, Calculo, Terminal, Aplicativo).

**EN:** Adds translation keys for Universal Search result types (Setting, File, Calculation, Terminal, App).

**ES:** Agrega claves de traduccion para los tipos de resultado de la Busqueda Universal (Configuracion, Archivo, Calculo, Terminal, Aplicacion).

---

## 9. add_settings_keys.js

**PT:** Popula as chaves de traducao dos paineis de configuracoes do Windows (ms-settings) nos tres idiomas suportados.

**EN:** Populates translation keys for Windows settings panels (ms-settings) in the three supported languages.

**ES:** Completa las claves de traduccion de los paneles de configuracion de Windows (ms-settings) en los tres idiomas admitidos.

---

## 10. add_timer_keys.js / add_timer_keys2.js

**PT:** Adiciona/corrige chaves de traducao relacionadas aos temporizadores e ao metodo Pomodoro.

**EN:** Adds/fixes translation keys related to timers and the Pomodoro method.

**ES:** Agrega/corrige claves de traduccion relacionadas con los temporizadores y el metodo Pomodoro.

---

## 11. add_qa_title.js

**PT:** Adiciona chaves de titulo para a area de Perguntas e Respostas (QA) nos arquivos de traducao.

**EN:** Adds title keys for the Questions and Answers (QA) area in the translation files.

**ES:** Agrega claves de titulo para el area de Preguntas y Respuestas (QA) en los archivos de traduccion.

---

## 12. add_xmlns.js

**PT:** Injeta namespaces XML (xmlns) ausentes nos arquivos XAML que precisavam dos helpers de traducao.

**EN:** Injects missing XML namespaces (xmlns) into XAML files that needed translation helpers.

**ES:** Inyecta namespaces XML (xmlns) faltantes en los archivos XAML que necesitaban helpers de traduccion.

---

## 13. update_brl_keys.js / update_yearly_keys.js

**PT:** Atualiza as chaves de preco nos tres idiomas, adicionando o sufixo BRL e corrigindo os textos dos planos Mensal e Anual.

**EN:** Updates price keys in the three languages, adding the BRL suffix and correcting the Monthly and Yearly plan texts.

**ES:** Actualiza las claves de precio en los tres idiomas, agregando el sufijo BRL y corrigiendo los textos de los planes Mensual y Anual.

---

## 14. extract_settings.js

**PT:** Extrai as configuracoes do XAML original e as exporta em formato estruturado para facilitar a criacao das chaves de traducao.

**EN:** Extracts settings from the original XAML and exports them in a structured format to facilitate the creation of translation keys.

**ES:** Extrae las configuraciones del XAML original y las exporta en formato estructurado para facilitar la creacion de claves de traduccion.

---

## 15. fix_auth_spaces.js

**PT:** Corrige o espacamento entre os elementos Run do XAML na pagina de autenticacao, inserindo Run Text=" " entre textos em negrito e normais.

**EN:** Fixes spacing between Run elements in the authentication page XAML, inserting Run Text=" " between bold and normal text.

**ES:** Corrige el espaciado entre elementos Run del XAML en la pagina de autenticacion, insertando Run Text=" " entre textos en negrita y normales.

---

## 16. fix_duplicates.js

**PT:** Remove entradas duplicadas nos arquivos JSON de traducao, garantindo que cada chave apareca apenas uma vez.

**EN:** Removes duplicate entries from the JSON translation files, ensuring each key appears only once.

**ES:** Elimina entradas duplicadas en los archivos JSON de traduccion, asegurando que cada clave aparezca solo una vez.

---

## 17. inject_xaml_keys.js

**PT:** Injeta as chamadas ao LocalizationHelper.GetString() nos arquivos XAML code-behind (.xaml.cs) para todas as propriedades que precisavam de traducao dinamica.

**EN:** Injects LocalizationHelper.GetString() calls into the XAML code-behind files (.xaml.cs) for all properties that needed dynamic translation.

**ES:** Inyecta llamadas a LocalizationHelper.GetString() en los archivos code-behind XAML (.xaml.cs) para todas las propiedades que necesitaban traduccion dinamica.

---

## 18. report_translate_keys.js

**PT:** Gera um relatorio auditando o projeto inteiro, listando quais chaves de traducao existem, quais estao faltando e quais estao inconsistentes entre os tres idiomas.

**EN:** Generates a report auditing the entire project, listing which translation keys exist, which are missing, and which are inconsistent across the three languages.

**ES:** Genera un informe auditando todo el proyecto, listando que claves de traduccion existen, cuales faltan y cuales son inconsistentes entre los tres idiomas.

---

## 19. fix_todos.ps1

**PT:** Script PowerShell que percorre os arquivos .xaml.cs buscando comentarios TODO: Implement translation e substitui com a chamada correta ao LocalizationHelper.

**EN:** PowerShell script that iterates through .xaml.cs files looking for TODO: Implement translation comments and replaces them with the correct LocalizationHelper call.

**ES:** Script de PowerShell que recorre los archivos .xaml.cs buscando comentarios TODO: Implement translation y los reemplaza con la llamada correcta a LocalizationHelper.

---

> Todos os scripts foram executados manualmente durante a refatoracao e nao sao parte do build do aplicativo.
> All scripts were executed manually during the refactoring and are not part of the app build.
> Todos los scripts fueron ejecutados manualmente durante la refactorizacion y no son parte del build de la aplicacion.
