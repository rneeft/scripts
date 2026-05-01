git config --global user.name "Rick Neeft"
git config --global user.email "rickneeft@outlook.com"

flatpak remote-add --if-not-exists flathub https://flathub.org/repo/flathub.flatpakrepo
flatpak install flathub org.gnome.Boxes

curl -sSL https://scripts.rickneeft.dev/ubuntu/ohmyposh.sh | bash

curl -sSL https://scripts.rickneeft.dev/ubuntu/install-godot.sh | bash

curl -sSL https://scripts.rickneeft.dev/ubuntu/docker.sh | sudo bash

curl -sSL https://aspire.dev/install.sh | bash
