# Chess

A fully playable chess game built with C# and WPF. Features all standard chess pieces with move validation, check detection, and turn-based play.

## Technologies

- C#, WPF (XAML), .NET

## Features

- All 6 piece types with individual move logic (Bishop, King, Knight, Pawn, Queen, Rook)
- Interface-based piece design (`IPiece`)
- Check detection with visual highlighting
- Turn indicator display
- Full set of piece graphics (white, black, and red-highlighted variants)

## Project Structure

```
Chess/
├── IPiece.cs           # Piece interface
├── Bishop.cs           # Bishop movement logic
├── King.cs             # King movement logic
├── Knight.cs           # Knight movement logic
├── Pawn.cs             # Pawn movement logic
├── Queen.cs            # Queen movement logic
├── Rook.cs             # Rook movement logic
├── Square.cs           # Board square representation
├── MainWindow.xaml     # Game UI
└── Images/             # Piece graphics
```

## How to Run

Open `Chess.sln` in Visual Studio and run the project.
