using System.Collections.Generic;
using System.Xml;
using UnityEngine;

namespace Text
{
	internal class TextLoadImpl
	{
		private Dictionary<string, CellDataList> m_categoryList;

		private bool m_setup;

		public TextLoadImpl()
		{
			m_categoryList = new Dictionary<string, CellDataList>();
		}

		public TextLoadImpl(ResourceSceneLoader sceneLoader, string fileName, string suffixe)
		{
			m_categoryList = new Dictionary<string, CellDataList>();
			LoadScene(sceneLoader, fileName, suffixe);
		}

		public string GetText(string categoryName, string cellName)
		{
			if (m_categoryList == null)
			{
				return null;
			}
			if (!m_categoryList.ContainsKey(categoryName))
			{
				Debug.LogWarning("Not Contains Key [GroupID]:" + categoryName + ", [CellID]:" + cellName);
				return null;
			}
			CellData cellData = m_categoryList[categoryName].Get(cellName);
			if (cellData == null)
			{
				Debug.LogWarning("Not Contains Key [GroupID]:" + categoryName + ", [CellID]:" + cellName);
				return null;
			}
			return cellData.m_cellString;
		}

		public int GetCellCount(string categoryName)
		{
			if (!m_categoryList.ContainsKey(categoryName))
			{
				return -1;
			}
			return m_categoryList[categoryName].GetCount();
		}

		public bool IsSetup()
		{
			return m_setup;
		}

		public void LoadScene(ResourceSceneLoader sceneLoader, string fileName, string suffixe)
		{
			if (sceneLoader != null)
			{
				bool onAssetBundle = true;
				string text = fileName + "_" + suffixe;
				GameObject x = GameObject.Find(text);
				if (x == null)
				{
					sceneLoader.AddLoad(text, onAssetBundle, false);
				}
			}
		}

		public void LoadResourcesSetup(string fileName, string language)
		{
			TextAsset textAsset = Resources.Load(fileName) as TextAsset;
			if (!(textAsset == null))
			{
				TextSetup(textAsset, language);
			}
		}

		public void LoadSceneSetup(string fileName, string language, string suffixe)
		{
			string name = fileName + "_" + suffixe;
			GameObject gameObject = GameObject.Find(name);
			if (!(gameObject == null))
			{
				AssetBundleTextData component = gameObject.GetComponent<AssetBundleTextData>();
				if (!(component == null))
				{
					TextSetup(component.m_TextAsset, language);
					Object.Destroy(gameObject);
				}
			}
		}

		private void TextSetup(TextAsset textData, string language)
		{
			string xml = AESCrypt.Decrypt(textData.text);
			XmlDocument xmlDocument = new XmlDocument();
			xmlDocument.LoadXml(xml);
			XmlNode documentElement = xmlDocument.DocumentElement;
			if (documentElement == null)
			{
				return;
			}
			XmlNodeList xmlNodeList = documentElement.SelectNodes("LanguageData");
			if (xmlNodeList == null)
			{
				return;
			}
			XmlNode xmlNode = null;
			for (int i = 0; i < xmlNodeList.Count; i++)
			{
				XmlNode xmlNode2 = xmlNodeList[i];
				if (xmlNode2 == null)
				{
					continue;
				}
				XmlNode namedItem = xmlNode2.Attributes.GetNamedItem("type");
				if (namedItem != null)
				{
					string value = namedItem.Value;
					if (value != null && value == language)
					{
						xmlNode = xmlNodeList[i];
						break;
					}
				}
			}
			if (xmlNode == null)
			{
				return;
			}
			XmlNode xmlNode3 = xmlNode.SelectSingleNode("CategoryList");
			if (xmlNode3 == null)
			{
				return;
			}
			XmlNodeList xmlNodeList2 = xmlNode3.SelectNodes("CategoryData");
			if (xmlNodeList2 == null)
			{
				return;
			}
			for (int j = 0; j < xmlNodeList2.Count; j++)
			{
				XmlNode xmlNode4 = xmlNodeList2[j];
				if (xmlNode4 != null)
				{
					XmlNode namedItem2 = xmlNode4.Attributes.GetNamedItem("ID");
					if (namedItem2 != null)
					{
						string value2 = namedItem2.Value;
						CreateCategoryData(xmlNode4.SelectSingleNode("CellList"), value2);
					}
				}
			}
			m_setup = true;
		}

		private void CreateCategoryData(XmlNode parentNode, string categoryName)
		{
			if (parentNode == null)
			{
				return;
			}
			XmlNodeList xmlNodeList = parentNode.SelectNodes("Cell");
			if (xmlNodeList == null)
			{
				return;
			}
			CellDataList cellDataList = new CellDataList(categoryName);
			for (int i = 0; i < xmlNodeList.Count; i++)
			{
				XmlNode xmlNode = xmlNodeList[i];
				if (xmlNode == null)
				{
					continue;
				}
				XmlNode namedItem = xmlNode.Attributes.GetNamedItem("ID");
				if (namedItem != null)
				{
					XmlNode namedItem2 = xmlNode.Attributes.GetNamedItem("String");
					if (namedItem2 != null)
					{
						CellData cellData = new CellData(namedItem.Value, namedItem2.Value);
						cellDataList.Add(cellData);
					}
				}
			}
			m_categoryList.Add(categoryName, cellDataList);
		}
	}
}
