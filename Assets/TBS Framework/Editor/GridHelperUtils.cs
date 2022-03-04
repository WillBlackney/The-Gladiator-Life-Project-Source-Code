using System.Collections.Generic;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace TbsFramework.EditorUtils
{
    public class GridHelperUtils
    {
        public static bool CheckMissingParameters(Dictionary<string, object> parameterValues)
        {
            List<string> missingParams = new List<string>();
            foreach (var entry in parameterValues)
            {
                if (entry.Value == null)
                {
                    missingParams.Add(entry.Key);
                }
            }

            if (missingParams.Count != 0)
            {
                string dialogTitle = string.Format("Parameter{0} missing", missingParams.Count > 1 ? "s" : "");

                StringBuilder dialogMessage = new StringBuilder();
                dialogMessage.AppendFormat("Please fill in the missing parameter{0} first:\n", missingParams.Count > 1 ? "s" : "");

                foreach (var missingParam in missingParams)
                {
                    dialogMessage.AppendLine(string.Format("   -{0}", missingParam));
                }

                string dialogOK = "Ok";
                EditorUtility.DisplayDialog(dialogTitle, dialogMessage.ToString(), dialogOK);
                return true;
            }
            return false;
        }
        public static void ClearScene()
        {
            var objects = GameObject.FindObjectsOfType<GameObject>();
            var toDestroy = new List<GameObject>();

            foreach (var obj in objects)
            {
                bool isChild = obj.transform.parent != null;

                if (isChild)
                    continue;

                toDestroy.Add(obj);
            }
            toDestroy.ForEach(o => GameObject.DestroyImmediate(o));
        }
    }
}