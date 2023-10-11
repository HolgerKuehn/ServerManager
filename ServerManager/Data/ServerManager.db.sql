BEGIN TRANSACTION;
DROP TABLE IF EXISTS "DynDnsDomainUpdateUri";
CREATE TABLE IF NOT EXISTS "DynDnsDomainUpdateUri" (
	"DynDnsDomain_ID"	INTEGER NOT NULL,
	"DynDnsDomainUpdateUri_ID"	INTEGER NOT NULL,
	"DynDnsDomainUpdateUri_AddressFamily"	INTEGER NOT NULL,
	"DynDnsDomainUpdateUri_Uri"	TEXT NOT NULL,
	PRIMARY KEY("DynDnsDomainUpdateUri_ID" AUTOINCREMENT)
);
DROP TABLE IF EXISTS "DynDnsDomain";
CREATE TABLE IF NOT EXISTS "DynDnsDomain" (
	"Configuration_ID"	INTEGER,
	"DynDnsDomain_ID"	INTEGER NOT NULL,
	"DynDnsDomain_Name"	TEXT NOT NULL,
	"DynDnsDomain_User"	TEXT NOT NULL,
	"DynDnsDomain_Password"	TEXT NOT NULL,
	PRIMARY KEY("DynDnsDomain_ID" AUTOINCREMENT)
);
DROP TABLE IF EXISTS "DynDnsServiceType";
CREATE TABLE IF NOT EXISTS "DynDnsServiceType" (
	"DynDnsServiceType_ID"	INTEGER NOT NULL,
	"DynDnsServiceType_Name"	TEXT NOT NULL,
	PRIMARY KEY("DynDnsServiceType_ID")
);
DROP TABLE IF EXISTS "DynDnsService";
CREATE TABLE IF NOT EXISTS "DynDnsService" (
	"DynDnsDomain_ID"	INTEGER NOT NULL,
	"DynDnsService_ID"	INTEGER NOT NULL,
	"DynDnsService_Name"	TEXT NOT NULL,
	"DynDnsServiceType_ID"	INTEGER NOT NULL,
	PRIMARY KEY("DynDnsService_ID" AUTOINCREMENT)
);
DROP TABLE IF EXISTS "Log";
CREATE TABLE IF NOT EXISTS "Log" (
	"Log_ID"	INTEGER NOT NULL,
	"Log_Timestamp"	INTEGER NOT NULL,
	"LogSeverity_ID"	INTEGER NOT NULL,
	"LogOrigin_ID"	INTEGER NOT NULL,
	"Log_Message"	TEXT NOT NULL,
	PRIMARY KEY("Log_ID" AUTOINCREMENT)
);
DROP TABLE IF EXISTS "LogOrigin";
CREATE TABLE IF NOT EXISTS "LogOrigin" (
	"LogOrigin_ID"	INTEGER NOT NULL,
	"LogOrigin_Class"	TEXT NOT NULL,
	"LogOrigin_Function"	TEXT NOT NULL,
	"LogOrigin_Step"	TEXT NOT NULL,
	PRIMARY KEY("LogOrigin_ID")
);
DROP TABLE IF EXISTS "LogSeverity";
CREATE TABLE IF NOT EXISTS "LogSeverity" (
	"LogSeverity_ID"	INTEGER NOT NULL,
	"LogSeverity_Name"	TEXT NOT NULL UNIQUE,
	PRIMARY KEY("LogSeverity_ID")
);
DROP TABLE IF EXISTS "Configuration";
CREATE TABLE IF NOT EXISTS "Configuration" (
	"Configuration_ID"	INTEGER NOT NULL,
	"Configuration_ServerName"	VARCHAR(255) NOT NULL UNIQUE,
	"Configuration_MinimumLogSeverity"	INTEGER NOT NULL DEFAULT '2',
	PRIMARY KEY("Configuration_ID")
);
DROP TABLE IF EXISTS "DynDnsIpAddressType";
CREATE TABLE IF NOT EXISTS "DynDnsIpAddressType" (
	"DynDnsIpAddressType_ID"	INTEGER NOT NULL,
	"DynDnsIpAddressType_Name"	TEXT,
	PRIMARY KEY("DynDnsIpAddressType_ID")
);
DROP TABLE IF EXISTS "DynDnsIpAddressVersion";
CREATE TABLE IF NOT EXISTS "DynDnsIpAddressVersion" (
	"DynDnsIpAddressVersion_ID"	INTEGER NOT NULL,
	"DynDnsIpAddressVersion_Name"	TEXT,
	PRIMARY KEY("DynDnsIpAddressVersion_ID")
);
DROP TABLE IF EXISTS "DynDnsIpAddress";
CREATE TABLE IF NOT EXISTS "DynDnsIpAddress" (
	"DynDnsIpAddressReferenceType_ID"	INTEGER,
	"DynDnsIpAddressReference_ID"	INTEGER,
	"DynDnsIpAddress_ID"	INTEGER NOT NULL,
	"DynDnsIpAddressType_ID"	INTEGER NOT NULL,
	"DynDnsIpAddressVersion_ID"	INTEGER NOT NULL,
	"DynDnsIpAddress_Name"	TEXT NOT NULL,
	"DynDnsIpAddress_Changed"	INTEGER NOT NULL,
	PRIMARY KEY("DynDnsIpAddress_ID" AUTOINCREMENT)
);
DROP TABLE IF EXISTS "DynDnsIpAddressReferenceType";
CREATE TABLE IF NOT EXISTS "DynDnsIpAddressReferenceType" (
	"DynDnsIpAddressReferenceType_ID"	INTEGER NOT NULL,
	"DynDnsIpAddressReferenceType_Name"	TEXT NOT NULL,
	PRIMARY KEY("DynDnsIpAddressReferenceType_ID")
);
DROP TABLE IF EXISTS "Command";
CREATE TABLE IF NOT EXISTS "Command" (
	"Command_ID"	INTEGER NOT NULL,
	"Command_Class"	TEXT,
	"Command_Function"	TEXT,
	"Command_Step"	TEXT,
	"CommandType_ID"	INTEGER,
	"Command_Name"	TEXT NOT NULL,
	PRIMARY KEY("Command_ID")
);
DROP TABLE IF EXISTS "CommandType";
CREATE TABLE IF NOT EXISTS "CommandType" (
	"CommandType_ID"	INTEGER,
	"CommandType_Name"	TEXT
);
INSERT INTO "DynDnsDomainUpdateUri" ("DynDnsDomain_ID","DynDnsDomainUpdateUri_ID","DynDnsDomainUpdateUri_AddressFamily","DynDnsDomainUpdateUri_Uri") VALUES (1,1,1,'https://ddns.dachs.blog/update.php?user=<user>&password=<password>&domain=<servicename>&ipv4=<ip4addr>');
INSERT INTO "DynDnsDomainUpdateUri" ("DynDnsDomain_ID","DynDnsDomainUpdateUri_ID","DynDnsDomainUpdateUri_AddressFamily","DynDnsDomainUpdateUri_Uri") VALUES (1,2,2,'https://ddns.dachs.blog/update.php?user=<user>&password=<password>&domain=<servicename>&ipv4=<ip6addr>');
INSERT INTO "DynDnsDomainUpdateUri" ("DynDnsDomain_ID","DynDnsDomainUpdateUri_ID","DynDnsDomainUpdateUri_AddressFamily","DynDnsDomainUpdateUri_Uri") VALUES (2,3,1,'https://dyndns.strato.com/nic/update?system=dyndns&hostname=<servicename>&offline=NO');
INSERT INTO "DynDnsDomainUpdateUri" ("DynDnsDomain_ID","DynDnsDomainUpdateUri_ID","DynDnsDomainUpdateUri_AddressFamily","DynDnsDomainUpdateUri_Uri") VALUES (2,4,2,'https://dyndns.strato.com/nic/update?system=dyndns&hostname=<servicename>&offline=NO');
INSERT INTO "DynDnsDomain" ("Configuration_ID","DynDnsDomain_ID","DynDnsDomain_Name","DynDnsDomain_User","DynDnsDomain_Password") VALUES (1,1,'dachs.blog','ZGRucy5kYWNocy5ibG9n','cnROaWpKaHA5ZVJmY1d5cGg1QjFjTUxCZElvUHhiMklXZE5SU3BGQmQ3NVVsblJ0d3drY0pJMmVRNm51TWVp');
INSERT INTO "DynDnsDomain" ("Configuration_ID","DynDnsDomain_ID","DynDnsDomain_Name","DynDnsDomain_User","DynDnsDomain_Password") VALUES (1,2,'phoebus.de','cGhvZWJ1cy5kZQ==','dFVWd0RWcHcxeWNkVktiaTFvbHk=');
INSERT INTO "DynDnsDomain" ("Configuration_ID","DynDnsDomain_ID","DynDnsDomain_Name","DynDnsDomain_User","DynDnsDomain_Password") VALUES (1,3,'profikita.de','','');
INSERT INTO "DynDnsServiceType" ("DynDnsServiceType_ID","DynDnsServiceType_Name") VALUES (1,'Server');
INSERT INTO "DynDnsServiceType" ("DynDnsServiceType_ID","DynDnsServiceType_Name") VALUES (2,'LocalService');
INSERT INTO "DynDnsServiceType" ("DynDnsServiceType_ID","DynDnsServiceType_Name") VALUES (3,'RemoteService');
INSERT INTO "DynDnsService" ("DynDnsDomain_ID","DynDnsService_ID","DynDnsService_Name","DynDnsServiceType_ID") VALUES (1,1,'dentaku',2);
INSERT INTO "DynDnsService" ("DynDnsDomain_ID","DynDnsService_ID","DynDnsService_Name","DynDnsServiceType_ID") VALUES (1,2,'ftp',2);
INSERT INTO "DynDnsService" ("DynDnsDomain_ID","DynDnsService_ID","DynDnsService_Name","DynDnsServiceType_ID") VALUES (1,3,'jellyfin',2);
INSERT INTO "DynDnsService" ("DynDnsDomain_ID","DynDnsService_ID","DynDnsService_Name","DynDnsServiceType_ID") VALUES (1,4,'keepass',2);
INSERT INTO "DynDnsService" ("DynDnsDomain_ID","DynDnsService_ID","DynDnsService_Name","DynDnsServiceType_ID") VALUES (1,5,'webdav',2);
INSERT INTO "DynDnsService" ("DynDnsDomain_ID","DynDnsService_ID","DynDnsService_Name","DynDnsServiceType_ID") VALUES (1,6,'hebisoochi',3);
INSERT INTO "DynDnsService" ("DynDnsDomain_ID","DynDnsService_ID","DynDnsService_Name","DynDnsServiceType_ID") VALUES (1,7,'kitsunesoochi',3);
INSERT INTO "DynDnsService" ("DynDnsDomain_ID","DynDnsService_ID","DynDnsService_Name","DynDnsServiceType_ID") VALUES (1,8,'torasoochi',3);
INSERT INTO "DynDnsService" ("DynDnsDomain_ID","DynDnsService_ID","DynDnsService_Name","DynDnsServiceType_ID") VALUES (1,9,'yusousoochi',3);
INSERT INTO "DynDnsService" ("DynDnsDomain_ID","DynDnsService_ID","DynDnsService_Name","DynDnsServiceType_ID") VALUES (1,10,'yobisoochi',3);
INSERT INTO "DynDnsService" ("DynDnsDomain_ID","DynDnsService_ID","DynDnsService_Name","DynDnsServiceType_ID") VALUES (2,11,'glomm',3);
INSERT INTO "DynDnsService" ("DynDnsDomain_ID","DynDnsService_ID","DynDnsService_Name","DynDnsServiceType_ID") VALUES (2,12,'hades',3);
INSERT INTO "DynDnsService" ("DynDnsDomain_ID","DynDnsService_ID","DynDnsService_Name","DynDnsServiceType_ID") VALUES (2,13,'knaack',3);
INSERT INTO "DynDnsService" ("DynDnsDomain_ID","DynDnsService_ID","DynDnsService_Name","DynDnsServiceType_ID") VALUES (2,14,'kuehn',2);
INSERT INTO "DynDnsService" ("DynDnsDomain_ID","DynDnsService_ID","DynDnsService_Name","DynDnsServiceType_ID") VALUES (2,15,'ak8-7490',3);
INSERT INTO "DynDnsService" ("DynDnsDomain_ID","DynDnsService_ID","DynDnsService_Name","DynDnsServiceType_ID") VALUES (3,16,'',3);
INSERT INTO "DynDnsService" ("DynDnsDomain_ID","DynDnsService_ID","DynDnsService_Name","DynDnsServiceType_ID") VALUES (1,17,'shinkansen',1);
INSERT INTO "LogOrigin" ("LogOrigin_ID","LogOrigin_Class","LogOrigin_Function","LogOrigin_Step") VALUES (1,'ProgramMain','Main','');
INSERT INTO "LogOrigin" ("LogOrigin_ID","LogOrigin_Class","LogOrigin_Function","LogOrigin_Step") VALUES (2,'ServerManagerGuiMain','ServerManagerGuiMain','');
INSERT INTO "LogOrigin" ("LogOrigin_ID","LogOrigin_Class","LogOrigin_Function","LogOrigin_Step") VALUES (3,'ServerManagerGuiMain','DataLogEntryOrigin','');
INSERT INTO "LogOrigin" ("LogOrigin_ID","LogOrigin_Class","LogOrigin_Function","LogOrigin_Step") VALUES (4,'GuiWindowLog','GuiWindowLog','');
INSERT INTO "LogOrigin" ("LogOrigin_ID","LogOrigin_Class","LogOrigin_Function","LogOrigin_Step") VALUES (5,'GuiWindowLog','GuiWindowLog_Shown','');
INSERT INTO "LogOrigin" ("LogOrigin_ID","LogOrigin_Class","LogOrigin_Function","LogOrigin_Step") VALUES (6,'GuiWindowLog','tmrGuiWindowLog_Tick','');
INSERT INTO "LogOrigin" ("LogOrigin_ID","LogOrigin_Class","LogOrigin_Function","LogOrigin_Step") VALUES (7,'GuiWindowLog','GuiWindowLog_VisibleChanged','');
INSERT INTO "LogOrigin" ("LogOrigin_ID","LogOrigin_Class","LogOrigin_Function","LogOrigin_Step") VALUES (8,'ThreadDynDns','ThreadDynDns','');
INSERT INTO "LogOrigin" ("LogOrigin_ID","LogOrigin_Class","LogOrigin_Function","LogOrigin_Step") VALUES (9,'DynDnsDomain','DynDnsDomain','');
INSERT INTO "LogOrigin" ("LogOrigin_ID","LogOrigin_Class","LogOrigin_Function","LogOrigin_Step") VALUES (10,'DynDnsServer','DynDnsServer','');
INSERT INTO "LogOrigin" ("LogOrigin_ID","LogOrigin_Class","LogOrigin_Function","LogOrigin_Step") VALUES (11,'DynDnsNetworkObject','DynDnsNetworkObject','');
INSERT INTO "LogOrigin" ("LogOrigin_ID","LogOrigin_Class","LogOrigin_Function","LogOrigin_Step") VALUES (12,'DynDnsNetworkObject','GetIpAddress','');
INSERT INTO "LogSeverity" ("LogSeverity_ID","LogSeverity_Name") VALUES (1,'Debug');
INSERT INTO "LogSeverity" ("LogSeverity_ID","LogSeverity_Name") VALUES (2,'Informational');
INSERT INTO "LogSeverity" ("LogSeverity_ID","LogSeverity_Name") VALUES (4,'Notice');
INSERT INTO "LogSeverity" ("LogSeverity_ID","LogSeverity_Name") VALUES (8,'Warning');
INSERT INTO "LogSeverity" ("LogSeverity_ID","LogSeverity_Name") VALUES (16,'Error');
INSERT INTO "LogSeverity" ("LogSeverity_ID","LogSeverity_Name") VALUES (32,'Critical');
INSERT INTO "LogSeverity" ("LogSeverity_ID","LogSeverity_Name") VALUES (64,'Alert');
INSERT INTO "LogSeverity" ("LogSeverity_ID","LogSeverity_Name") VALUES (128,'Emergency');
INSERT INTO "Configuration" ("Configuration_ID","Configuration_ServerName","Configuration_MinimumLogSeverity") VALUES (1,'SHINKANSEN',2);
INSERT INTO "DynDnsIpAddressType" ("DynDnsIpAddressType_ID","DynDnsIpAddressType_Name") VALUES (1,'NotValid');
INSERT INTO "DynDnsIpAddressType" ("DynDnsIpAddressType_ID","DynDnsIpAddressType_Name") VALUES (2,'Public');
INSERT INTO "DynDnsIpAddressType" ("DynDnsIpAddressType_ID","DynDnsIpAddressType_Name") VALUES (3,'Private');
INSERT INTO "DynDnsIpAddressType" ("DynDnsIpAddressType_ID","DynDnsIpAddressType_Name") VALUES (4,'LinkLocal');
INSERT INTO "DynDnsIpAddressType" ("DynDnsIpAddressType_ID","DynDnsIpAddressType_Name") VALUES (5,'UniqueLocal');
INSERT INTO "DynDnsIpAddressType" ("DynDnsIpAddressType_ID","DynDnsIpAddressType_Name") VALUES (6,'DnsServerPrivate');
INSERT INTO "DynDnsIpAddressType" ("DynDnsIpAddressType_ID","DynDnsIpAddressType_Name") VALUES (7,'DnsServerPublic');
INSERT INTO "DynDnsIpAddressType" ("DynDnsIpAddressType_ID","DynDnsIpAddressType_Name") VALUES (8,'Network');
INSERT INTO "DynDnsIpAddressVersion" ("DynDnsIpAddressVersion_ID","DynDnsIpAddressVersion_Name") VALUES (1,'NotValid');
INSERT INTO "DynDnsIpAddressVersion" ("DynDnsIpAddressVersion_ID","DynDnsIpAddressVersion_Name") VALUES (2,'IPv4');
INSERT INTO "DynDnsIpAddressVersion" ("DynDnsIpAddressVersion_ID","DynDnsIpAddressVersion_Name") VALUES (3,'IPv6');
INSERT INTO "DynDnsIpAddress" ("DynDnsIpAddressReferenceType_ID","DynDnsIpAddressReference_ID","DynDnsIpAddress_ID","DynDnsIpAddressType_ID","DynDnsIpAddressVersion_ID","DynDnsIpAddress_Name","DynDnsIpAddress_Changed") VALUES (1,17,1,6,2,'192.168.22.1',0);
INSERT INTO "DynDnsIpAddress" ("DynDnsIpAddressReferenceType_ID","DynDnsIpAddressReference_ID","DynDnsIpAddress_ID","DynDnsIpAddressType_ID","DynDnsIpAddressVersion_ID","DynDnsIpAddress_Name","DynDnsIpAddress_Changed") VALUES (1,17,2,6,3,'fd11:f0d8:a7bb:135d:1ac0:4dff:fe8b:20d4',0);
INSERT INTO "DynDnsIpAddress" ("DynDnsIpAddressReferenceType_ID","DynDnsIpAddressReference_ID","DynDnsIpAddress_ID","DynDnsIpAddressType_ID","DynDnsIpAddressVersion_ID","DynDnsIpAddress_Name","DynDnsIpAddress_Changed") VALUES (1,17,3,7,2,'45.90.28.58',0);
INSERT INTO "DynDnsIpAddress" ("DynDnsIpAddressReferenceType_ID","DynDnsIpAddressReference_ID","DynDnsIpAddress_ID","DynDnsIpAddressType_ID","DynDnsIpAddressVersion_ID","DynDnsIpAddress_Name","DynDnsIpAddress_Changed") VALUES (1,17,4,7,3,'2a07:a8c0::6d:cda2',0);
INSERT INTO "DynDnsIpAddress" ("DynDnsIpAddressReferenceType_ID","DynDnsIpAddressReference_ID","DynDnsIpAddress_ID","DynDnsIpAddressType_ID","DynDnsIpAddressVersion_ID","DynDnsIpAddress_Name","DynDnsIpAddress_Changed") VALUES (3,1,5,8,2,'192.168.22.0/24',0);
INSERT INTO "DynDnsIpAddress" ("DynDnsIpAddressReferenceType_ID","DynDnsIpAddressReference_ID","DynDnsIpAddress_ID","DynDnsIpAddressType_ID","DynDnsIpAddressVersion_ID","DynDnsIpAddress_Name","DynDnsIpAddress_Changed") VALUES (3,2,6,8,3,'fd11:f0d8:a7bb:135d::/64',0);
INSERT INTO "DynDnsIpAddress" ("DynDnsIpAddressReferenceType_ID","DynDnsIpAddressReference_ID","DynDnsIpAddress_ID","DynDnsIpAddressType_ID","DynDnsIpAddressVersion_ID","DynDnsIpAddress_Name","DynDnsIpAddress_Changed") VALUES (3,3,7,8,2,'45.90.28.58/32',0);
INSERT INTO "DynDnsIpAddress" ("DynDnsIpAddressReferenceType_ID","DynDnsIpAddressReference_ID","DynDnsIpAddress_ID","DynDnsIpAddressType_ID","DynDnsIpAddressVersion_ID","DynDnsIpAddress_Name","DynDnsIpAddress_Changed") VALUES (3,4,8,8,3,'2a07:a8c0::6d:cda2/128',0);
INSERT INTO "DynDnsIpAddressReferenceType" ("DynDnsIpAddressReferenceType_ID","DynDnsIpAddressReferenceType_Name") VALUES (1,'DynDnsService');
INSERT INTO "DynDnsIpAddressReferenceType" ("DynDnsIpAddressReferenceType_ID","DynDnsIpAddressReferenceType_Name") VALUES (2,'DynDnsFirewallRule');
INSERT INTO "DynDnsIpAddressReferenceType" ("DynDnsIpAddressReferenceType_ID","DynDnsIpAddressReferenceType_Name") VALUES (3,'DynDnsIpAddress');
INSERT INTO "Command" ("Command_ID","Command_Class","Command_Function","Command_Step","CommandType_ID","Command_Name") VALUES (1,'Configuration','Configuration','',1,'select a.Configuration_ID, a.Configuration_MinimumLogSeverity

from Configuration as a

where a.Configuration_ServerName = "<MachineName>"');
INSERT INTO "Command" ("Command_ID","Command_Class","Command_Function","Command_Step","CommandType_ID","Command_Name") VALUES (2,'ThreadDynDns','ThreadDynDns','DynDnsServiceType',1,'select min(b.DynDnsServiceType_ID)

from DynDnsDomain as a

inner join DynDnsService b on

    a.Configuration_ID = <ConfigurationID> and b.DynDnsServiceType_ID != 1 and

	a.DynDnsDomain_ID = b.DynDnsDomain_ID');
INSERT INTO "Command" ("Command_ID","Command_Class","Command_Function","Command_Step","CommandType_ID","Command_Name") VALUES (3,'ThreadDynDns','ThreadDynDns','DynDnsServer',1,'select b.DynDnsService_ID

from DynDnsDomain as a

inner join DynDnsService b on

    a.Configuration_ID = <ConfigurationID> and b.DynDnsServiceType_ID = 1 and

	a.DynDnsDomain_ID = b.DynDnsDomain_ID');
INSERT INTO "Command" ("Command_ID","Command_Class","Command_Function","Command_Step","CommandType_ID","Command_Name") VALUES (4,'DynDnsDomain','DynDnsDomain','',1,'select a.DynDnsDomain_Name, a.DynDnsDomain_User, a.DynDnsDomain_Password

from DynDnsDomain as a

where a.DynDnsDomain_ID = <DynDnsDomainID>');
INSERT INTO "Command" ("Command_ID","Command_Class","Command_Function","Command_Step","CommandType_ID","Command_Name") VALUES (5,'DynDnsServer','DynDnsServer','',1,'select a.DynDnsDomain_ID

from DynDnsDomain as a

where a.Configuration_ID = <ConfigurationID>');
INSERT INTO "Command" ("Command_ID","Command_Class","Command_Function","Command_Step","CommandType_ID","Command_Name") VALUES (6,'GuiWindowLog','TmrGuiWindowLog_Tick','',1,'select a.Log_ID, datetime(a.Log_Timestamp, ''unixepoch'', ''localtime'') as Log_Timestamp, b.LogSeverity_Name, c.LogOrigin_Class, c.LogOrigin_Function, c.LogOrigin_Step, a.Log_Message

from Log as a

inner join LogSeverity as b on

    a.LogSeverity_ID = b.LogSeverity_ID

inner join LogOrigin as c on

    a.LogOrigin_ID = c.LogOrigin_ID

order by a.Log_ID desc

limit 10000;');
INSERT INTO "Command" ("Command_ID","Command_Class","Command_Function","Command_Step","CommandType_ID","Command_Name") VALUES (7,'DynDnsNetworkObject','GetIpAddress','',2,'Resolve-DNSName -Name <DomainName> -Server <DnsServer> | Select-Object IPAddress');
INSERT INTO "Command" ("Command_ID","Command_Class","Command_Function","Command_Step","CommandType_ID","Command_Name") VALUES (8,'DynDnsNetworkObject','DynDnsNetworkObject','',1,'select a.DynDnsService_Name

from DynDnsService a

where a.DynDnsService_ID = <DynDnsServiceID>');
INSERT INTO "CommandType" ("CommandType_ID","CommandType_Name") VALUES (1,'SQL');
INSERT INTO "CommandType" ("CommandType_ID","CommandType_Name") VALUES (2,'PowerShell');
COMMIT;
