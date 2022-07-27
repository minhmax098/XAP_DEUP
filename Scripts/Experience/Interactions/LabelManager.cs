using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LabelManager : MonoBehaviour
{
    private static LabelManager instance;
    public static LabelManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<LabelManager>();
            }
            return instance;
        }
    }

    private const float LONG_LINE_FACTOR = 6f;
    private string levelObject = "";

    // UI
    public Button btnLabel;
    private bool isShowingLabel;
    public bool IsShowingLabel
    {
        get
        {
            return isShowingLabel;
        }
        set
        {
            isShowingLabel = value;
            btnLabel.GetComponent<Image>().sprite = isShowingLabel ? Resources.Load<Sprite>(PathConfig.LABEL_CLICKED_IMAGE) : Resources.Load<Sprite>(PathConfig.LABEL_UNCLICK_IMAGE);
        }
    }

    public void HandleLabelView(bool currentLabelStatus)
    {
        IsShowingLabel = currentLabelStatus;
        if (IsShowingLabel)
        {
            btnLabel.interactable = false;
            CreateLabel();
            btnLabel.interactable = true;
        }
        else
        {
            btnLabel.interactable = false;
            ClearLabel();
            btnLabel.interactable = true;
        }
    }

    public bool CheckAvailableLabel(GameObject obj)
    {
        if (StaticLesson.ListLabel.Length <= 0)
            return false;

        string levelObject = Helper.GetLevelObjectInLevelParent(obj);
        foreach (Label item in StaticLesson.ListLabel)
        {
            if (item.level == levelObject)
                return true;
        }
        return false;
    }

    public void CreateLabel()
    {
        ClearLabel();
        foreach (Label item in StaticLesson.ListLabel)
        {
            levelObject = Helper.GetLevelObjectInLevelParent(ObjectManager.Instance.CurrentObject);
            if (item.level == levelObject)
            {
                GameObject tag = Instantiate(Resources.Load(PathConfig.MODEL_TAG_LABEL) as GameObject);
                tag.tag = TagConfig.LABEL_TAG;
                tag.transform.SetParent(ObjectManager.Instance.CurrentObject.transform, false);

                tag.transform.localScale = tag.transform.localScale / (ObjectManager.Instance.OriginObject.transform.localScale.x / ObjectManager.Instance.OriginScale.x);

                tag.transform.GetChild(1).localScale = tag.transform.GetChild(1).localScale / ObjectManager.Instance.FactorScaleInitial;
                SetLabel(ObjectManager.Instance.CurrentObject, tag, item);
                TagHandler.Instance.AddTag(tag);
            }
        }
    }

    public void ClearLabel()
    {
        foreach (GameObject label in TagHandler.Instance.addedTags)
        {
            Destroy(label);
        }
        TagHandler.Instance.DeleteTags();
    }

    public void SetLabel(GameObject childObject, GameObject tag, Label label)
    {
        GameObject line = tag.transform.GetChild(0).gameObject;
        GameObject labelName = tag.transform.GetChild(1).gameObject;
        labelName.transform.GetChild(0).GetComponent<TMPro.TextMeshProUGUI>().text = label.labelName;

        Vector3 centerPointChildObject = Helper.CalculateBounds(childObject).center;
        Vector3 pointClickInObject = new Vector3(label.coordinates.x, label.coordinates.y, label.coordinates.z);

        labelName.transform.localPosition = pointClickInObject * (ObjectManager.Instance.OriginObject.transform.localScale.x / ObjectManager.Instance.OriginScale.x);
        TagHandler.Instance.positionOriginLabel.Add(pointClickInObject);

        line.GetComponent<LineRenderer>().SetPosition(1, labelName.transform.localPosition);
    }
}