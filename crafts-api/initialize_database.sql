DROP DATABASE IF EXISTS `crafts`;
CREATE DATABASE `crafts`;

DROP USER IF EXISTS 'crafts_user'@'localhost';
CREATE USER 'crafts_user'@'localhost' IDENTIFIED WITH mysql_native_password BY 'crafts';
GRANT ALL PRIVILEGES ON `crafts`.* TO 'crafts_user'@'localhost';
