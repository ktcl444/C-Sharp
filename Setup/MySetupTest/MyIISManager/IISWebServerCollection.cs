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
		#region  "公共的实例属性"
		/// <summary>
		/// 通过序号进行索引
		/// </summary>
		public IISWebServer this[int Index]
		{
			get
			{
				return (IISWebServer)this.List[Index];

			}
		}

		
		/// <summary>
		/// 通过网站名进行索引
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


		#region "公共的实例方法"
		/// <summary>
		/// 添加集合元素
		/// </summary>
		public void Add(IISWebServer webServer)
		{
			this.List.Add(webServer);
		}


		/// <summary>
		/// 批量增加集合元素
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
		/// 删除集合元素
		/// </summary>
		public void Remove(IISWebServer webServer)
		{
			this.List.Remove(webServer);					
		}


		/// <summary>
		/// 是否包含指定名称的网站
		/// </summary>
		public bool Contains(IISWebServer webServer)
		{
			return this.List.Contains(webServer);
		}

		/// <summary>
		/// 是否包含指定名称的网站
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
