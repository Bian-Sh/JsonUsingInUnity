/****************************************************************************
 * Copyright (c) 2017 xiaojun
 * Copyright (c) 2017 liangxie
 * Copyright (c) 2017 maoling
 * Copyright (c) 2018.5 ~ 2018.6 liangxie
 * 
 * http://qframework.io
 * https://github.com/liangxiegame/QFramework
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
 ****************************************************************************/

namespace QFramework
{
	using System;
	using UnityEngine;

	/// <summary>
	/// 每个UIbehaviour对应的Data
	/// </summary>
	public interface IUIData
	{
	}

	public class UIPanelData : IUIData
	{
	}

	[Obsolete("弃用啦")]
	public class UIPageData : UIPanelData
	{
	}

	[Obsolete("弃用啦")]
	public abstract class QUIBehaviour : UIPanel
	{
	}

	public abstract class UIPanel : QMonoBehaviour, IUIBehaviour
	{
		public Transform Transform
		{
			get { return transform; }
		}

		private int            mUILayerType    = -10000;
		private int            mLayerSortIndex = -10;
		private IUIPanelLoader mUiPanelLoader  = null;

		public int UILayerType
		{
			get { return mUILayerType; }
			set { mUILayerType = value; }
		}

		public int LayerSortIndex
		{
			get { return mLayerSortIndex; }
			set { mLayerSortIndex = value; }
		}

		public static UIPanel Load(string panelName, string assetBundleName = null)
		{
			var panelLoader = new DefaultUIPanelLoader();
			var panelPrefab = assetBundleName.IsNullOrEmpty()
				? panelLoader.LoadPanelPrefab(panelName)
				: panelLoader.LoadPanelPrefab(assetBundleName, panelName);
			var obj = Instantiate(panelPrefab);
			var retScript = obj.GetComponent<UIPanel>();
			retScript.mUiPanelLoader = panelLoader;
			return retScript;
		}

		protected override void SetupMgr()
		{
			mCurMgr = QUIManager.Instance;
		}

		protected override void OnBeforeDestroy()
		{
			ClearUIComponents();
		}

		protected virtual void ClearUIComponents()
		{
		}


		public void SetSiblingIndexAndNewLayerIndex(int siblingIndex, int layerIndex)
		{
			if (mLayerSortIndex != layerIndex)
			{
				Transform.SetSiblingIndex(siblingIndex);
				mLayerSortIndex = layerIndex;
			}
		}


		public void Init(IUIData uiData = null)
		{
			InnerInit(uiData);
			RegisterUIEvent();
		}

		void InnerInit(IUIData uiData = null)
		{
			InitUI(uiData);
		}

		protected virtual void InitUI(IUIData uiData = null)
		{
		}

		protected virtual void RegisterUIEvent()
		{
		}


		/// <summary>
		/// avoid override in child class
		/// </summary>
		protected sealed override void OnDestroy()
		{
			base.OnDestroy();
		}

		/// <summary>
		/// 关闭,不允许子类调用
		/// </summary>
		void IUIBehaviour.Close(bool destroyed)
		{
			OnClose();
			if (destroyed)
			{
				Destroy(gameObject);
			}

			mOnPanelClosed.InvokeGracefully();
			mOnPanelClosed = null;
			mUiPanelLoader.Unload();
			mUiPanelLoader = null;
		}

		protected void CloseSelf()
		{
			QUIManager.Instance.CloseUI(name);
		}

		/// <summary>
		/// 关闭
		/// </summary>
		protected virtual void OnClose()
		{
		}

		private Action mOnPanelClosed;

		public void OnClosed(Action onPanelClosed)
		{
			mOnPanelClosed = onPanelClosed;
		}
	}
}