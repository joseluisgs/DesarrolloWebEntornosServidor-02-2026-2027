<#
.SYNOPSIS
    Valida documentos .md contra las reglas de AGENTS.md
.USAGE
    .\Validate-Docs.ps1                    # Validar todos los .md de UD02
    .\Validate-Docs.ps1 -File 19-graphql.md  # Validar uno solo
#>

param(
    [string]$Path = ".",
    [string]$File = ""
)

# Colores prohibidos en Mermaid
$ForbiddenColors = @(
    "#e1f5ff","#e1ffe1","#1B5E20","#0D47A1","#E65100","#C62828",
    "#1565C0","#1976D2","#2E7D32","#388E3C","#43A047","#B71C1C",
    "#D32F2F","#6A1B9A","#7c3aed","#8BC34A","#FFEB3B","#00BCD4","#795548"
)

function Test-Document {
    param([string]$FilePath)

    $content = [System.IO.File]::ReadAllText($FilePath, [System.Text.Encoding]::UTF8)
    $lines = [System.IO.File]::ReadAllLines($FilePath, [System.Text.Encoding]::UTF8)
    $filename = Split-Path $FilePath -Leaf
    $fileErrors = @()
    $fileWarnings = @()

    # 1. TOC: Resumen no debe estar en el TOC
    $tocResumen = $lines | Where-Object { $_ -match '^\s+-\s+\[' -and $_ -match 'Resumen' }
    if ($tocResumen) {
        $fileErrors += "TOC: 'Resumen' aparece en el TOC (debe eliminarse)"
    }

    # 2. Separadores: NO --- antes de ## o ###
    for ($i = 0; $i -lt $lines.Count; $i++) {
        if ($lines[$i].Trim() -eq "---") {
            for ($j = $i + 1; $j -lt [Math]::Min($i + 5, $lines.Count); $j++) {
                $next = $lines[$j].Trim()
                if ($next -ne "") {
                    if ($next -match '^##') {
                        $fileErrors += "Separador: --- antes de heading '$next' (linea $($j+1))"
                    }
                    break
                }
            }
        }
    }

    # 3. Resumen: debe ser **Resumen del punto:** (no ##)
    if ($content -match '(?m)^##\s+\d+[\.\d]*\s+Resumen' -and $content -notmatch '\*\*Resumen del punto:\*\*') {
        $fileErrors += "Resumen: Usa '## X. Resumen' en vez de '**Resumen del punto:**'"
    }

    # 4. Resumen: debe tener Que viene despues
    if ($content -match 'Resumen del punto' -and $content -notmatch 'viene despue') {
        $fileErrors += "Resumen: Falta '**Que viene despues?**'"
    }

    # 5. Punto de partida
    if ($content -notmatch 'Punto de partida') {
        $fileErrors += "Contenido: Falta 'Punto de partida'"
    }

    # 6. Colores Mermaid prohibidos
    $styleLines = $lines | Where-Object { $_ -match 'style.*fill:#' }
    foreach ($line in $styleLines) {
        foreach ($color in $ForbiddenColors) {
            if ($line -match [regex]::Escape($color)) {
                $lineNum = [Array]::IndexOf($lines, $line) + 1
                $fileErrors += "Mermaid: Color prohibido '$color' (linea $lineNum)"
            }
        }
        if ($line -notmatch 'color:#fff') {
            $lineNum = [Array]::IndexOf($lines, $line) + 1
            $fileErrors += "Mermaid: Falta 'color:#fff' (linea $lineNum)"
        }
    }

    # 7. Ejemplo real
    if ($content -notmatch 'Ejemplo real') {
        $fileWarnings += "Contenido: No hay 'Ejemplo real'"
    }

    # 8. Buenas Practicas
    if ($content -notmatch 'Buenas Pr' -and $content -notmatch 'Buenas pr') {
        $fileWarnings += "Contenido: Falta seccion 'Buenas Practicas'"
    }

    # 9. Reto FunkoApp
    if ($content -match 'Reto') {
        $retoMatch = [regex]::Match($content, '(?s)##\s+\d+[\.\d]*\s+Reto.{0,500}')
        if ($retoMatch.Success -and $retoMatch.Value -notmatch 'Funko' -and $retoMatch.Value -notmatch 'funko') {
            $fileWarnings += "Reto: No menciona FunkoApp/Funkos"
        }
    }

    # 10. Emojis en titulos (simplificado - buscar simbolos Unicode comunes)
    $titleLines = $lines | Where-Object { $_ -match '^#{1,3}\s' }
    foreach ($line in $titleLines) {
        if ($line -match '[\u2600-\u27BF]|[\u2702-\u27B0]|[\u2460-\u24FF]|[\u2190-\u21FF]|[\u2300-\u23FF]|[\u25A0-\u25FF]|[\u2600-\u26FF]') {
            $fileWarnings += "Emojis: Emoji en titulo '$($line.Trim())'"
        }
    }

    return @{ Errors = $fileErrors; Warnings = $fileWarnings }
}

# Obtener archivos
if ($File) {
    $files = @((Join-Path $Path $File))
} else {
    $files = Get-ChildItem -Path $Path -Filter "*.md" |
        Where-Object { $_.Name -match '^\d{2}-' } |
        Select-Object -ExpandProperty FullName
}

Write-Host "`n=== Validacion de Documentos ===" -ForegroundColor Cyan
Write-Host "Archivos: $($files.Count)`n"

$totalErrors = 0
$totalWarnings = 0

foreach ($file in $files) {
    $result = Test-Document -FilePath $file
    $filename = Split-Path $file -Leaf

    if ($result.Errors.Count -gt 0) {
        Write-Host "FAIL $filename" -ForegroundColor Red
        foreach ($err in $result.Errors) {
            Write-Host "  [ERROR] $err" -ForegroundColor Red
            $totalErrors++
        }
    }

    if ($result.Warnings.Count -gt 0) {
        Write-Host "WARN $filename" -ForegroundColor Yellow
        foreach ($warn in $result.Warnings) {
            Write-Host "  [WARN] $warn" -ForegroundColor Yellow
            $totalWarnings++
        }
    }

    if ($result.Errors.Count -eq 0 -and $result.Warnings.Count -eq 0) {
        Write-Host "OK   $filename" -ForegroundColor Green
    }
}

Write-Host "`n=== Resultado ===" -ForegroundColor Cyan
Write-Host "Errores: $totalErrors" -ForegroundColor $(if ($totalErrors -gt 0) { "Red" } else { "Green" })
Write-Host "Advertencias: $totalWarnings" -ForegroundColor $(if ($totalWarnings -gt 0) { "Yellow" } else { "Green" })
