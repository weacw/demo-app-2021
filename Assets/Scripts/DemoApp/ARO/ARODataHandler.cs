using System.Collections.Generic;
using UnityEngine;
using Parse;

namespace Immersal.Samples.DemoApp.ARO
{
    public class ARODataHandler : MonoBehaviour
    {
        private string uid;

        public string Uid { get => uid; set => uid = value; }
        private IDictionary<string, object> currentData;
        public IDictionary<string, object> CurrentData { get => currentData; set => currentData = value; }
        
        public System.Action DataUpdated;

        public void UpdateData(IDictionary<string, object> newData)
        {
            CurrentData = newData;
            DataUpdated?.Invoke();
        }

        public void PushData()
        {
            AROManager.Instance.UpdateAROData(uid, currentData);
        }

        public void TryToMoveARO()
        {
            if (!IsLocked())
            {
                Lock();
                AROManager.Instance.PlaceAROWithPlacer(uid);
            }
        }

        public void MoveTo(Pose newWorldPose)
        {
            if (!IsLocked())
            {
                AROManager.Instance.PlaceARO(uid, newWorldPose);
            }
        }

        public void AROMoved()
        {
            Unlock();
        }

        public void TryToRemoveARO()
        {
            AROManager.Instance.DeleteARO(uid);
        }

        public void Lock()
        {
            currentData["Locked"] = ParseManager.Instance.parseClient.GetCurrentUser().Username;
            PushData();
        }

        public void Unlock()
        {
            currentData["Locked"] = "";
            PushData();
        }

        public bool IsLocked()
        {
            return currentData.ContainsKey("Locked") && (currentData["Locked"] as string) != "";
        }

        public void PushField(string field, string value)
        {
            currentData[field] = value;
            PushData();
        }
    }
}
