using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum QuestObjective { KILL, COLLECT, OTHER };

public class Quest{
    private QuestObjective questObjective;
    private int questID;
    private string questName;
    private int currentProgress = 0;
    private int completionAmmount;
    private bool hasBeenCompleted = false;
    private int objectiveID;
    private string questText = "";
    private AllyController questGiver;
	private static List<int> idList = new List<int>();

    public Quest(AllyController _questGiver, QuestObjective _questObjective, int _objectiveID, int _completionAmmount) {
		do {
			questID = Random.Range(0, 1000);
		} while (idList.Contains(questID));
		Quest.idList.Add (questID);

		questGiver = _questGiver;
        questObjective = _questObjective;
		Debug.Log ("Quest Objective: " + questObjective);
        objectiveID = _objectiveID;
		Debug.Log ("ObjectiveID: " + objectiveID);
		completionAmmount = _completionAmmount;
        setName();
    }

	public static void initRandomQuest(AllyController owner) {
		if (owner.questToGive == null && owner.leader == null) {
			QuestObjective randObjective = (QuestObjective)Random.Range (0, System.Enum.GetValues(typeof(QuestObjective)).Length - 1);
			owner.questToGive = new Quest(owner, randObjective, (randObjective.Equals(QuestObjective.KILL) ? 1 : Random.Range(2, 4)), (int)Random.Range(3, 10));
		}
	}

    public void addProgress() {
        if (!hasBeenCompleted) {
            currentProgress++;
            if (currentProgress >= completionAmmount) completeQuest();
        } 
    }

    //return name from item id
    private string getNameofObjectiveID(int id) {
        switch (id) {
            case 1: return "Zombie";
            case 2: return "Wood";
            case 3: return "Metal";
        }
        return "DOES NOT EXIST";
    }

    public int getObjectiveID() {
        return objectiveID;
    }

    public string getLogString() {
        string logString = questName + ": " + currentProgress + " of " + completionAmmount;
        if (hasBeenCompleted == true) logString = questName + ": Complete";
        return logString;
    }

    public bool isComplete() {
        return hasBeenCompleted;
    }

    public void giveReward(PlayerController p) {
        questGiver.becomeFollower(p);
        p.addAlly(questGiver);
        //add items that are in the reward list as well
    }

    public string getQuestName() {
        return questName;
    }

    public int getCompletionAmmount() {
        return completionAmmount;
    }

    public int getCurrentProgress() {
        return currentProgress;
    }

    public void setName() {
        if (questObjective == QuestObjective.KILL) {
            questName = "Quest: Kill " + completionAmmount + " " + getNameofObjectiveID(objectiveID) + "s";
        } 
		else if (questObjective == QuestObjective.COLLECT) {
            questName = "Quest: Collect " + completionAmmount + " " + getNameofObjectiveID(objectiveID);
        }
        else {
            questName = "Quest: Other";
        }
    }

    public AllyController getQuestGiver() {
        return questGiver;
    }

    public void completeQuest() {
        hasBeenCompleted = true;
    }

    public int getQuestID() {
        return questID;
    }

    public QuestObjective getQuestObjective() {
        return questObjective;
    }

    public void setQuestText(string text) {
        questText = text;
    }
	

}


