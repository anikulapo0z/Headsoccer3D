using DG.Tweening;
using UnityEngine;

public class GoalMovement : MonoBehaviour
{
    [SerializeField] Transform[] waypoints;
    [SerializeField] float moveSpeed = 5f;
    [SerializeField] float rotationSpeed = 5f;
    [SerializeField] Vector3 rotationOffset;
    [SerializeField] float thrownbackDuration;

    Vector3 originalGroundPosition;
    Vector3 originalGroundRotation;
    GoalMoverManager manager;
    Sequence sequence;

    int currentPathIndex = 0;
    Vector3[] pathPositions;
    bool isMoving = false;

    public void GoalRun(GoalMoverManager mgr, Vector3 groundPos, Vector3 groundRot)
    {
        manager = mgr;
        originalGroundPosition = groundPos;
        originalGroundRotation = groundRot;
        currentPathIndex = 0;
        isMoving = true;

        pathPositions = new Vector3[waypoints.Length];
        for (int i = 0; i < waypoints.Length; i++)
            pathPositions[i] = new Vector3(waypoints[i].position.x, transform.position.y, waypoints[i].position.z);

        float totalLength = 0f;
        Vector3 prev = transform.position;
        foreach (var p in pathPositions) { totalLength += Vector3.Distance(prev, p); prev = p; }
        float duration = totalLength / moveSpeed;

        sequence?.Kill();
        sequence = DOTween.Sequence();

        Tween move = transform
            .DOPath(pathPositions, duration, PathType.CatmullRom)
            .SetEase(Ease.InOutSine)
            .OnWaypointChange(index => currentPathIndex = index)
            .OnUpdate(UpdateRotation)
            .OnComplete(() => isMoving = false);

        sequence.Append(move).AppendCallback(ThrowBack);
    }

    void UpdateRotation()
    {
        int lookIndex = Mathf.Clamp(currentPathIndex, 0, pathPositions.Length - 1);
        Vector3 dir = pathPositions[lookIndex] - transform.position;
        dir.y = 0f;
        if (dir.sqrMagnitude < 0.001f) return;

        Quaternion targetRot = Quaternion.LookRotation(dir) * Quaternion.Euler(rotationOffset);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Time.deltaTime * rotationSpeed);
    }

    void ThrowBack()
    {
        isMoving = false;
        manager?.DetachMovers(this);

        Vector3 mid = Vector3.Lerp(transform.position, originalGroundPosition, 0.5f) + Vector3.up * 3f;
        Vector3[] path = { mid, originalGroundPosition };

        Sequence throwSeq = DOTween.Sequence();
        throwSeq.Append(
            transform.DOPath(path, thrownbackDuration, PathType.CatmullRom).SetEase(Ease.InQuad)
        );
        throwSeq.Join(
            transform.DORotate(originalGroundRotation, thrownbackDuration).SetEase(Ease.Linear)
        );
        throwSeq.AppendCallback(() => manager?.OnGoalLanded(this));
    }

    void OnDestroy() => sequence?.Kill();
}