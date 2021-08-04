create database TicTacToe
use TicTacToe

create table Games (
    Id int identity, Result int,
    constraint PK_Games primary key (Id)
)

create table Moves (
    Id int identity, Coordinate varchar(2), GameId int,
    constraint PK_Moves primary key (Id),
    constraint FK_Moves foreign key (Id) references Games (Id)
)