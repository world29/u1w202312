using UnityEngine;
using System.Collections;
using UnityEngine.Playables;

public class TimeMachineBehaviour : PlayableBehaviour
{
    public TimeMachineAction action;
    public Condition condition;
    public string markerToJumpTo, markerLabel;
    public Unity1Week.InputWait inputWait;

    public bool ConditionMet()
    {
        switch (condition)
        {
            case Condition.Always:
                return true;
            case Condition.IsInputWaiting:
                if (inputWait != null)
                {
                    return inputWait.IsWaiting;
                }
                else
                {
                    return false;
                }
            case Condition.Never:
            default:
                return false;
        }
    }

    public enum TimeMachineAction
    {
        Marker,
        JumpToMarker,
    }

    public enum Condition
    {
        Always,
        Never,
        IsInputWaiting,
    }
}
