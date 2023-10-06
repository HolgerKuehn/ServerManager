BEGIN TRANSACTION;
CREATE TABLE IF NOT EXISTS "ServerManagerDynDnsService" (
	"ServerManagerDynDnsDomain_ID"	INTEGER NOT NULL,
	"ServerManagerDynDnsService_ID"	INTEGER NOT NULL,
	"ServerManagerDynDnsService_Name"	TEXT NOT NULL,
	"ServerManagerDynDnsService_Type"	INTEGER NOT NULL,
	PRIMARY KEY("ServerManagerDynDnsService_ID" AUTOINCREMENT)
);
CREATE TABLE IF NOT EXISTS "ServerManagerDynDnsDomainUpdateUri" (
	"ServerManagerDynDnsDomain_ID"	INTEGER NOT NULL,
	"ServerManagerDynDnsDomainUpdateUri_ID"	INTEGER NOT NULL,
	"ServerManagerDynDnsDomainUpdateUri_AddressFamily"	INTEGER NOT NULL,
	"ServerManagerDynDnsDomainUpdateUri_Uri"	TEXT NOT NULL,
	PRIMARY KEY("ServerManagerDynDnsDomainUpdateUri_ID" AUTOINCREMENT)
);
CREATE TABLE IF NOT EXISTS "ServerManagerDynDnsDomain" (
	"ServerManagerConfiguration_ID"	INTEGER,
	"ServerManagerDynDnsDomain_ID"	INTEGER NOT NULL,
	"ServerManagerDynDnsDomain_Name"	TEXT NOT NULL,
	"ServerManagerDynDnsDomain_User"	TEXT NOT NULL,
	"ServerManagerDynDnsDomain_Password"	TEXT NOT NULL,
	PRIMARY KEY("ServerManagerDynDnsDomain_ID" AUTOINCREMENT)
);
CREATE TABLE IF NOT EXISTS "Configuration" (
	"Configuration_ID"	INTEGER NOT NULL,
	"Configuration_ServerName"	VARCHAR(255) NOT NULL UNIQUE,
	"Configuration_Services"	INTEGER NOT NULL DEFAULT '0',
	PRIMARY KEY("Configuration_ID")
);
CREATE TABLE IF NOT EXISTS "LogSeverity" (
	"LogSeverity_ID"	INTEGER NOT NULL,
	"LogSeverity_Name"	TEXT NOT NULL UNIQUE,
	PRIMARY KEY("LogSeverity_ID")
);
CREATE TABLE IF NOT EXISTS "LogOrigin" (
	"LogOrigin_ID"	INTEGER NOT NULL,
	"LogOrigin_Class"	TEXT NOT NULL,
	"LogOrigin_Function"	TEXT NOT NULL,
	"LogOrigin_Step"	TEXT NOT NULL,
	PRIMARY KEY("LogOrigin_ID")
);
CREATE TABLE IF NOT EXISTS "Log" (
	"Log_ID"	INTEGER NOT NULL,
	"Log_Timestamp"	INTEGER NOT NULL,
	"LogSeverity_ID"	INTEGER NOT NULL,
	"LogOrigin_ID"	INTEGER NOT NULL,
	"Log_Message"	TEXT NOT NULL,
	PRIMARY KEY("Log_ID" AUTOINCREMENT)
);
INSERT INTO "LogSeverity" VALUES (1,'Debug');
INSERT INTO "LogSeverity" VALUES (2,'Informational');
INSERT INTO "LogSeverity" VALUES (4,'Notice');
INSERT INTO "LogSeverity" VALUES (8,'Warning');
INSERT INTO "LogSeverity" VALUES (16,'Error');
INSERT INTO "LogSeverity" VALUES (32,'Critical');
INSERT INTO "LogSeverity" VALUES (64,'Alert');
INSERT INTO "LogSeverity" VALUES (128,'Emergency');
INSERT INTO "LogOrigin" VALUES (1,'ProgramMain','Main','');
INSERT INTO "LogOrigin" VALUES (2,'GuiMain','GuiMain','');
INSERT INTO "LogOrigin" VALUES (3,'GuiMain','LogEntryOrigin','');
INSERT INTO "LogOrigin" VALUES (4,'LogEntry','LogEntryMessage','');
INSERT INTO "LogOrigin" VALUES (5,'ProgramMain','Main','');
COMMIT;
