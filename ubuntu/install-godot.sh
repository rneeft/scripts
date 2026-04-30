GODOT_DIR="$HOME/apps/godot"
mkdir -p "$GODOT_DIR"

echo "⬇️ Fetching latest Godot Mono release..."

API_URL="https://api.github.com/repos/godotengine/godot/releases/latest"

DOWNLOAD_URL=$(curl -s "$API_URL" | jq -r \
  '.assets[] | select(.name | test("mono.*linux.*x86_64.*zip")) | .browser_download_url' | head -n 1)

if [ -z "$DOWNLOAD_URL" ] || [ "$DOWNLOAD_URL" = "null" ]; then
  echo "❌ Could not find Godot Mono Linux release"
  exit 1
fi

curl -L "$DOWNLOAD_URL" -o /tmp/godot.zip
unzip -o /tmp/godot.zip -d "$GODOT_DIR"

GODOT_BIN=$(find "$GODOT_DIR" -type f -name "Godot_v*_mono*" | head -n 1)

chmod +x "$GODOT_BIN"
ln -sf "$GODOT_BIN" "$GODOT_DIR/godot"

#############################################
# GODOT DESKTOP ENTRY
#############################################

mkdir -p ~/.local/share/applications

cat > ~/.local/share/applications/godot.desktop <<EOF
[Desktop Entry]
Version=1.0
Type=Application
Name=Godot Engine
Comment=Game Engine
Exec=$HOME/apps/godot/godot
Icon=$HOME/apps/godot/godot.png
Terminal=false
Categories=Development;IDE;
StartupNotify=true
EOF

chmod +x ~/.local/share/applications/godot.desktop

#############################################
# GODOT ICON DOWNLOAD
#############################################

curl -L https://www.rickneeft.dev/assets/img/godot.png \
  -o "$GODOT_DIR/godot.png"
