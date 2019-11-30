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
		
		#region "����������"
		/// <summary>
		/// ͨ����Ž�������
		/// </summary>
		public IISWebVirtualDir this[int Index]
		{
			get
			{
				return (IISWebVirtualDir)this.List[Index];
			}
		}

		
		/// <summary>
		/// ͨ������Ŀ¼����������
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

		#region "������ʵ������"
		/// <summary>
		/// ��Ӽ���Ԫ��
		/// </summary>
		public void Add(IISWebVirtualDir webVirtualDir)
		{
			this.List.Add(webVirtualDir);
		}
		

		/// <summary>
		/// �������Ӽ���Ԫ��
		/// </summary>
		public void AddRange(IISWebVirtualDir[] webVirtualDirs)
		{
			for (int i = 0; i < webVirtualDirs.Length; i++)
			{
				this.List.Add(webVirtualDirs[i]);
			}
		}


		/// <summary>
		/// ɾ������Ԫ��
		/// </summary>
		public void Remove(IISWebVirtualDir webVirtualDir)
		{
			this.List.Remove(webVirtualDir);
		}


		/// <summary>
		/// �Ƿ����ָ�����Ƶ�����Ŀ¼
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
