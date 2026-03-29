# EmuSync Agent - Instalador de servicio Windows
# Ejecutar como Administrador

param(
    [string]$AgentPath = "$PSScriptRoot\EmuSync.Agent.exe"
)

$ServiceName = "EmuSyncAgent"
$ServiceDisplayName = "EmuSync Agent"
$ServiceDescription = "EmuSync background sync agent"

# Verificar que el ejecutable existe
if (-not (Test-Path $AgentPath)) {
    Write-Error "No se encontró el ejecutable en: $AgentPath"
    Write-Host "Descárgalo desde GitHub Actions y colócalo en la misma carpeta que este script."
    exit 1
}

# Detener y eliminar servicio anterior si existe
$existing = Get-Service -Name $ServiceName -ErrorAction SilentlyContinue
if ($existing) {
    Write-Host "Eliminando servicio anterior..."
    Stop-Service -Name $ServiceName -Force -ErrorAction SilentlyContinue
    sc.exe delete $ServiceName | Out-Null
    Start-Sleep -Seconds 2
}

# Registrar el nuevo servicio
Write-Host "Registrando servicio..."
New-Service `
    -Name $ServiceName `
    -DisplayName $ServiceDisplayName `
    -Description $ServiceDescription `
    -BinaryPathName "`"$AgentPath`"" `
    -StartupType Automatic

# Arrancar el servicio
Write-Host "Arrancando servicio..."
Start-Service -Name $ServiceName

$status = Get-Service -Name $ServiceName
Write-Host "Estado: $($status.Status)"
Write-Host ""
Write-Host "¡Listo! El agente EmuSync corre ahora como servicio Windows."
Write-Host "Puedes gestionarlo desde services.msc o con:"
Write-Host "  Start-Service EmuSyncAgent"
Write-Host "  Stop-Service EmuSyncAgent"
