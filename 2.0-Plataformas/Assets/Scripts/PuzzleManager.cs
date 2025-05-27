using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PuzzleManager : MonoBehaviour
{
    public Transform puzzlePanel;
    public Button undoButton;
    public Button replayButton;
    public Button skipReplayButton;
    public GameObject victoryPanel;

    private PuzzlePiece selectedPiece;
    private Stack<ICommand> history = new Stack<ICommand>();
    private List<ICommand> replayCommands = new List<ICommand>();
    private List<int> initialOrder = new List<int>();
    private bool isReplaying = false;

    void Start()
    {
        InitializePieces();
        ShufflePieces();
        victoryPanel.SetActive(false);
        undoButton.onClick.AddListener(UndoMove);
        replayButton.onClick.AddListener(StartReplay);
        skipReplayButton.gameObject.SetActive(false);
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

        // Atualiza os índices após o embaralhamento
        for (int i = 0; i < puzzlePanel.childCount; i++)
        {
            PuzzlePiece piece = puzzlePanel.GetChild(i).GetComponent<PuzzlePiece>();
            piece.currentIndex = i;
        }

        // Salva a ordem embaralhada
        initialOrder.Clear();
        for (int i = 0; i < puzzlePanel.childCount; i++)
        {
            PuzzlePiece piece = puzzlePanel.GetChild(i).GetComponent<PuzzlePiece>();
            initialOrder.Add(piece.correctIndex);
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
        skipReplayButton.gameObject.SetActive(true);
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
        skipReplayButton.gameObject.SetActive(false);
        ShowVictory();
    }

    public void SkipReplay()
    {
        if (!isReplaying) return;

        StopAllCoroutines();
        ResetPuzzle();

        foreach (ICommand command in replayCommands)
        {
            command.Execute();
        }

        isReplaying = false;
        skipReplayButton.gameObject.SetActive(false);
        ShowVictory();
    }

    void ResetPuzzle()
    {
        for (int i = 0; i < puzzlePanel.childCount; i++)
        {
            for (int j = 0; j < puzzlePanel.childCount; j++)
            {
                PuzzlePiece piece = puzzlePanel.GetChild(j).GetComponent<PuzzlePiece>();
                if (piece.correctIndex == initialOrder[i])
                {
                    piece.transform.SetSiblingIndex(i);
                    piece.currentIndex = i;
                    break;
                }
            }
        }

        history.Clear();
        selectedPiece = null;
    }

    public void RestartGame()
    {
        skipReplayButton.gameObject.SetActive(false);
        history.Clear();
        replayCommands.Clear();
        selectedPiece = null;
        victoryPanel.SetActive(false);
        ShufflePieces();
    }
}