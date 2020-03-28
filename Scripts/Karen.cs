using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Karen : MonoBehaviour
{
    bool executingBehavior = false;
    Task currentTask;
    public GameObject stopPoint;
    public GameObject midPoint;
    public GameObject startPoint;
    public TextMesh text;
    public TextMesh textEmp;
    //public Food theFood;
    public bool OutOfFood = false;
    public bool MachineBroken = false;
    public Dropdown dropdown1;
    public Dropdown dropdown2;
    public Rigidbody managerRb;
    public Animator karenAnim;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            if (!executingBehavior)
            {
                executingBehavior = true;
                currentTask = BuildTask();

                EventBus.StartListening(currentTask.TaskFinished, OnTaskFinished);
                currentTask.run();
            }
        }
    }

    void OnTaskFinished()
    {
        EventBus.StopListening(currentTask.TaskFinished, OnTaskFinished);
        executingBehavior = false;
    }

    public void SetBools()
    {
        Debug.Log(dropdown1.value);
        if(dropdown1.value == 0)
        {
            OutOfFood = false;
        }
        else
        {
            OutOfFood = true;
        }
        if (dropdown2.value == 0)
        {
            MachineBroken = false;
        }
        else
        {
            MachineBroken = true;
        }
    }

    Task BuildTask()
    {
        List<Task> taskList = new List<Task>();

        Task isOutOfFood = new IsTrue(OutOfFood);
        Task machineBroken = new IsTrue(MachineBroken);
        Task walkToCounter = new MoveKinematicToObject(this.GetComponent<Kinematic>(), stopPoint.gameObject, text);
        Task walkFromCounter = new MoveKinematicToObject(this.GetComponent<Kinematic>(), startPoint.gameObject, text);
        Task walkFromCounterMid = new MoveKinematicToObject(this.GetComponent<Kinematic>(), midPoint.gameObject, text);
        Task wait = new Wait(1.5f);
        Task orderFood = new OrderFood(text);
        Task giveFood = new GiveFood(textEmp);
        Task takeSlurpie = new TakeSlurpie(text);
        Task takingLong = new TakingLong(text);
        Task outOfFood = new OutOfFood(textEmp);
        Task outOfSlurpie = new OutOfSlurpie(textEmp);
        Task hulkOut = new HulkOut(text, this.gameObject);
        Task enterManager = new EnterManager(managerRb);
        Task killKaren = new KillKaren(this.gameObject, karenAnim);

        //if out of food, yell and get slurpie
        taskList.Add(isOutOfFood);
        taskList.Add(walkToCounter);
        taskList.Add(wait);
        taskList.Add(orderFood);
        taskList.Add(wait);
        taskList.Add(outOfFood);
        taskList.Add(wait);
        taskList.Add(takeSlurpie);
        taskList.Add(wait);
        taskList.Add(walkFromCounter);


        Sequence sequenceSlurpie = new Sequence(taskList);

        taskList = new List<Task>();

        //if slurpie machine broken
        taskList.Add(machineBroken);
        taskList.Add(walkToCounter);
        taskList.Add(wait);
        taskList.Add(orderFood);
        taskList.Add(wait);
        taskList.Add(outOfSlurpie);
        taskList.Add(wait);
        taskList.Add(hulkOut);
        taskList.Add(wait);
        taskList.Add(wait);
        taskList.Add(takingLong);
        taskList.Add(enterManager);
        taskList.Add(walkFromCounterMid);
        taskList.Add(killKaren);

        Sequence sequenceKaren = new Sequence(taskList);


        taskList = new List<Task>();

        //if else walk and get food
        taskList.Add(walkToCounter);
        taskList.Add(wait);
        taskList.Add(orderFood);
        taskList.Add(wait);
        taskList.Add(giveFood);
        taskList.Add(wait);
        taskList.Add(walkFromCounter);

        Sequence sequenceNormal = new Sequence(taskList);

        taskList = new List<Task>();

        taskList.Add(sequenceSlurpie);
        taskList.Add(sequenceKaren);
        taskList.Add(sequenceNormal);

        Selector selector = new Selector(taskList);

        return selector;
    }
}
