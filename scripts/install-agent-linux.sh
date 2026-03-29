#!/bin/bash
# EmuSync Agent - Instalador para SteamOS/Linux

set -e

AGENT_DIR="$HOME/.local/share/emusync-agent"
AGENT_BIN="$AGENT_DIR/EmuSync.Agent"
SERVICE_NAME="emusync-agent"
SERVICE_FILE="$HOME/.config/systemd/user/$SERVICE_NAME.service"
SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
SOURCE_BIN="$SCRIPT_DIR/EmuSync.Agent"

echo "=== EmuSync Agent - Instalador Linux ==="

# Verificar que el ejecutable existe
if [ ! -f "$SOURCE_BIN" ]; then
    echo "ERROR: No se encontró EmuSync.Agent en $SCRIPT_DIR"
    echo "Descárgalo desde GitHub Actions y colócalo junto a este script."
    exit 1
fi

# Detener servicio si está corriendo
echo "Deteniendo servicio anterior (si existe)..."
systemctl --user stop "$SERVICE_NAME" 2>/dev/null || true
systemctl --user disable "$SERVICE_NAME" 2>/dev/null || true

# Copiar ejecutable
echo "Instalando ejecutable en $AGENT_DIR..."
mkdir -p "$AGENT_DIR"
cp "$SOURCE_BIN" "$AGENT_BIN"
chmod +x "$AGENT_BIN"

# Crear archivo de servicio systemd (usuario, sin necesitar sudo)
echo "Creando servicio systemd..."
mkdir -p "$(dirname "$SERVICE_FILE")"

cat > "$SERVICE_FILE" << EOF
[Unit]
Description=EmuSync Agent
After=network.target

[Service]
Type=simple
ExecStart=$AGENT_BIN
WorkingDirectory=$AGENT_DIR
Restart=on-failure
RestartSec=10
StandardOutput=journal
StandardError=journal

[Install]
WantedBy=default.target
EOF

# Activar e iniciar el servicio
echo "Activando servicio..."
systemctl --user daemon-reload
systemctl --user enable "$SERVICE_NAME"
systemctl --user start "$SERVICE_NAME"

# Habilitar linger para que el servicio arranque sin login (importante en Steam Deck Gaming Mode)
loginctl enable-linger "$USER" 2>/dev/null || true

echo ""
echo "=== Instalación completada ==="
echo "Estado del servicio:"
systemctl --user status "$SERVICE_NAME" --no-pager
echo ""
echo "Comandos útiles:"
echo "  systemctl --user status emusync-agent   # ver estado"
echo "  systemctl --user stop emusync-agent     # parar"
echo "  systemctl --user restart emusync-agent  # reiniciar"
echo "  journalctl --user -u emusync-agent -f   # ver logs"
