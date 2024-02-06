CREATE TABLE IF NOT EXISTS AspNetUsers (
	Id varchar(128) PRIMARY KEY,
	UserName varchar(256) NOT NULL,
	PasswordHash text,
	Email varchar(256),
	PhoneNumber text,
	IP varchar(20),
	CreatedDate timestamp NOT NULL,
	ModifiedDate timestamp
);

CREATE TABLE IF NOT EXISTS Admin (
	AdminId SERIAL PRIMARY KEY,
	AspNetUserId varchar(128) REFERENCES AspNetUsers (Id),
	FirstName varchar(100) NOT NULL,
	LastName varchar(100),
	Email varchar(50) NOT NULL,
	Mobile varchar(20),
	Address1 varchar(500),
	Address2 varchar(500),
	City varchar(100),
	RegionId int,
	Zip varchar(10),
	AltPhone varchar(20),
	CreatedBy varchar(128) NOT NULL,
	CreatedDate timestamp NOT NULL,
	ModifiedBy varchar(128) REFERENCES AspNetUsers (Id),
	ModifiedDate timestamp,
	Status smallint,
	IsDeleted boolean,
	RoleId int
);

CREATE TABLE IF NOT EXISTS Region (
	RegionId SERIAL PRIMARY KEY,
	Name varchar(50) NOT NULL,
	Abbreviation varchar(50)
);
CREATE TABLE IF NOT EXISTS AdminRegion (
	AdminRegionId SERIAL PRIMARY KEY,
	RegionId int REFERENCES Region (RegionId),
	AdminId int REFERENCES Admin (AdminId)
);

CREATE TABLE IF NOT EXISTS AspNetRoles (
	Id varchar(128) PRIMARY KEY,
	Name varchar(256) NOT NULL
);

CREATE TABLE IF NOT EXISTS AspNetUserRoles (
	UserId varchar(128) REFERENCES AspNetUsers (Id),
	Name varchar(256) NOT NULL,
	PRIMARY KEY (UserId , Name)
);

CREATE TABLE IF NOT EXISTS BlockRequests (
	BlockRequestId SERIAL PRIMARY KEY,
	PhoneNumber varchar(50),
	Email varchar(50),
	IsActive boolean,
	Reason text,
	RequestId varchar(50) NOT NULL,
	IP varchar(20),
	CreatedDate timestamp,
	ModifiedDate timestamp
);

CREATE TABLE IF NOT EXISTS R_BusinessData (
	Id SERIAL PRIMARY KEY,
	Name varchar(100) NOT NULL,
	Address1 varchar(500),
	Address2 varchar(500),
	City varchar(50),
	RegionId int REFERENCES Region(RegionId),
	ZipCode varchar(10),
	PhoneNumber varchar(20),
	FaxNumber varchar(20),
	IsRefistered boolean,
	CreatedBy varchar(128) REFERENCES AspNetUsers(Id),
	CreatedDate timestamp NOT NULL,
	ModifiedBy varchar(128) REFERENCES AspNetUsers(Id),
	ModifiedDate timestamp NOT NULL,
	Status smallint,
	IsDeleted boolean,
	IP varchar(20)
);

CREATE TABLE IF NOT EXISTS CaseTag (
	CaseTagId int ,
	Name varchar(50)
);

CREATE TABLE IF NOT EXISTS R_Concierge (
	ConciergeId SERIAL PRIMARY KEY,
	ConciergeName varchar(100) NOT NULL,
	Address varchar(150),
	Street varchar(50) NOT NULL,
	City varchar(50) NOT NULL,
	State varchar(50) NOT NULL,
	ZipCode varchar(50) NOT NULL,
	CreatedDate timestamp NOT NULL,
	RegionId int REFERENCES Region (RegionId),
	IP varchar(20)
);

CREATE TABLE IF NOT EXISTS Emaillog (
	EmailLogID SERIAL PRIMARY KEY,
	EmailTemplate varchar(1) NOT NULL,
	SubjectName varchar(200) NOT NULL,
	EmailID varchar(200) NOT NULL,
	ConfirmationNumber varchar(200),
	FilePath text,
	RoleId int,
	RequestId int,
	AdminId int REFERENCES Admin (AdminId),
	PhysicianId int,
	CreateDate timestamp NOT NULL,
	SentDate timestamp,
	IsEmailSent boolean,
	SentTries int,
	Action int
);

