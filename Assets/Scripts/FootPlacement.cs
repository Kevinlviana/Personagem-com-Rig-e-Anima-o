using UnityEngine;

public class FootPlacement : MonoBehaviour
{
    [SerializeField] Transform leftFootT, rightFootT;
    [SerializeField] Transform leftFootTarget, rightFootTarget;
    [SerializeField] LayerMask groundLayer;

    void Start()
    {
        Animator animator = GetComponentInParent<Animator>();
        if (animator == null) animator = transform.root.GetComponentInChildren<Animator>();

        if (animator != null && animator.isHuman)
        {
            if (leftFootT == null) leftFootT = animator.GetBoneTransform(HumanBodyBones.LeftFoot);
            if (rightFootT == null) rightFootT = animator.GetBoneTransform(HumanBodyBones.RightFoot);
        }

        if (leftFootT == null || rightFootT == null || leftFootTarget == null || rightFootTarget == null)
        {
            enabled = false;
        }
    }

    private void LateUpdate()
    {
        LeftFootUpdate();
        RightFootUpdate();
    }

    void LeftFootUpdate()
    {
        Vector3 animatedPosition = leftFootT.position;
        Vector3 origin = animatedPosition + Vector3.up * 0.3f;

        RaycastHit hit;
        if (Physics.Raycast(origin, Vector3.down, out hit, 1.5f, groundLayer))
        {
            float heightDifference = animatedPosition.y - hit.point.y;

            if (heightDifference < 0.15f)
            {
                Vector3 targetPosition = hit.point + Vector3.up * 0.1f;

                leftFootTarget.position =
                    Vector3.Lerp(leftFootTarget.position, targetPosition, Time.deltaTime * 15f);
            }
            else
            {
                leftFootTarget.position = animatedPosition;
            }
        }
    }

    void RightFootUpdate()
    {
        Vector3 animatedPosition = rightFootT.position;
        Vector3 origin = animatedPosition + Vector3.up * 0.3f;

        RaycastHit hit;
        if (Physics.Raycast(origin, Vector3.down, out hit, 1.5f, groundLayer))
        {
            float heightDifference = animatedPosition.y - hit.point.y;

            if (heightDifference < 0.15f)
            {
                Vector3 targetPosition = hit.point + Vector3.up * 0.05f;

                rightFootTarget.position =
                    Vector3.Lerp(rightFootTarget.position, targetPosition, Time.deltaTime * 15f);
            }
            else
            {
                rightFootTarget.position = animatedPosition;
            }
        }
    }
}
