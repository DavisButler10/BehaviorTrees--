using UnityEngine;

public enum SteeringBehaviors
{
     Arrive, None
}

public class Kinematic : MonoBehaviour
{

    public Vector3 linear;
    public float angular; //degrees
    public GameObject newTarget;
    public float maxSpeed = 1.0f;
    public float maxAngularVelocity = 1.0f; //degrees

    public SteeringBehaviors choiceOfBehavior;

    public delegate void Arrived();
    public event Arrived OnArrived;

    SteeringOutput steeringUpdate = new SteeringOutput();
    Arrive arrive;

    void Start()
    {
        arrive = new Arrive();
        arrive.character = this;
        arrive.target = newTarget;
       
    }


    void Update()
    {
        switch (choiceOfBehavior)
        {
            case SteeringBehaviors.None:
                ResetOrientation();
                linear = Vector3.zero;
                break;
            default:
                MainSteeringBehaviors();
                break;
        }
    }

    void MainSteeringBehaviors()
    {
        ResetOrientation();

        switch (choiceOfBehavior)
        {
            
            case SteeringBehaviors.Arrive:
                arrive.target = newTarget;

                if (newTarget != null)
                {
                    if ((newTarget.transform.position - transform.position).magnitude < 1.5f)
                    {
                        OnArrived?.Invoke();
                    }
                }

                if (newTarget != null)
                {
                    //steeringUpdate = new SteeringOutput();
                    steeringUpdate = arrive.getSteering();
                }
                if (steeringUpdate != null)
                {
                    linear += steeringUpdate.linear * Time.deltaTime;
                    angular += steeringUpdate.angular * Time.deltaTime;
                }
                break;
        }

    }

    void ResetOrientation()
    {
        transform.position += linear * Time.deltaTime;
        Vector3 angularIncrement = new Vector3(0, angular * Time.deltaTime, 0);
        transform.eulerAngles += angularIncrement;
        
    }
}