CREATE TABLE IF NOT EXISTS HealthProfessionalType (
	HealthProfessionalId  SERIAL PRIMARY KEY,
	ProfessionName VARCHAR(50) NOT NULL,
	CreatedDate timestamp NOT NULL,
	IsActive  boolean,
	IsDeleted boolean
);

CREATE TABLE IF NOT EXISTS HealthProfessionals (
	VendorId SERIAL PRIMARY KEY,
	VendorName varchar(100) NOT NULL,
	Profession int REFERENCES HealthProfessionalType (HealthProfessionalId),
	FaxNumber varchar(50) NOT NULL,
	Address varchar(150),
	City varchar(100),
	State varchar(50),
	Zip varchar(50),
	RegionId int,
	CreatedDate timestamp NOT NULL,
	ModifiedDate timestamp,
	PhoneNumber varchar(100),
	IsDeleted boolean,
	IP varchar(20),
	Email varchar(50),
	BusinessContact varchar(100)
);




CREATE TABLE IF NOT EXISTS Menu (
	MenuId SERIAL PRIMARY KEY,
	Name varchar(50) NOT NULL,
	AccountType smallint NOT NULL CHECK (AccountType IN (1, 2)),
	SortOrder int
);

CREATE TABLE IF NOT EXISTS OrderDetails (
	Id SERIAL PRIMARY KEY ,
	VendorId INT ,
	RequestId INT,
	FaxNumber varchar(50),
	Email varchar(50),
	BusinessContact varchar(100),
	Prescription text,
	NoOfRefill INT,
	CreatedDate timestamp,
	CreatedBy varchar(100)
);


CREATE TABLE IF NOT EXISTS Physician	(
	PhysicianId SERIAL PRIMARY KEY,
	AspNetUserId varchar(128) REFERENCES AspNetUsers (Id),
	FirstName varchar(100) NOT NULL,
	LastName varchar(100),
	Email varchar(50) NOT NULL,
	Mobile varchar(20),
	MedicalLicense varchar(500),
	Photo varchar(100),
	AdminNotes varchar(500),
	IsAgreementDoc boolean,
	IsBackgroundDoc boolean,
	IsTrainingDoc boolean,
	IsNonDisclosureDoc boolean,
	Address1 varchar(500),
	Address2 varchar(500),
	City varchar(100),
	RegionId int,
	Zip varchar(10),
	AltPhone varchar(20),
	CreatedBy varchar(128) NOT NULL REFERENCES AspNetUsers (Id),
	CreatedDate timestamp(0) NOT NULL,
	ModifiedBy varchar(128) REFERENCES AspNetUsers (Id),
	ModifiedDate timestamp,
	Status smallint,
	BusinessName varchar(100) NOT NULL,
	BusinessWebsite varchar(200) NOT NULL,
	IsDeleted boolean,
	RoleId int,
	NPINumber varchar(500),
	IsLicenseDoc boolean,
	Signature varchar(100),
	IsCredentialDoc boolean,
	IsTokenGenerate boolean,
	SyncEmailAddress varchar(50)
);

CREATE TABLE IF NOT EXISTS PhysicianLocation (
	LocationId SERIAL PRIMARY KEY,
	PhysicianId INT REFERENCES Physician(PhysicianId),
	Latitude DECIMAL(9),
	Longtitude DECIMAL(9),
	CreatedDate timestamp,
	PhysicianName varchar(50),
	Address varchar(500)
);

CREATE TABLE PhysicianNotification (
	Id SERIAL PRIMARY KEY,
	PhysicianId int NOT NULL REFERENCES Physician(PhysicianId),
	IsNotificationStopped boolean NOT NULL
);

CREATE TABLE IF NOT EXISTS PhysicianRegion (
	PhysicianRegionId SERIAL PRIMARY KEY,
	PhysicianId int NOT NULL REFERENCES Physician (PhysicianId),
	RegionId int NOT NULL REFERENCES Region (RegionId)
);

