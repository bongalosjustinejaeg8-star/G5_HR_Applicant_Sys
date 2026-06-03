
CREATE DATABASE IF NOT EXISTS hr_applicant_system;
USE hr_applicant_system;

CREATE TABLE Roles (
    role_id     CHAR(36)     PRIMARY KEY DEFAULT (UUID()),
    role_name   VARCHAR(50)  NOT NULL UNIQUE,
    permissions TEXT
);

CREATE TABLE Users (
    user_id       CHAR(36)     PRIMARY KEY DEFAULT (UUID()),
    role_id       CHAR(36)     NOT NULL,
    username      VARCHAR(50)  NOT NULL UNIQUE,
    email         VARCHAR(255) NOT NULL UNIQUE,
    password_hash VARCHAR(255) NOT NULL,
    is_active     BOOLEAN      NOT NULL DEFAULT TRUE,

    FOREIGN KEY (role_id) REFERENCES Roles(role_id)
        ON DELETE RESTRICT
        ON UPDATE CASCADE
);

CREATE TABLE ApplicantAccounts (
    account_id    CHAR(36)     PRIMARY KEY DEFAULT (UUID()),
    email         VARCHAR(255) NOT NULL UNIQUE,
    password_hash VARCHAR(255) NOT NULL,
    is_active     BOOLEAN      NOT NULL DEFAULT TRUE,
    created_at    TIMESTAMP    DEFAULT CURRENT_TIMESTAMP
);


CREATE TABLE Applicants (
    applicant_id    CHAR(36)     PRIMARY KEY DEFAULT (UUID()),
    account_id      CHAR(36)     NOT NULL,
    full_name       VARCHAR(150) NOT NULL,
    address         TEXT,
    contact_no      VARCHAR(20),
    education       TEXT,
    work_experience TEXT,
    skills          TEXT,

    FOREIGN KEY (account_id) REFERENCES ApplicantAccounts(account_id)
        ON DELETE CASCADE
        ON UPDATE CASCADE
);

CREATE TABLE Departments (
    department_id   CHAR(36)     PRIMARY KEY DEFAULT (UUID()),
    department_name VARCHAR(100) NOT NULL UNIQUE
);

CREATE TABLE RequirementTypes (
    requirement_type_id CHAR(36)     PRIMARY KEY DEFAULT (UUID()),
    name                VARCHAR(100) NOT NULL,
    description         TEXT
);

CREATE TABLE JobVacancies (
    vacancy_id       CHAR(36)     PRIMARY KEY DEFAULT (UUID()),
    department_id    CHAR(36)     NOT NULL,
    position_title   VARCHAR(150) NOT NULL,
    qualifications   TEXT,
    employment_type  VARCHAR(50)  NOT NULL,
    status           VARCHAR(50)  NOT NULL DEFAULT 'Open',
    created_at       TIMESTAMP    DEFAULT CURRENT_TIMESTAMP,

    FOREIGN KEY (department_id) REFERENCES Departments(department_id)
        ON DELETE RESTRICT
        ON UPDATE CASCADE
);

CREATE TABLE Applications (
    application_id CHAR(36)  PRIMARY KEY DEFAULT (UUID()),
    applicant_id   CHAR(36)  NOT NULL,
    vacancy_id     CHAR(36)  NOT NULL,
    status         VARCHAR(50) NOT NULL DEFAULT 'Draft',
    submitted_at   TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    is_locked      BOOLEAN   NOT NULL DEFAULT FALSE,

    FOREIGN KEY (applicant_id) REFERENCES Applicants(applicant_id)
        ON DELETE CASCADE
        ON UPDATE CASCADE,

    FOREIGN KEY (vacancy_id) REFERENCES JobVacancies(vacancy_id)
        ON DELETE CASCADE
        ON UPDATE CASCADE
);

CREATE TABLE ApplicantDocuments (
    document_id         CHAR(36)     PRIMARY KEY DEFAULT (UUID()),
    applicant_id        CHAR(36)     NOT NULL,
    requirement_type_id CHAR(36)     NOT NULL,
    file_path           VARCHAR(255) NOT NULL,
    status              VARCHAR(50)  NOT NULL DEFAULT 'Missing',
    remarks             TEXT,
    submitted_at        TIMESTAMP    DEFAULT CURRENT_TIMESTAMP,

    FOREIGN KEY (applicant_id) REFERENCES Applicants(applicant_id)
        ON DELETE CASCADE
        ON UPDATE CASCADE,

    FOREIGN KEY (requirement_type_id) REFERENCES RequirementTypes(requirement_type_id)
        ON DELETE RESTRICT
        ON UPDATE CASCADE
);

