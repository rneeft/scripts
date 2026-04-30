OH_MY_POSH="$HOME/apps/ohmyposh"
mkdir -p "$OH_MY_POSH"

curl -o "$OH_MY_POSH/template.json" \
  https://scripts.rickneeft.dev/ubuntu/ohmyposh-template.json

echo 'export PATH="$HOME/.local/bin:$PATH"' >> ~/.bashrc
echo 'eval "$(oh-my-posh init bash --config ~/apps/ohmyposh/template.json)"' >> ~/.bashrc

curl -s https://ohmyposh.dev/install.sh | bash -s
export PATH=$PATH:$HOME/.local/bin
oh-my-posh font install meslo