CREATE TABLE IF NOT EXISTS Request (
	RequestId SERIAL PRIMARY KEY,
	RequestTypeId int NOT NULL CHECK (RequestTypeId IN (1, 2, 3, 4)),
	UserId int REFERENCES "User" (UserId),
	FirstName varchar(100),
	LastName varchar(100),
	PhoneNumber varchar(23),
	Email varchar(50),
	Status smallint NOT NULL CHECK (Status IN (1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15)),
	PhysicianId int REFERENCES Physician (PhysicianId),
	ConfirmationNumber varchar(20),
	CreatedDate timestamp NOT NULL,
	IsDeleted boolean,
	ModifiedDate timestamp,
	DeclinedBy varchar(250),
	IsUrgentEmailSent boolean NOT NULL,
	LastWellnessDate timestamp,
	IsMobile boolean,
	CallType smallint CHECK (CallType IN (1, 2)),
	CompletedByPhysician boolean,
	LastReservationDate timestamp,
	AcceptedDate timestamp,
	RelationName varchar(100),
	CaseNumber varchar(50),
	IP varchar(20),
	CaseTag varchar(50),
	CaseTagPhysician varchar(50),
	PatientAccountId varchar(128),
	CreatedUserId int
);


CREATE TABLE IF NOT EXISTS RequestBusiness (
	RequestBusinessId SERIAL PRIMARY KEY,
	RequestId int NOT NULL REFERENCES Request (RequestId),
	BusinessId int NOT NULL REFERENCES R_BusinessData (Id),
	IP varchar(20)
);


CREATE TABLE IF NOT EXISTS RequestClient (
	RequestClientId SERIAL PRIMARY KEY,
	RequestId int NOT NULL REFERENCES Request (RequestId),
	FirstName varchar(100) NOT NULL,
	LastName varchar(100),
	PhoneNumber varchar(23),
	Location varchar(100),
	Address varchar(500),
	RegionId int REFERENCES Region (RegionId),
	NotiMobile varchar(20),
	NotiEmail varchar(50),
	Notes varchar(500),
	Email varchar(50),
	strMonth varchar(20),
	intYear int,
	intDate int,
	IsMobile boolean,
	Street varchar(100),
	City varchar(100),
	State varchar(100),
	ZipCode varchar(10),
	CommunicationType smallint,
	RemindReservationCount smallint,
	RemindHouseCallCount smallint,
	IsSetFollowupSent smallint,
	IP varchar(20),
	IsReservationReminderSent smallint,
	Latitude decimal(9),
	Longitude decimal(9)
);


CREATE TABLE RequestStatusLog (
	RequestStatusLogId serial PRIMARY KEY,
	RequestId int NOT NULL REFERENCES Request (RequestId),
	Status smallint NOT NULL,
	PhysicianId int REFERENCES Physician (PhysicianId),
	AdminId int REFERENCES Admin (AdminId),
	TransToPhysicianId int REFERENCES Physician (PhysicianId),
	Notes varchar (500),
	CreatedDate timestamp NOT NULL,
	IP varchar (20),
	TransToAdmin boolean
);  


CREATE TABLE IF NOT EXISTS RequestClosed (
	RequestClosedId SERIAL PRIMARY KEY,
	RequestId int NOT NULL REFERENCES Request (RequestId),
	RequestStatusLogId int NOT NULL REFERENCES RequestStatusLog (RequestStatusLogId),
	PhyNotes varchar(500),
	ClientNotes varchar(500),
	IP varchar(20)
);


CREATE TABLE IF NOT EXISTS RequestConcierge (
	Id int PRIMARY KEY,
	RequestId int NOT NULL REFERENCES Request (RequestId),
	ConciergeId int NOT NULL REFERENCES R_Concierge (ConciergeId),
	IP varchar(20)
);


CREATE TABLE IF NOT EXISTS RequestNotes (
	RequestNotesId int PRIMARY KEY,
	RequestId int NOT NULL REFERENCES Request (RequestId),
	strMonth varchar(20),
	intYear int,
	intDate int,
	PhysicianNotes varchar(500),
	AdminNotes varchar(500),
	CreatedBy varchar(128) NOT NULL,
	CreatedDate timestamp NOT NULL,
	ModifiedBy varchar(128),
	ModifiedDate timestamp,
	IP varchar(20),
	AdministrativeNotes varchar(500)
);



