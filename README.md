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
â”œâ”€â”€ IPiece.cs           # Piece interface
â”œâ”€â”€ Bishop.cs           # Bishop movement logic
â”œâ”€â”€ King.cs             # King movement logic
â”œâ”€â”€ Knight.cs           # Knight movement logic
â”œâ”€â”€ Pawn.cs             # Pawn movement logic
â”œâ”€â”€ Queen.cs            # Queen movement logic
â”œâ”€â”€ Rook.cs             # Rook movement logic
â”œâ”€â”€ Square.cs           # Board square representation
â”œâ”€â”€ MainWindow.xaml     # Game UI
â””â”€â”€ Images/             # Piece graphics
```

## How to Run

Open `Chess.sln` in Visual Studio and run the project.
