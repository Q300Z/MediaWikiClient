create database mediawiki;
go;
use mediawiki;

create table articles
(
    id             int primary key,
    titre          varchar(255),
    resumer        varchar(max),
    contenu        varchar(max),
    date           datetime,
    indatabase     bit,
    dateindatabase datetime,
    isfavoris      bit,
    datefavoris    datetime,
    islu           bit,
    datelu         datetime
);
go;
create index index_articles_titre on articles (titre);
go;
