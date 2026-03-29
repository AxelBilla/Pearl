#!/bin/bash
origin="$(realpath $0)"

db_name=$1

sudo -u postgres psql -d $db_name -c "CREATE TABLE Users(
   ID VARCHAR(50),
   Username VARCHAR(25),
   Password VARCHAR(50),
   PRIMARY KEY(ID),
   UNIQUE(Username)
);"
sudo -u postgres psql -d $db_name -c "ALTER TABLE Users OWNER TO $db_name"

sudo -u postgres psql -d $db_name -c "CREATE TABLE Admins(
   ID VARCHAR(50),
   PRIMARY KEY(ID),
   FOREIGN KEY(ID) REFERENCES Users(ID)
);"
sudo -u postgres psql -d $db_name -c "ALTER TABLE Admins OWNER TO $db_name"

sudo -u postgres psql -d $db_name -c "CREATE TABLE Categories(
   ID VARCHAR(50),
   Name VARCHAR(25),
   PRIMARY KEY(ID),
   UNIQUE(Name)
);"
sudo -u postgres psql -d $db_name -c "ALTER TABLE Categories OWNER TO $db_name"

sudo -u postgres psql -d $db_name -c "CREATE TABLE Sources(
   ID VARCHAR(50),
   Name VARCHAR(25),
   Score INT,
   PRIMARY KEY(ID),
   UNIQUE(Name)
);"
sudo -u postgres psql -d $db_name -c "ALTER TABLE Sources OWNER TO $db_name"

sudo -u postgres psql -d $db_name -c "CREATE TABLE Messages(
   ID VARCHAR(50),
   Content VARCHAR(1000),
   Timestamp Timestamp,
   last_update Timestamp,
   user_id VARCHAR(50) NOT NULL,
   PRIMARY KEY(ID),
   FOREIGN KEY(user_id) REFERENCES Users(ID)
);"
sudo -u postgres psql -d $db_name -c "ALTER TABLE Messages OWNER TO $db_name"

sudo -u postgres psql -d $db_name -c "CREATE TABLE Articles(
   ID VARCHAR(50),
   Content VARCHAR(1000) NOT NULL,
   Timestamp Timestamp,
   admin_id VARCHAR(50) NOT NULL,
   category_id VARCHAR(50) NOT NULL,
   PRIMARY KEY(ID),
   FOREIGN KEY(admin_id) REFERENCES Admins(ID),
   FOREIGN KEY(category_id) REFERENCES Categories(ID)
);"
sudo -u postgres psql -d $db_name -c "ALTER TABLE Articles OWNER TO $db_name"

sudo -u postgres psql -d $db_name -c "CREATE TABLE Links(
   article_id VARCHAR(50),
   source_id VARCHAR(50),
   Link VARCHAR(250),
   PRIMARY KEY(article_id, source_id),
   FOREIGN KEY(article_id) REFERENCES Articles(ID),
   FOREIGN KEY(source_id) REFERENCES Sources(ID)
);"
sudo -u postgres psql -d $db_name -c "ALTER TABLE Links OWNER TO $db_name"

sudo -u postgres psql -d $db_name -c "INSERT INTO Users VALUES(0, 'admin', 'admin')"
sudo -u postgres psql -d $db_name -c "INSERT INTO Admins VALUES(0)"

sudo rm -- $origin
