using UnityEngine;

public class PuzzlePiece : MonoBehaviour {
    public int correctIndex;
    public int currentIndex;

    public void OnClick() {
        FindObjectOfType<PuzzleManager>().SelectPiece(this);
    }

    public void Swap(PuzzlePiece other) {
        int tempIndex = this.currentIndex;
        this.currentIndex = other.currentIndex;
        other.currentIndex = tempIndex;

        Vector3 tempPos = this.transform.localPosition;
        this.transform.localPosition = other.transform.localPosition;
        other.transform.localPosition = tempPos;
    }

    public bool IsCorrect() {
        return currentIndex == correctIndex;
    }
}
