using System.Collections;
using UnityEngine;
using UnityEngine.Splines;


public class MovingCubeTest : MonoBehaviour
{
   public SplineAnimate splineAnimate;
   public Transform transformToCompare;
   public float length = 100;
   public float speed = 1;

   public float margin;
   public bool horizontallyAligned;
   public bool verticallyAligned;

   private void OnDrawGizmos()
   {
      var direction = gameObject.transform.right;
      Gizmos.DrawRay(transform.position, direction.normalized * length);
      if (transformToCompare != null)
      {
         horizontallyAligned = IsHorizontallyAligned(this.transform.position, transformToCompare.position, margin);
         verticallyAligned = IsVerticallyAligned(this.transform.position, transformToCompare.position, margin);
      }
   }

   private void Update()
   {
      if (transformToCompare == null) return;
      
      horizontallyAligned = IsHorizontallyAligned(this.transform.position, transformToCompare.position, margin);
      verticallyAligned = IsVerticallyAligned(this.transform.position, transformToCompare.position, margin);
      if (horizontallyAligned || verticallyAligned)
      {
         StartCoroutine(MoveToCenter());
      }
   }

   private IEnumerator MoveToCenter()
   {
      splineAnimate.Pause();
      var step =  speed * Time.deltaTime;
      while (Vector3.Distance(transform.position, transformToCompare.position) > 0.001f)
      {
         transform.position = Vector3.MoveTowards(transform.position, transformToCompare.position, step);
         yield return null;
      }
   }

   private bool IsHorizontallyAligned(Vector3 localPos, Vector3 comparePos, float margin) => Mathf.Abs(localPos.z - comparePos.z) <= margin;

   private bool IsVerticallyAligned(Vector3 localPos, Vector3 comparePos, float margin) => Mathf.Abs(localPos.x - comparePos.x) <= margin;
}