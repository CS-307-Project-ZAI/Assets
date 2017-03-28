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

    public Quest(AllyController _questGiver, int _questID, QuestObjective _questObjective, int _objectiveID, int _completionAmmount)
    {
        questGiver = _questGiver;
        questID = _questID;
        completionAmmount = _completionAmmount;
        questObjective = _questObjective;
        objectiveID = _objectiveID;
        setName();
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
            case 1: return "zombie";
            case 2: return "wood";
            case 3: return "metal";
        }
        return "DOES NOT EXIST";
    }

    public int getObjectiveID() {
        return objectiveID;
    }

    public string getLogString() {
        string logString = questName + ": " + currentProgress + " of " + completionAmmount;
        if (hasBeenCompleted == true) logString = questName + ": complete";
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
        if (questObjective == QuestObjective.KILL)
        {
            questName = "Quest: Kill " + completionAmmount + " " + getNameofObjectiveID(objectiveID) + "s";
        }
        else if (questObjective == QuestObjective.COLLECT)
        {
            questName = "Quest: Collect " + completionAmmount + " " + getNameofObjectiveID(objectiveID) + "s";
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


