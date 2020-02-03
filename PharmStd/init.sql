create schema pharm;

alter schema pharm owner to postgres;

create table drugs
(
    drugid serial not null
        constraint drugs_pk
            primary key,
    name text not null,
    pharmgroup text[] not null,
    activecomponent text not null,
    atxcode text not null,
    category text not null
);

alter table drugs owner to root;

create unique index drugs_atxcode_uindex
    on drugs (atxcode);

create table medicalfacilities
(
    facilityid serial not null
        constraint medicalfacilities_pk
            primary key,
    name text not null,
    address text not null,
    phonenumber text not null
);

alter table medicalfacilities owner to root;

create unique index medicalfacilities_name_uindex
    on medicalfacilities (name);

create table patients
(
    patientid serial not null
        constraint patients_pk
            primary key,
    firstname text not null,
    lastname text not null
);

alter table patients owner to root;

create table prescribeddrugs
(
    patientid integer not null
        constraint prescribeddrugs_patients_patientid_fk
            references patients
            on update cascade on delete cascade,
    drugid integer not null
        constraint prescribeddrugs_drugs_drugid_fk
            references drugs
            on update cascade on delete cascade
);

alter table prescribeddrugs owner to root;

create table patientsinfacilities
(
    patientid integer not null
        constraint patientsinfacilities_patients_patientid_fk
            references patients
            on update cascade on delete cascade,
    facilityid integer not null
        constraint patientsinfacilities_medicalfacilities_facilityid_fk
            references medicalfacilities
            on update cascade on delete cascade
);

alter table patientsinfacilities owner to root;

create unique index patientsinfacilities_facilityid_uindex
    on patientsinfacilities (facilityid);

