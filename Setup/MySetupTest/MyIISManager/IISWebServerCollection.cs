using System;
using System.Collections;
using System.Text;

namespace MyIISManager
{
	/// <summary>
	/// IISWebServerCollection 
	/// </summary>
	public class IISWebServerCollection : CollectionBase
	{
		#region  "������ʵ������"
		/// <summary>
		/// ͨ����Ž�������
		/// </summary>
		public IISWebServer this[int Index]
		{
			get
			{
				return (IISWebServer)this.List[Index];

			}
		}

		
		/// <summary>
		/// ͨ����վ����������
		/// </summary>
		public IISWebServer this[string sitName]
		{
			get
			{
				sitName = sitName.ToLower().Trim();
				IISWebServer list;
				for (int i = 0; i < this.List.Count; i++)
				{
					list = (IISWebServer)this.List[i];
					if (list.SiteName.ToLower().Trim() == sitName)
						return list;
				}
				return null;
			}
		}
		#endregion


		#region "������ʵ������"
		/// <summary>
		/// ��Ӽ���Ԫ��
		/// </summary>
		public void Add(IISWebServer webServer)
		{
			this.List.Add(webServer);
		}


		/// <summary>
		/// �������Ӽ���Ԫ��
		/// </summary>
		/// <param name="WebServers"></param>
		public void AddRange(IISWebServer[] webServers)
		{
			for (int i = 0; i < webServers.Length; i++)
			{
				this.List.Add(webServers[i]);
			}
		}


		/// <summary>
		/// ɾ������Ԫ��
		/// </summary>
		public void Remove(IISWebServer webServer)
		{
			this.List.Remove(webServer);					
		}


		/// <summary>
		/// �Ƿ����ָ�����Ƶ���վ
		/// </summary>
		public bool Contains(IISWebServer webServer)
		{
			return this.List.Contains(webServer);
		}

		/// <summary>
		/// �Ƿ����ָ�����Ƶ���վ
		/// </summary>
		/// <param name="ServerComment"></param>
		/// <returns></returns>
		public bool Contains(string sitName)
		{
			sitName = sitName.ToLower().Trim();
			for (int i = 0; i < this.List.Count; i++)
			{
				IISWebServer server = this[i];
				if (server.SiteName.ToLower().Trim() == sitName)
					return true;
			}
			return false;
		}

				
		#endregion

		
	}
}
