# Checklist de Validación - Documentación

**ANTES de cada cambio en un `.md`, comprueba TODOS estos puntos:**

## Formato

- [ ] **TOC**: Todos los `##` y `###` del cuerpo aparecen en el TOC
- [ ] **TOC**: `Resumen` NO aparece en el TOC
- [ ] **Separadores**: `---` SOLO después del TOC (antes del `#`) y antes de `**Resumen del punto:**`
- [ ] **Separadores**: NUNCA `---` antes de `##` o `###`
- [ ] **Resumen**: Es `**Resumen del punto:**` (negrita), NUNCA `## XX. Resumen`
- [ ] **Resumen**: Termina con `**¿Qué viene después?**`
- [ ] **Punto de partida**: Empieza con `> **Punto de partida:**` o `> 💡 **Punto de partida:**`

## Mermaid

- [ ] SOLO colores de paleta: `#4CAF50` `#f44336` `#2196F3` `#FF9800` `#9C27B0` `#607D8B`
- [ ] SIEMPRE `color:#fff` en todos los estilos
- [ ] NUNCA colores claros: `#e1f5ff`, `#e1ffe1`, `#1B5E20`, `#0D47A1`, `#E65100`, `#C62828`, `#1565C0`, `#1976D2`, `#2E7D32`, `#388E3C`, `#43A047`, `#B71C1C`, `#D32F2F`, `#6A1B9A`, `#7c3aed`, `#8BC34A`, `#FFEB3B`, `#00BCD4`, `#795548`
- [ ] NUNCA `%%` como nodo (solo comentarios)
- [ ] Nodos con texto descriptivo en español

## Contenido

- [ ] **Punto de partida**: Al inicio, antes del primer `##`
- [ ] **📌 Ejemplo real**: Al menos uno por documento
- [ ] **❌/✅**: Al menos un ejemplo de código bueno/malo
- [ ] **Buenas Prácticas**: Sección presente
- [ ] **Reto**: Menciona FunkoApp/Funkos (no genérico "productos")
- [ ] **Cada sección**: Tiene texto explicativo antes del código

## Idioma

- [ ] Sin emojis en títulos (`#`, `##`, `###`)
- [ ] Español de España en documentación
- [ ] Inglés en identificadores C#
