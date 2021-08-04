# TicTacToe

This is a game in which you can try to win the computer.

### Technologies
Used technoligies are: ASP.NET Core 3.1, EntityFramework (ORM), MSSQL (RDBMS),
Bootstrap and JQuery for UI.

While development MSSQL was launched in docker container.

### Structure / architecture
Project structured in a 'Clean Architecture'-like (R.Martin) style.
Web presentation layer uses MVC.

### Technical decisions reasoning
1. DB.
For data schema 2 tables (Game and Move) were made. By requirements history of moves for a game must be stored in separate table.
I would rather store that in one table (reasoning below).

History of moves can be stored either in separate table (every move - separate row) with many-to-one relationship with table of games results or in one table with results on column (serialised to json e.g.)

As we will save and retrieve all moves of a game with one db query (for performance reasons to avoid quering db on every user move) its reasonable to serialise them to string and store in same table (with one query we will get all results with history of moves for every game without much payload because maximum amount of moves in each game is 9). If data is stored in one table as described all work with histories will be faster and easier.

2. "Games in progress" storage
We need to somehow store game board and history of moves while game is in progress. This data can be stored in database, session or in static collection in backend. Storing such data in db is slow and "uncomfortable", session could be injected so static collection was chosen.

Singleton Collection object (wrapper for actual collection) was created for storage. For its internals there were two options available: MemoryCache and ConcurrentDictionary. MemoryCache was chosen because we need expiration logic. If there are many users some of them can have problems with internet e.g. and there will appear interrupted games which will be stored forever. To clean them we need to create background process or use buil-in MemoryCache which already implements such funtionality.

### Possible improvements
- Unit tests.

### Contributors
QA: shaurmur_