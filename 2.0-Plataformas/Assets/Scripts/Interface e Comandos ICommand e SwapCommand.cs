using UnityEngine;

public interface ICommand {
    void Execute();
    void Undo();
}

public class SwapCommand : ICommand {
    private PuzzlePiece pieceA, pieceB;

    public SwapCommand(PuzzlePiece a, PuzzlePiece b) {
        pieceA = a;
        pieceB = b;
    }

    public void Execute() {
        pieceA.Swap(pieceB);
    }

    public void Undo() {
        pieceA.Swap(pieceB); // Trocar de novo desfaz
    }
}