CREATE TABLE ScreeningResults (
    screening_id   CHAR(36)    PRIMARY KEY DEFAULT (UUID()),
    application_id CHAR(36)    NOT NULL,
    screened_by    CHAR(36),
    result         VARCHAR(50) NOT NULL,
    remarks        TEXT,
    screened_at    TIMESTAMP   DEFAULT CURRENT_TIMESTAMP,

    FOREIGN KEY (application_id) REFERENCES Applications(application_id)
        ON DELETE CASCADE
        ON UPDATE CASCADE,

    FOREIGN KEY (screened_by) REFERENCES Users(user_id)
        ON DELETE SET NULL
        ON UPDATE CASCADE
);

CREATE TABLE InterviewSchedules (
    schedule_id    CHAR(36)     PRIMARY KEY DEFAULT (UUID()),
    application_id CHAR(36)     NOT NULL,
    interviewer_id CHAR(36),
    interview_date DATETIME     NOT NULL,
    mode           VARCHAR(50)  NOT NULL,
    location       VARCHAR(255),
    status         VARCHAR(50)  NOT NULL DEFAULT 'Scheduled',

    FOREIGN KEY (application_id) REFERENCES Applications(application_id)
        ON DELETE CASCADE
        ON UPDATE CASCADE,

    FOREIGN KEY (interviewer_id) REFERENCES Users(user_id)
        ON DELETE SET NULL
        ON UPDATE CASCADE
);

CREATE TABLE InterviewEvaluations (
    eval_id        CHAR(36)     PRIMARY KEY DEFAULT (UUID()),
    schedule_id    CHAR(36)     NOT NULL,
    evaluated_by   CHAR(36),
    score          INT,
    remarks        TEXT,
    recommendation VARCHAR(100),
    pass_fail      VARCHAR(10)  NOT NULL,

    FOREIGN KEY (schedule_id) REFERENCES InterviewSchedules(schedule_id)
        ON DELETE CASCADE
        ON UPDATE CASCADE,

    FOREIGN KEY (evaluated_by) REFERENCES Users(user_id)
        ON DELETE SET NULL
        ON UPDATE CASCADE
);

CREATE TABLE HiringDecisions (
    decision_id    CHAR(36)    PRIMARY KEY DEFAULT (UUID()),
    application_id CHAR(36)    NOT NULL,
    decided_by     CHAR(36),
    decision       VARCHAR(50) NOT NULL,
    remarks        TEXT,
    decided_at     TIMESTAMP   DEFAULT CURRENT_TIMESTAMP,

    FOREIGN KEY (application_id) REFERENCES Applications(application_id)
        ON DELETE CASCADE
        ON UPDATE CASCADE,

    FOREIGN KEY (decided_by) REFERENCES Users(user_id)
        ON DELETE SET NULL
        ON UPDATE CASCADE
);
CREATE TABLE ApplicationStatusHistory (
    history_id     CHAR(36)    PRIMARY KEY DEFAULT (UUID()),
    application_id CHAR(36)    NOT NULL,
    changed_by     CHAR(36),
    old_status     VARCHAR(50),
    new_status     VARCHAR(50) NOT NULL,
    changed_at     TIMESTAMP   DEFAULT CURRENT_TIMESTAMP,
    remarks        TEXT,

    FOREIGN KEY (application_id) REFERENCES Applications(application_id)
        ON DELETE CASCADE
        ON UPDATE CASCADE,

    FOREIGN KEY (changed_by) REFERENCES Users(user_id)
        ON DELETE SET NULL
        ON UPDATE CASCADE
);

CREATE TABLE AuditTrail (
    audit_id       CHAR(36)     PRIMARY KEY DEFAULT (UUID()),
    user_id        CHAR(36),
    action         VARCHAR(100) NOT NULL,
    table_affected VARCHAR(100) NOT NULL,
    record_id      CHAR(36)     NOT NULL,
    performed_at   TIMESTAMP    DEFAULT CURRENT_TIMESTAMP,

    FOREIGN KEY (user_id) REFERENCES Users(user_id)
        ON DELETE SET NULL
        ON UPDATE CASCADE
);