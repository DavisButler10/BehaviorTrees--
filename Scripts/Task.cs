using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class Task
{
    public abstract void run();
    public bool succeeded;

    protected int eventId;
    const string EVENT_NAME_PREFIX = "FinishedTask";

    public string TaskFinished
    {
        get
        {
            return EVENT_NAME_PREFIX + eventId;
        }
    }
    public Task()
    {

        eventId = EventBus.GetEventID();
    }
}

public class IsTrue : Task
{
    bool varToTest;

    public IsTrue(bool someBool)
    {
        varToTest = someBool;

    }

    public override void run()
    {
        succeeded = varToTest;
        EventBus.TriggerEvent(TaskFinished);
    }
}


public class IsFalse : Task
{
    bool varToTest;

    public IsFalse(bool someBool)
    {
        varToTest = someBool;
    }

    public override void run()
    {
        succeeded = !varToTest;
        EventBus.TriggerEvent(TaskFinished);
    }
}

public class OrderFood : Task
{
    TextMesh textStuff;

    public OrderFood(TextMesh text)
    {
        textStuff = text;
    }

    public override void run()
    {
        textStuff.text = "Gimme Food!";
        succeeded = true;
        EventBus.TriggerEvent(TaskFinished);
    }
}

public class GiveFood : Task
{
    TextMesh textStuff;

    public GiveFood(TextMesh text)
    {
        textStuff = text;
    }

    public override void run()
    {
        textStuff.text = "Here's your food!";
        succeeded = true;
        EventBus.TriggerEvent(TaskFinished);
    }
}

public class OutOfFood : Task
{
    TextMesh textStuff;

    public OutOfFood(TextMesh text)
    {
        textStuff = text;
    }

    public override void run()
    {
        textStuff.text = "Sorry, but we are out of food. \nWould you like a slurpie instead?";
        succeeded = true;
        EventBus.TriggerEvent(TaskFinished);
    }
}

public class TakingLong: Task
{
    TextMesh textStuff;

    public TakingLong(TextMesh text)
    {
        textStuff = text;
    }

    public override void run()
    {
        textStuff.text = "She is taking too long! \n I'm leaving!!!";
        succeeded = true;
        EventBus.TriggerEvent(TaskFinished);
    }
}

public class OutOfSlurpie : Task
{
    TextMesh textStuff;

    public OutOfSlurpie(TextMesh text)
    {
        textStuff = text;
    }

    public override void run()
    {
        textStuff.text = "Sorry, but the machine is broken.\n";
        succeeded = true;
        EventBus.TriggerEvent(TaskFinished);
    }
}

public class HulkOut : Task
{
    TextMesh textStuff;
    GameObject karen;

    public HulkOut(TextMesh text, GameObject gameObject)
    {
        textStuff = text;
        karen = gameObject;
    }

    public override void run()
    {
        textStuff.text = "What! Do you know who I am! \n Let me speak to you manager!";
        karen.transform.localScale *= 2;
        succeeded = true;
        EventBus.TriggerEvent(TaskFinished);
    }
}

public class TakeSlurpie : Task
{
    TextMesh textStuff;

    public TakeSlurpie(TextMesh text)
    {
        textStuff = text;
    }

    public override void run()
    {
        textStuff.text = "Unbeleivable...but yes...";
        succeeded = true;
        EventBus.TriggerEvent(TaskFinished);
    }
}

public class EnterManager : Task
{
    Rigidbody managerRb;

    public EnterManager(Rigidbody rigidbody)
    {
        managerRb = rigidbody;
    }

    public override void run()
    {
        //textStuff.text = "Unbeleivable...but yes...";
        managerRb.useGravity = true;
        succeeded = true;
        EventBus.TriggerEvent(TaskFinished);
    }
}

public class KillKaren : Task
{
    GameObject karen;
    Animator anim;

    public KillKaren(GameObject gameobject, Animator animator)
    {
        karen = gameobject;
        anim = animator;
    }

