using UnityEngine;

public class CameraControl : MonoBehaviour
{
    public float m_DampTime = 0.2f;                 
    public float m_ScreenEdgeBuffer = 4f;           
    public float m_MinSize = 6.5f;                  
    /*[HideInInspector]*/ public Transform[] m_Targets; // array of transforms


    private Camera m_Camera;                        
    private float m_ZoomSpeed;                      
    private Vector3 m_MoveVelocity;                 
    private Vector3 m_DesiredPosition;              


    private void Awake()
    {
        // set up referrance
        m_Camera = GetComponentInChildren<Camera>(); // only find the first one
    }


    private void FixedUpdate()
    {
        // update every physical step
        Move();
        Zoom();
    }


    private void Move()
    {
        FindAveragePosition();
     
        transform.position = Vector3.SmoothDamp(transform.position, m_DesiredPosition, ref m_MoveVelocity, m_DampTime);
    }


    private void FindAveragePosition()
    {
        // add every tank's position to an array, devide by tank numbers, get the average
        // input check
        if (m_Targets.Length == 0)
        {
            return;
        }
        Vector3 positionSum = new Vector3(0, 0, 0);
        int tank_num = 0;

        for (int i = 0; i < m_Targets.Length; i ++)
        {
            if (!m_Targets[i].gameObject.activeSelf)
            {
                continue;
            }
            positionSum += m_Targets[i].position;
            tank_num += 1;
        }
        Vector3 averagePos = new Vector3(0, 0, 0);
        averagePos = positionSum / tank_num;
        averagePos.y = transform.position.y;
        m_DesiredPosition = averagePos;
    }


    private void Zoom()
    {
        float requiredSize = FindRequiredSize();
        m_Camera.orthographicSize = Mathf.SmoothDamp(m_Camera.orthographicSize, requiredSize, ref m_ZoomSpeed, m_DampTime);
    }


    private float FindRequiredSize()
    {
        float size = 0.0f;
        Vector3 locDesiredPos = transform.InverseTransformPoint(m_DesiredPosition);

        // find max size
        for (int i = 0; i < m_Targets.Length; i ++)
        {
            // check if target is active
            if (!m_Targets[i].gameObject.activeSelf)
            {
                continue;
            }
            Vector3 locTankPos = transform.InverseTransformPoint(m_Targets[i].position);
            Vector3 tankToCamera = locDesiredPos - locTankPos;
            size = Mathf.Max(size, Mathf.Abs(tankToCamera.y));
            size = Mathf.Max(size, Mathf.Abs(tankToCamera.x * m_Camera.aspect));
        }
        size += m_ScreenEdgeBuffer;
        size = Mathf.Max(size, m_MinSize);



        return size;
        //Vector3 desiredLocalPos = transform.InverseTransformPoint(m_DesiredPosition); // transform from world to local 

        //float size = 0f;

        //for (int i = 0; i < m_Targets.Length; i++)
        //{
        //    if (!m_Targets[i].gameObject.activeSelf)
        //        continue;

        //    Vector3 targetLocalPos = transform.InverseTransformPoint(m_Targets[i].position);

        //    Vector3 desiredPosToTarget = targetLocalPos - desiredLocalPos;

        //    size = Mathf.Max (size, Mathf.Abs (desiredPosToTarget.y));

        //    size = Mathf.Max (size, Mathf.Abs (desiredPosToTarget.x) / m_Camera.aspect);
        //}
        
        //size += m_ScreenEdgeBuffer;

        //size = Mathf.Max(size, m_MinSize);

        //return size;
    }


    public void SetStartPositionAndSize()
    {
        // find the inital pos and size and set camera as is 
        FindAveragePosition();
        transform.position = m_DesiredPosition;
        m_Camera.orthographicSize = FindRequiredSize();
    }
}