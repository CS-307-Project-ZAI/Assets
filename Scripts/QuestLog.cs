using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestLog : MonoBehaviour {

    public PlayerController questLogOwner;
    public int questLogSize = 5;
    public List<Quest> quests = new List<Quest>();

    public void addKill(int id) {
        foreach (Quest q in quests) {
            if (!q.isComplete() && q.getQuestObjective() == QuestObjective.KILL && q.getObjectiveID() == id) {
                q.addProgress();
            }
        }
    }

    public Quest questAt(int index) {
        if (index < quests.Count && index >= 0) {
            return quests[index];
        }
        return null;
    }

    public void addCollect(int id)
    {
        foreach (Quest q in quests)
        {
            if (!q.isComplete() && q.getQuestObjective() == QuestObjective.COLLECT && q.getObjectiveID() == id)
            {
                q.addProgress();
            }
        }
    }

	public void itemCollected(int itemId) {
		foreach (Quest q in quests) {
			if (!q.isComplete () && q.getQuestObjective () == QuestObjective.COLLECT) {

			}
		}
	}

    public string questLogString(Quest q) {
        return q.getLogString();
    }

	public bool addQuest(Quest q) {
        if (quests.Count < questLogSize) {
            quests.Add(q);
			return true;
        }
        //error message quest log is full
        Debug.Log("quest log is full");
		return false;
    }

    public Quest getQuest(int id) {
        foreach (Quest q in quests) {
            if (q.getQuestID() == id) {
                return q;
            }
        }
        return null;
    }

    public void turninQuest(AllyController turninTo, int questID) {
        Quest q = getQuest(questID);
        if (q != null)
        {
            if (q.getQuestGiver() == turninTo)
            {
                if (q.isComplete())
                {
                    q.giveReward(questLogOwner);
                    quests.Remove(q);
                }
                else {
                    Debug.Log("Quest not yet ready to turn in");
                }
            }
            else {
                //quest not turned in to correct person
                Debug.Log("Quest not turned in to correct person");
            }
        }
        else {
            //quest does not exist in log
            Debug.Log("Quest not found in questlog");
        }
    }
}