    public override void run()
    {
        karen.transform.localScale = new Vector3(2, 2, -2);
        anim.SetBool("ded", true);
        succeeded = true;
        EventBus.TriggerEvent(TaskFinished);
    }
}

public class Wait : Task
{
    float waitTime;
    //TextMesh textCust;
    //TextMesh textEmp;

    public Wait(float time/*TextMesh text, TextMesh text2*/)
    {
        waitTime = time;
        //textCust = text;
        //textEmp = text2;
    }

    public override void run()
    {
        succeeded = true;
        EventBus.ScheduleTrigger(TaskFinished, waitTime);
        //textCust.text = "";
        //textEmp.text = "";
    }
}

public class MoveKinematicToObject : Task
{
    Kinematic movie;
    GameObject leTarget;
    Animator anim;
    TextMesh leText;

    public MoveKinematicToObject(Kinematic mover, GameObject target, TextMesh text)
    {
        movie = mover;
        leTarget = target;
        leText = text;
        anim = movie.GetComponent<Animator>();
    }

    public override void run()
    {
        Debug.Log("Moving to target position: " + leTarget);
        Debug.Log(leTarget.name);
        if(leTarget.name == "StartPoint")
        {
            movie.transform.localScale = new Vector3(1, 1, -1);
            leText.transform.localScale = new Vector3(-1, 1, 1);
        }else if (leTarget.name == "MidPoint"){
            movie.transform.localScale = new Vector3(2, 2, -2);
            leText.transform.localScale = new Vector3(-1, 1, 1);
        }
        else
        {
            movie.transform.localScale = new Vector3(1, 1, 1);
            leText.transform.localScale = new Vector3(1, 1, 1);
        }
        movie.choiceOfBehavior = SteeringBehaviors.Arrive;
        anim.SetBool("walk", true);
        movie.OnArrived += MovieArrived;
        movie.newTarget = leTarget;
    }

    public void MovieArrived()
    {
        Debug.Log("arrived at " + leTarget);
        movie.choiceOfBehavior = SteeringBehaviors.None;
        movie.OnArrived -= MovieArrived;
        anim.SetBool("walk", false);
        succeeded = true;
        EventBus.TriggerEvent(TaskFinished);
    }
}

public class Sequence : Task
{
    List<Task> children;
    Task currentTask;
    int currentTaskIndex = 0;

    public Sequence(List<Task> taskList)
    {
        children = taskList;
    }

    public override void run()
    {
        currentTask = children[currentTaskIndex];
        EventBus.StartListening(currentTask.TaskFinished, OnChildTaskFinished);
        currentTask.run();
    }

    void OnChildTaskFinished()
    {
        if (currentTask.succeeded)
        {
            EventBus.StopListening(currentTask.TaskFinished, OnChildTaskFinished);
            currentTaskIndex++;
            if (currentTaskIndex < children.Count)
            {
                this.run();
            }
            else
            {
                succeeded = true;
                EventBus.TriggerEvent(TaskFinished);
            }

        }
        else
        {
            succeeded = false;
            EventBus.TriggerEvent(TaskFinished);
        }
    }
}

public class Selector : Task
{
    List<Task> children;
    Task currentTask;
    int currentTaskIndex = 0;

    public Selector(List<Task> taskList)
    {
        children = taskList;
    }

    public override void run()
    {
        currentTask = children[currentTaskIndex];
        EventBus.StartListening(currentTask.TaskFinished, OnChildTaskFinished);
        currentTask.run();
    }

    void OnChildTaskFinished()
    {
        if (currentTask.succeeded)
        {
            succeeded = true;
            EventBus.TriggerEvent(TaskFinished);
        }
        else
        {
            EventBus.StopListening(currentTask.TaskFinished, OnChildTaskFinished);
            currentTaskIndex++;
            if (currentTaskIndex < children.Count)
            {
                this.run();
            }
            else
            {
                succeeded = false;
                EventBus.TriggerEvent(TaskFinished);
            }
        }
    }
}




