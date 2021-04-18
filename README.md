# Battleship simulation
this branch has an issue with Probability Density Guessing :(

[Stage.cs](https://github.com/DDaarcon/recruitment-task/blob/master/Classes/Stage.cs) - class responsible for representation of board and operations on it:
- picks random positions for ships,
- recieves attacks and deals them to opponents Stage,
- stores information about board, ships.

[AI.cs](https://github.com/DDaarcon/recruitment-task/blob/master/Classes/AI.cs) - class responsible for simulating gameplay:
- uses Hunt (with parity)/Target method,
- uses Probability Density + Target method,
- methods overview: https://www.datagenetics.com/blog/december32011/

[Index.cs](https://github.com/DDaarcon/recruitment-task/blob/master/Pages/Index.cs) - class resposible for initializing game

[Index.razor](https://github.com/DDaarcon/recruitment-task/blob/master/Pages/Index.razor) - main interface

[Stage.razor](https://github.com/DDaarcon/recruitment-task/blob/master/Components/Stage.razor),
[Cell.razor](https://github.com/DDaarcon/recruitment-task/blob/master/Components/Cell.razor) - visualization of the board
