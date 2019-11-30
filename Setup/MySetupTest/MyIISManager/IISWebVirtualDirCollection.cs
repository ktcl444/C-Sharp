using System;
using System.Collections;
using System.Text;

namespace MyIISManager
{
	/// <summary>
	/// IISWebVirtualDirCollection
	/// </summary>
	public class IISWebVirtualDirCollection : CollectionBase
	{
		
		#region "公共的属性"
		/// <summary>
		/// 通过序号进行索引
		/// </summary>
		public IISWebVirtualDir this[int Index]
		{
			get
			{
				return (IISWebVirtualDir)this.List[Index];
			}
		}

		
		/// <summary>
		/// 通过虚拟目录名进行索引
		/// </summary>
		public IISWebVirtualDir this[string name]
		{
			get
			{
				name = name.ToLower();
				IISWebVirtualDir list;
				for (int i = 0; i < this.List.Count; i++)
				{
					list = (IISWebVirtualDir)this.List[i];
					if (list.Name.ToLower() == name)
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
		public void Add(IISWebVirtualDir webVirtualDir)
		{
			this.List.Add(webVirtualDir);
		}
		

		/// <summary>
		/// 批量增加集合元素
		/// </summary>
		public void AddRange(IISWebVirtualDir[] webVirtualDirs)
		{
			for (int i = 0; i < webVirtualDirs.Length; i++)
			{
				this.List.Add(webVirtualDirs[i]);
			}
		}


		/// <summary>
		/// 删除集合元素
		/// </summary>
		public void Remove(IISWebVirtualDir webVirtualDir)
		{
			this.List.Remove(webVirtualDir);
		}


		/// <summary>
		/// 是否包含指定名称的虚拟目录
		/// </summary>
		/// <param name="ServerComment"></param>
		/// <returns></returns>
		public bool Contains(string name)
		{
			name = name.ToLower().Trim();
			for (int i = 0; i < this.List.Count; i++)
			{
				IISWebVirtualDir virDir = this[i];
				if (virDir.Name.ToLower().Trim() == name)
					return true;
			}
			return false;
		}
		#endregion
		
	}
}
