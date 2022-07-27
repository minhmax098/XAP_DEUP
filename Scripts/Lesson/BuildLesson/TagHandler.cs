using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace BuildLesson
{
    public class TagHandler : MonoBehaviour
    {
        // Tag Handler use to hanlder: - All NormalTag and One 2DTag(with the index)
        private static TagHandler instance;
        public static TagHandler Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindObjectOfType<TagHandler>();
                }
                return instance;
            }
        }
        public List<GameObject> addedTags = new List<GameObject>();
        public List<int> labelIds = new List<int>();
        public GameObject labelEditTag;
        public int currentEditingIdx = -1; 
        private Vector2 rootLabel2D;
        private Vector3 originLabelScale;
        public List<Vector3> positionOriginLabel = new List<Vector3>();

        void Update()
        {
            OnMove();
        }

        // function called along with the setter of labelEditTag
        public void updateCurrentEditingIdx(string organName)
        {
            // This function use to update the labelEditTag corresponding to current selected label
            // Use both labelEditTag and the currentEditingIdx to handle
            // labelEditTag can directly pass into the class instance 
            // This function is used to find the index of corresponding normal label that the labelEditTag belong to
            foreach(GameObject tag in addedTags)
            {
                // Debug.Log("Traversing tags " + tag.transform.GetChild(1).GetChild(0).gameObject.GetComponent<TMP_Text>().text);
                Debug.Log("Traversing tags " + tag.transform.GetChild(1).GetChild(0).gameObject.GetComponent<Text>().text);
                // if (tag.transform.GetChild(1).GetChild(0).gameObject.GetComponent<TMP_Text>().text == organName)
                if (tag.transform.GetChild(1).GetChild(0).gameObject.GetComponent<Text>().text == organName)
                {  
                    Debug.Log("Traversing hit: " + organName); // The currentEditingIdx is updated 
                    currentEditingIdx = addedTags.IndexOf(tag);
                }
            }
        }

        public void deleteCurrentLabel()
        {
            // Remove the Label as well as the correspondingId 
            GameObject x = addedTags[currentEditingIdx];
            Destroy(x);
            addedTags.RemoveAt(currentEditingIdx);
            labelIds.RemoveAt(currentEditingIdx);
            
            currentEditingIdx = -1;
        }

        public void ResetEditLabelIndex()
        {
            currentEditingIdx = -1;
        }

        public void AddLabelId(int labelId)
        {
            // the same shape as addedTags, will be called after the created request successful 
            labelIds.Add(labelId);
            Debug.Log("ADD AUDIO ADD LABELID, length: " + labelIds.Count);
        }

        public void AddTag(GameObject tag)
        {
            // Add Tag mean add NormalTag, happened when created a new label, so update the currentEditingIndx
            addedTags.Add(tag);
            originLabelScale = tag.transform.localScale;

            // Found an easier way to do 
            currentEditingIdx = addedTags.Count - 1;
        }


        public void DeleteTags()
        {
            // Reset the value 
            addedTags.Clear();
            currentEditingIdx = -1;
        }

        public void OnMove()
        {
            foreach (GameObject addedTag in addedTags)
            {
                if (addedTag != null)
                {
                    DenoteTag(addedTag);
                    MoveTag(addedTag);
                }
                // Handler the display of the correponding 2Dlabel 
                if (currentEditingIdx != -1)
                {
                    Update2DLabelPosition();
                }
            }
        }

        public void Update2DLabelPosition()
        {
            // Based on the currentEditingIdx, get position of the NormalLabel
            rootLabel2D = Camera.main.WorldToScreenPoint(addedTags[currentEditingIdx].transform.GetChild(1).gameObject.transform.position);
            labelEditTag.transform.position = rootLabel2D;
        }

        public void DenoteTag(GameObject addedTag)
        {
            if (addedTag.transform.GetChild(1).transform.position.z > 1f)
            {
                addedTag.transform.GetChild(0).gameObject.GetComponent<LineRenderer>().enabled = false;
                addedTag.transform.GetChild(1).gameObject.GetComponent<MeshRenderer>().enabled = false;
                addedTag.transform.GetChild(1).GetChild(0).gameObject.GetComponent<MeshRenderer>().enabled = false;
            }
            else
            {
                addedTag.transform.GetChild(0).gameObject.GetComponent<LineRenderer>().enabled = true;
                addedTag.transform.GetChild(1).gameObject.GetComponent<MeshRenderer>().enabled = true;
                addedTag.transform.GetChild(1).GetChild(0).gameObject.GetComponent<MeshRenderer>().enabled = true;
            }
        }

        public void MoveTag(GameObject addedTag)
        {
            addedTag.transform.GetChild(1).transform.LookAt(addedTag.transform.GetChild(1).position + Camera.main.transform.rotation * Vector3.forward, Camera.main.transform.rotation * Vector3.up);
            addedTag.transform.GetChild(1).GetChild(0).transform.LookAt(addedTag.transform.GetChild(1).GetChild(0).position + Camera.main.transform.rotation * Vector3.forward, Camera.main.transform.rotation * Vector3.up);
        }
        
        public void ShowHideCurrentLabel(bool showLabel)
        {
            if (currentEditingIdx != -1)
            {
                addedTags[currentEditingIdx].transform.GetChild(1).gameObject.SetActive(showLabel);
            }
        }

        public void ShowHideAllLabels(bool showLabel)
        {
            // Always show the label at currentIdx
            for (int i=0; i < addedTags.Count; i++)
            {
                if ( i != currentEditingIdx)
                {
                    addedTags[i].SetActive(showLabel);
                }
            }
        }

        public void AdjustTag(float scaleFactor)
        {
            for (int i=0; i< addedTags.Count; i++)
            {
                // addedTags[i].transform.GetChild(0).gameObject.GetComponent<LineRenderer>().SetPosition(0, addedTags[i].transform.InverseTransformPoint(hitPoint));
                // addedTags[i].transform.GetChild(0).gameObject.GetComponent<LineRenderer>().SetPosition(1, addedTags[i].transform.InverseTransformPoint(labelName.transform.position));
                
                addedTags[i].transform.localScale = originLabelScale / scaleFactor;
                addedTags[i].transform.GetChild(1).localPosition = positionOriginLabel[i];
                // ObjectManagerBuildLesson.Instance.OriginScale.x / ObjectManagerBuildLesson.Instance.OriginObject.transform.localScale.x * ObjectManagerBuildLesson.Instance.OriginScale.x
                addedTags[i].transform.GetChild(0).GetComponent<LineRenderer>().SetPosition(1, addedTags[i].transform.GetChild(1).localPosition * 0.9f);
            }
           
        }

        public void ShowHideTags(bool isShowing)
        {
            foreach(GameObject tag in addedTags)
            {
                tag.SetActive(isShowing);
            }
        }
    }
}