CREATE TABLE IF NOT EXISTS RequestType (
	RequestTypeId SERIAL PRIMARY KEY,
	Name VARCHAR(50)
);

CREATE TABLE IF NOT EXISTS RequestWiseFile(
	RequestWiseFileId SERIAL PRIMARY KEY,
	RequestId int NOT NULL REFERENCES Request(RequestId),
	FileName varchar(500) NOT NULL,
	CreatedDate timestamp NOT NULL,
	PhysicianId int REFERENCES Physician (PhysicianId),
	AdminId int REFERENCES Admin (AdminId),
	DocType smallint CHECK (DocType IN (1, 2, 3)),
	IsFrontSide boolean,
	IsCompensation boolean,
	IP varchar(20),
	IsFinalize boolean,
	IsDeleted boolean,
	IsPatinetRecords boolean	
);

CREATE TABLE IF NOT EXISTS "Role" (
	RoleId serial PRIMARY KEY,
	Name  varchar(50) NOT NULL,
	AccountType smallint CHECK (AccountType IN (1, 2)),
	CreatedBy varchar(128) NOT NULL,
	CreatedDate timestamp NOT NULL,
	ModifiedBy varchar(128),
	ModifiedDate timestamp ,
	IsDeleted boolean NOT NULL,
	IP varchar(20)
);

CREATE TABLE IF NOT EXISTS RoleMenu (
	RoleMenuId SERIAL PRIMARY KEY,
	RoleId INT REFERENCES "Role" (RoleId),
	MenuId INT REFERENCES Menu (MenuId)
);

CREATE TABLE Shift (
	ShiftId int PRIMARY KEY,
	PhysicianId int NOT NULL REFERENCES Physician(PhysicianId),
	StartDate date NOT NULL,
	IsRepeat boolean NOT NULL,
	WeekDays char(7),
	RepeatUpto int,
	CreatedBy varchar(128) NOT NULL REFERENCES AspNetUsers(Id),
	CreatedDate timestamp NOT NULL,
	IP varchar(20)
);

CREATE TABLE ShiftDetail (
	ShiftDetailId int PRIMARY KEY,
	ShiftId int NOT NULL REFERENCES Shift(ShiftId),
	ShiftDate timestamp NOT NULL,
	RegionId int REFERENCES Region(RegionId),
	StartTime time NOT NULL,
	EndTime time NOT NULL,
	Status smallint NOT NULL,
	IsDeleted boolean NOT NULL,
	ModifiedBy varchar(128) REFERENCES AspNetUsers(Id),
	ModifiedDate timestamp,
	LastRunningDate timestamp,
	EventId varchar(100),
	IsSync boolean
);


CREATE TABLE ShiftDetailRegion (
	ShiftDetailRegionId int PRIMARY KEY,
	ShiftDetailId int NOT NULL REFERENCES ShiftDetail(ShiftDetailId),
	RegionId int NOT NULL REFERENCES Region(RegionId),
	IsDeleted boolean
);


CREATE TABLE SMSLog (
	SMSLogID decimal(9) PRIMARY KEY,
	SMSTemplate varchar(1) NOT NULL,
	MobileNumber varchar(50) NOT NULL,
	ConfirmationNumber varchar(200),
	RoleId int,
	AdminId int,
	RequestId int,
	PhysicianId int,
	CreateDate timestamp NOT NULL,
	SentDate timestamp,
	IsSMSSent boolean,
	SentTries int NOT NULL,
	Action int
);


CREATE TABLE "User" (
	UserId int PRIMARY KEY,
	AspNetUserId varchar(128) REFERENCES AspNetUsers(Id),
	FirstName varchar(100) NOT NULL,
	LastName varchar(100),
	Email varchar(50) NOT NULL,
	Mobile varchar(20),
	IsMobile boolean,
	Street varchar(100),
	City varchar(100),
	State varchar(100),
	RegionId int REFERENCES Region(RegionId),
	ZipCode varchar(10),
	strMonth varchar(20),
	intYear int,
	intDate int,
	CreatedBy varchar(128) NOT NULL,
	CreatedDate timestamp NOT NULL,
	ModifiedBy varchar(128),
	ModifiedDate timestamp,
	Status smallint,
	IsDeleted boolean,
	IP varchar(20),
	IsRequestWithEmail boolean
);













