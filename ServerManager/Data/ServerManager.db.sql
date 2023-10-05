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
CREATE TABLE IF NOT EXISTS "DataLogSeverity" (
	"DataLogSeverity_ID"	INTEGER NOT NULL,
	"DataLogSeverity_Name"	TEXT NOT NULL UNIQUE,
	PRIMARY KEY("DataLogSeverity_ID")
);
CREATE TABLE IF NOT EXISTS "DataLogOrigin" (
	"DataLogOrigin_ID"	INTEGER NOT NULL,
	"DataLogOrigin_Class"	TEXT NOT NULL,
	"DataLogOrigin_Function"	TEXT NOT NULL,
	"DataLogOrigin_Step"	TEXT NOT NULL,
	PRIMARY KEY("DataLogOrigin_ID")
);
CREATE TABLE IF NOT EXISTS "DataLog" (
	"DataLog_ID"	INTEGER NOT NULL,
	"DataLog_Timestamp"	INTEGER NOT NULL,
	"DataLogSeverity_ID"	INTEGER NOT NULL,
	"DataLogOrigin_ID"	INTEGER NOT NULL,
	"DataLog_Message"	TEXT NOT NULL,
	PRIMARY KEY("DataLog_ID" AUTOINCREMENT)
);
INSERT INTO "DataLogSeverity" VALUES (1,'Debug');
INSERT INTO "DataLogSeverity" VALUES (2,'Informational');
INSERT INTO "DataLogSeverity" VALUES (4,'Notice');
INSERT INTO "DataLogSeverity" VALUES (8,'Warning');
INSERT INTO "DataLogSeverity" VALUES (16,'Error');
INSERT INTO "DataLogSeverity" VALUES (32,'Critical');
INSERT INTO "DataLogSeverity" VALUES (64,'Alert');
INSERT INTO "DataLogSeverity" VALUES (128,'Emergency');
INSERT INTO "DataLogOrigin" VALUES (1,'ProgramMain','Main','');
INSERT INTO "DataLogOrigin" VALUES (2,'GuiMain','GuiMain','');
INSERT INTO "DataLogOrigin" VALUES (3,'GuiMain','DataLogEntryOrigin','');
INSERT INTO "DataLogOrigin" VALUES (4,'DataLogEntry','DataLogEntryMessage','');
INSERT INTO "DataLogOrigin" VALUES (5,'ProgramMain','Main','');
COMMIT;
