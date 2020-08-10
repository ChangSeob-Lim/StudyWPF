CREATE DATABASE  IF NOT EXISTS `iot_sensordata` /*!40100 DEFAULT CHARACTER SET utf8 */ /*!80016 DEFAULT ENCRYPTION='N' */;
USE `iot_sensordata`;

DROP TABLE IF EXISTS `sensordatatbl`;

CREATE TABLE `sensordatatbl` (
  `id` bigint NOT NULL AUTO_INCREMENT,
  `Date` datetime NOT NULL,
  `Value` int NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=194 DEFAULT CHARSET=utf8;

CREATE TABLE `arduinodatatbl` (
  `id` bigint NOT NULL AUTO_INCREMENT,
  `Date` datetime NOT NULL,
  `PhotoValue` int NOT NULL,
  `TempValue` double NOT NULL,
  `HumiValue` double NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=194 DEFAULT CHARSET=utf8;