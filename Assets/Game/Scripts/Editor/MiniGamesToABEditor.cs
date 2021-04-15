using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using LitJson;
using System.IO;

namespace Wanderer.GameFramework
{
	public class MiniGamesToABEditor 
	{
		private const string _configName = "AssetBundleEditor.json";

		[MenuItem("Tools/Asset Bundle/Update MiniGames")]
		public static void Main()
		{
			JsonData config = ProjectSettingsConfig.LoadJsonData(_configName);
			if (config != null&& config.Count>0)
			{
				List<JsonData> datas = new List<JsonData>();
				for (int i = 0; i < config.Count; i++)
				{
					datas.Add(config[i]);
				}
				bool update = false;
				var folders = Directory.GetDirectories("Assets/Game/MiniGames");
				for (int i = 0; i < folders.Length; i++)
				{
					EditorUtility.DisplayProgressBar("MiniGames To AssetBundleEditor", folders[i], i /(folders.Length-1.0f));
					string abName = Path.GetFileNameWithoutExtension(folders[i]);
					JsonData getData = datas.Find(x => (abName.Equals((string)x["AssetBundleName"])));
					if (getData == null)
					{
						getData = new JsonData();
						getData["AssetBundleName"] = abName;
						getData["Variant"] = "";
						getData["Filter"] = 17410;
						getData["Split"] = false;
						getData["SplitCount"] = 1;
						getData["ForceUpdate"] = false;
						getData["Preload"] = false;
						getData["SearchInFolders"] = new JsonData();
						getData["SearchInFolders"].SetJsonType(JsonType.Array);
						getData["SearchInFolders"].Add(folders[i].Replace("\\","/"));
						datas.Add(getData);
						config.Add(getData);
						update = true;
					}
				}
				EditorUtility.ClearProgressBar();
				if (update)
				{
					ProjectSettingsConfig.SaveJsonData(_configName, config);
					EditorUtility.DisplayDialog("数据保存成功", _configName, "ok");
				}
			}
		}
	}
}