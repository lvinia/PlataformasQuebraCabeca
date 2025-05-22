using UnityEngine;
public class PuzzleManager : MonoBehaviour {
    public GameObject piecePrefab;
    public Sprite[] puzzleSprites; // As 16 peças cortadas
    public Transform gridParent;
    private PuzzlePiece selectedPiece = null;
    
    public Stack<ICommand> commandHistory = new Stack<ICommand>();
    public List<ICommand> replayCommands = new List<ICommand>();

    void Start() {
        CreatePuzzle();
        Shuffle();
    }

    void CreatePuzzle() {
        // Instancia peças na ordem correta
    }

    void Shuffle() {
        // Embaralha posições
    }

    public void SelectPiece(PuzzlePiece piece) {
        if (selectedPiece == null) {
            selectedPiece = piece;
        } else {
            ICommand move = new SwapCommand(selectedPiece, piece);
            move.Execute();
            commandHistory.Push(move);
            replayCommands.Add(move);
            selectedPiece = null;
        }
    }

    public void Undo() {
        if (selectedPiece == null && commandHistory.Count > 0) {
            ICommand last = commandHistory.Pop();
            last.Undo();
        }
    }

    public IEnumerator Replay() {
        foreach (ICommand cmd in replayCommands) {
            cmd.Execute();
            yield return new WaitForSeconds(1f);
        }
        // Mostrar vitória novamente
    }

    public void SkipReplay() {
        foreach (ICommand cmd in replayCommands) {
            cmd.Execute();
        }
        // Mostrar vitória
    }
}