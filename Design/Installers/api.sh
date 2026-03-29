#!/bin/bash
origin="$(realpath $0)"

db_name=$1
db_password=$1

sudo apt install git && sudo apt install npm && sudo apt install nodejs
sudo git clone --filter=blob:none --sparse https://github.com/AxelBilla/Pearl.git
cd Pearl
sudo git sparse-checkout set API
cd API
sudo npm install
cd Pearl
mkdir .env
cd .env
printf "{\"name\": \"$db_name\", \"password\": \"$db_password\"}" >> db.env.json

sudo rm -- $origin

