using System.Linq;

public class TrialInfo
{
    public string mode;
    public bool leftLast;
    public bool TOJFeedback;
    public float soa;
    public bool probeFirstSelected;
    public double soaDuration;
    public string condition = "NULL";
    public int[][] boardPositions;
    public int firstSelectedPosition;
    public int secondSelectedPosition;
    public string toJSON()
    {
        condition = ExperimentController.GetInstance().platformCondition.ToString();
        return "{'boardConfig':[" +string.Join(",", boardPositions.Select((row) => "["+string.Join(",", row)+"]"))+"],'mode':'" + mode + "','leftLast':" + leftLast + ",'feedback':" + TOJFeedback + ", 'soa':'" + soa + "', 'probeFirstSelected':'" + probeFirstSelected + "', 'soaDuration':'" + soaDuration + "', 'condition':'" + condition + "','firstSelectedPosition':'" + firstSelectedPosition + "','secondSelectedPosition':'" + secondSelectedPosition + "' }";
    }
}