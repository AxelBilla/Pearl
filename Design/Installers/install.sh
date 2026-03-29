#!/bin/bash
origin="$(realpath $0)"

sudo apt update && sudo apt upgrade
sudo apt install postgresql

clear
echo "[DATABASE - ADMIN SETUP]"
read -p "NAME: " db_name;
read -p "PASSWORD: " db_password;

sudo useradd $db_name
sudo -u postgres psql -c "CREATE USER $db_name PASSWORD '$db_password';"
sudo -u postgres psql -c "CREATE DATABASE $db_name OWNER $db_name;"

sudo bash db.sh $db_name;

clear
read -p "Include API (Y/n): " yn;

if [ "$yn" != "${yn#[Yy]}" ] ;then  
    sudo bash api.sh $db_name $db_password;
else
    exit
fi

sudo bash start.sh

sudo rm -- $origin
