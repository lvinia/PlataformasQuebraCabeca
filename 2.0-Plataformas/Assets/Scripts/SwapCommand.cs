public class SwapCommand : ICommand
{
    private PuzzlePiece piece1;
    private PuzzlePiece piece2;

    public SwapCommand(PuzzlePiece p1, PuzzlePiece p2)
    {
        piece1 = p1;
        piece2 = p2;
    }

    public void Execute()
    {
        piece1.SwapWith(piece2);
    }

    public void Undo()
    {
        piece1.SwapWith(piece2); // Trocar novamente desfaz
    }
}