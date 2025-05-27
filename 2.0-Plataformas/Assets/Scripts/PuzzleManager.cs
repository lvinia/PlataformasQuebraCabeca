using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PuzzleManager : MonoBehaviour
{
    public Transform puzzlePanel;               // Painel com as peças (GridLayoutGroup)
    public Button undoButton;                   // Botão "Desfazer"
    public Button replayButton;                 // Botão "Ver Replay" (tela de vitória)
    public GameObject victoryPanel;             // Painel de vitória com botões

    private PuzzlePiece selectedPiece;
    private Stack<ICommand> history = new Stack<ICommand>();
    private List<ICommand> replayCommands = new List<ICommand>();
    private bool isReplaying = false;

    void Start()
    {
        InitializePieces();
        ShufflePieces();
        victoryPanel.SetActive(false);

        undoButton.onClick.AddListener(UndoMove);
        replayButton.onClick.AddListener(StartReplay);
    }

    void InitializePieces()
    {
        for (int i = 0; i < puzzlePanel.childCount; i++)
        {
            PuzzlePiece piece = puzzlePanel.GetChild(i).GetComponent<PuzzlePiece>();
            piece.Initialize(i, this);
        }
    }

    void ShufflePieces()
    {
        for (int i = 0; i < puzzlePanel.childCount; i++)
        {
            int rand = Random.Range(0, puzzlePanel.childCount);
            puzzlePanel.GetChild(i).SetSiblingIndex(rand);
        }

        // Atualizar os índices atuais das peças após embaralhar
        for (int i = 0; i < puzzlePanel.childCount; i++)
        {
            PuzzlePiece piece = puzzlePanel.GetChild(i).GetComponent<PuzzlePiece>();
            piece.currentIndex = i;
        }
    }

    public void OnPieceSelected(PuzzlePiece piece)
    {
        if (isReplaying) return;

        if (selectedPiece == null)
        {
            selectedPiece = piece;
        }
        else
        {
            if (selectedPiece != piece)
            {
                ICommand command = new SwapCommand(selectedPiece, piece);
                command.Execute();

                history.Push(command);
                replayCommands.Add(command);

                CheckVictory();
            }

            selectedPiece = null;
        }
    }

    public void UndoMove()
    {
        if (isReplaying || selectedPiece != null || history.Count == 0)
            return;

        ICommand lastCommand = history.Pop();
        lastCommand.Undo();
    }

    void CheckVictory()
    {
        foreach (Transform child in puzzlePanel)
        {
            PuzzlePiece piece = child.GetComponent<PuzzlePiece>();
            if (!piece.IsCorrect()) return;
        }

        ShowVictory();
    }

    void ShowVictory()
    {
        victoryPanel.SetActive(true);
    }

    public void StartReplay()
    {
        if (isReplaying) return;

        victoryPanel.SetActive(false);
        StartCoroutine(ReplayRoutine());
    }

    IEnumerator ReplayRoutine()
    {
        isReplaying = true;
        ResetPuzzle();
        yield return new WaitForSeconds(1f);

        foreach (ICommand command in replayCommands)
        {
            command.Execute();
            yield return new WaitForSeconds(1f);
        }

        isReplaying = false;
        ShowVictory();
    }

    void ResetPuzzle()
    {
        for (int i = 0; i < puzzlePanel.childCount; i++)
        {
            puzzlePanel.GetChild(i).SetSiblingIndex(i);

            PuzzlePiece piece = puzzlePanel.GetChild(i).GetComponent<PuzzlePiece>();
            piece.currentIndex = i;
        }

        history.Clear();
        selectedPiece = null;
    }

    public void RestartGame()
    {
        history.Clear();
        replayCommands.Clear();
        selectedPiece = null;
        victoryPanel.SetActive(false);
        ShufflePieces();
    }
}

