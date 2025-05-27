using UnityEngine;
using UnityEngine.UI;

public class PuzzlePiece : MonoBehaviour
{
    public int correctIndex; // posição correta
    public int currentIndex; // posição atual
    public Button button;

    private PuzzleManager manager;

    public void Initialize(int index, PuzzleManager puzzleManager)
    {
        correctIndex = index;
        currentIndex = index;
        manager = puzzleManager;
        button = GetComponent<Button>();
        button.onClick.AddListener(OnPieceClicked);
    }

    void OnPieceClicked()
    {
        manager.OnPieceSelected(this);
    }

    public void SwapWith(PuzzlePiece other)
    {
        // Trocar as posições no layout
        Transform tempParent = this.transform.parent;
        int thisIndex = this.transform.GetSiblingIndex();
        int otherIndex = other.transform.GetSiblingIndex();

        this.transform.SetSiblingIndex(otherIndex);
        other.transform.SetSiblingIndex(thisIndex);

        // Trocar os índices
        int temp = currentIndex;
        currentIndex = other.currentIndex;
        other.currentIndex = temp;
    }

    public bool IsCorrect()
    {
        return correctIndex == currentIndex;
    }
}