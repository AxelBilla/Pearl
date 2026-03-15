CREATE TABLE Users(
   ID VARCHAR(50),
   username VARCHAR(25),
   password VARCHAR(50),
   PRIMARY KEY(ID)
);

CREATE TABLE Admins(
   ID VARCHAR(50),
   PRIMARY KEY(ID),
   FOREIGN KEY(ID) REFERENCES Users(ID)
);

CREATE TABLE Messages(
   ID VARCHAR(50),
   content VARCHAR(1000),
   timestamp Timestamp,
   last_update Timestamp,
   user_id VARCHAR(50) NOT NULL,
   PRIMARY KEY(ID),
   FOREIGN KEY(user_id) REFERENCES Users(ID)
);
