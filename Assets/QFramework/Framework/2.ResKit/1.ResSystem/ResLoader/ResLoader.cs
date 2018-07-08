﻿/****************************************************************************
 * Copyright (c) 2017 snowcold
 * Copyright (c) 2017 ~ 2018.6 liangxie
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
    using System.Collections.Generic;
    using UnityEngine;
    using Object = UnityEngine.Object;

    public class ResLoader : DisposableObject, IResLoader, IPoolable, IPoolType
    {
        /// <summary>
        /// ID:RKRL001 申请ResLoader对象 ResLoader.Allocate（IResLoaderStrategy strategy = null)
        /// </summary>
        /// <param name="strategy">加载策略</param>
        /// <returns></returns>
        public static ResLoader Allocate(IResLoaderStrategy strategy = null)
        {
            var loader = SafeObjectPool<ResLoader>.Instance.Allocate();
            loader.SetStrategy(strategy);
            return loader;
        }

        /// <summary>
        /// ID:RKRL002 释放ResLoader对象 ResLoader.Recycle2Cache
        /// </summary>
        public void Recycle2Cache()
        {
            SafeObjectPool<ResLoader>.Instance.Recycle(this);
        }

        /// <summary>
        /// ID:RKRL003 同步加载AssetBundle里的资源 ResLoader.LoadSync<T>(string ownerBundle,string assetBundle)
        /// </summary>
        /// <param name="ownerBundle">assetBundle名字</param>
        /// <param name="assetName">资源名字</param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T LoadSync<T>(string ownerBundle, string assetName) where T : Object
        {
            ownerBundle = ownerBundle.ToLower();
            assetName = assetName.ToLower();
            Add2Load(ownerBundle, assetName);
            LoadSync();

            var res = ResMgr.Instance.GetRes(ownerBundle, assetName);
            if (res == null)
            {
                Log.E("Failed to Load Res:" + ownerBundle + assetName);
                return null;
            }

            return res.Asset as T;
        }

        /// <summary>
        /// ID:RKRL003 只通过资源名字进行同步加载 ResLoader.LoadSync<T>(string assetName)
        /// </summary>
        /// <param name="assetName">资源名字</param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T LoadSync<T>(string assetName) where T : Object
        {
            return LoadSync(assetName) as T;
        }

        /// <summary>
        /// ID:RKRL003 只通过资源名字进行同步加载,
        /// </summary>
        /// <param name="name">资源名字</param>
        /// <returns></returns>
        public Object LoadSync(string name)
        {
            Add2Load(name);
            LoadSync();

            var res = ResMgr.Instance.GetRes(name, false);
            if (res == null)
            {
                Log.E("Failed to Load Res:" + name);
                return null;
            }

            return res.Asset;
        }

        private void LoadSync()
        {
            while (mWaitLoadList.Count > 0)
            {
                var first = mWaitLoadList.First.Value;
                --mLoadingCount;
                mWaitLoadList.RemoveFirst();

                if (first == null)
                {
                    return;
                }

                if (first.LoadSync())
                {
                    first.AcceptLoaderStrategySync(this, mStrategy);
                }
            }

            mStrategy.OnAllTaskFinish(this);
        }


        class CallBackWrap
        {
            private readonly Action<bool, IRes> mListener;
            private readonly IRes               mRes;

            public CallBackWrap(IRes r, Action<bool, IRes> l)
            {
                mRes = r;
                mListener = l;
            }

            public void Release()
            {
                mRes.UnRegisteResListener(mListener);
            }

            public bool IsRes(IRes res)
            {
                return res.AssetName == mRes.AssetName;
            }
        }

        private readonly List<IRes>         mResList      = new List<IRes>();
        private readonly LinkedList<IRes>   mWaitLoadList = new LinkedList<IRes>();
        private          Action             mListener;
        private          IResLoaderStrategy mStrategy;

        private int  mLoadingCount;

        private        LinkedList<CallBackWrap> mCallbackRecardList;
        private static DefaultLoaderStrategy    sDefaultStrategy;

        public static IResLoaderStrategy defaultStrategy
        {
            get
            {
                if (sDefaultStrategy == null)
                {
                    sDefaultStrategy = new DefaultLoaderStrategy();
                }

                return sDefaultStrategy;
            }
        }

        public float Progress
        {
            get
            {
                if (mWaitLoadList.Count == 0)
                {
                    return 1;
                }

                var unit = 1.0f / mResList.Count;
                var currentValue = unit * (mResList.Count - mLoadingCount);

                var currentNode = mWaitLoadList.First;

                while (currentNode != null)
                {
                    currentValue += unit * currentNode.Value.Progress;
                    currentNode = currentNode.Next;
                }

                return currentValue;
            }
        }

        public bool IsRecycled { get; set; }


        public ResLoader()
        {
            SetStrategy(sDefaultStrategy);
        }

        public void Add2Load(List<string> list)
        {
            if (list == null)
            {
                return;
            }

            for (var i = list.Count - 1; i >= 0; --i)
            {
                Add2Load(list[i]);
            }
        }

        public void Add2Load(string ownerBundle, string assetName, Action<bool, IRes> listener = null,
            bool lastOrder = true)
        {
            if (string.IsNullOrEmpty(ownerBundle) || string.IsNullOrEmpty(assetName))
            {
                Log.E("Res Name Or Bundle Name Is Null.");
                return;
            }

            var res = FindResInArray(mResList, ownerBundle, assetName);
            if (res != null)
            {
                if (listener != null)
                {
                    AddResListenerReward(res, listener);
                    res.RegisteResListener(listener);
                }

                return;
            }

            res = ResMgr.Instance.GetRes(ownerBundle, assetName, true);

            if (res == null)
            {
                return;
            }

            if (listener != null)
            {
                AddResListenerReward(res, listener);
                res.RegisteResListener(listener);
            }

            //无论该资源是否加载完成，都需要添加对该资源依赖的引用
            var depends = res.GetDependResList();

            if (depends != null)
            {
                foreach (var depend in depends)
                {
                    Add2Load(depend);
                }
            }

            AddRes2Array(res, lastOrder);
        }


        public void Add2Load(string resName, Action<bool, IRes> listener = null, bool lastOrder = true)
        {
            if (string.IsNullOrEmpty(resName))
            {
                Log.E("Res Name Is Null.");
                return;
            }

            var res = FindResInArray(mResList, resName);
            if (res != null)
            {
                if (listener != null)
                {
                    AddResListenerReward(res, listener);
                    res.RegisteResListener(listener);
                }

                return;
            }

            res = ResMgr.Instance.GetRes(resName, true);

            if (res == null)
            {
                return;
            }

            if (listener != null)
            {
                AddResListenerReward(res, listener);
                res.RegisteResListener(listener);
            }

            //无论该资源是否加载完成，都需要添加对该资源依赖的引用
            var depends = res.GetDependResList();

            if (depends != null)
            {
                foreach (var depend in depends)
                {
                    Add2Load(depend);
                }
            }

            AddRes2Array(res, lastOrder);
        }


#if UNITY_EDITOR
        private readonly Dictionary<string, Sprite> mCachedSpriteDict = new Dictionary<string, Sprite>();
#endif

        public Sprite LoadSprite(string bundleName, string spriteName)
        {
#if UNITY_EDITOR
            if (AbstractRes.SimulateAssetBundleInEditor)
            {
                if (mCachedSpriteDict.ContainsKey(spriteName))
                {
                    return mCachedSpriteDict[spriteName];
                }

                var texture = LoadSync<Texture2D>(bundleName, spriteName);
                var sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height),
                    Vector2.one * 0.5f);
                mCachedSpriteDict.Add(spriteName, sprite);
                return mCachedSpriteDict[spriteName];
            }
#endif

            return LoadSync<Sprite>(bundleName, spriteName);
        }


        public Sprite LoadSprite(string spriteName)
        {
#if UNITY_EDITOR
            if (AbstractRes.SimulateAssetBundleInEditor)
            {
                if (mCachedSpriteDict.ContainsKey(spriteName))
                {
                    return mCachedSpriteDict[spriteName];
                }

                var texture = LoadSync(spriteName) as Texture2D;
                var sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height),
                    Vector2.one * 0.5f);
                mCachedSpriteDict.Add(spriteName, sprite);
                return mCachedSpriteDict[spriteName];
            }
#endif

            return LoadSync<Sprite>(spriteName);
        }


        public void LoadAsync(Action listener = null)
        {
            mListener = listener;
            DoLoadAsync();
        }

        public void ReleaseRes(string resName)
        {
            if (string.IsNullOrEmpty(resName))
            {
                return;
            }

#if UNITY_EDITOR
            if (AbstractRes.SimulateAssetBundleInEditor)
            {
                if (mCachedSpriteDict.ContainsKey(resName))
                {
                    var sprite = mCachedSpriteDict[resName];
                    GameObject.Destroy(sprite);
                    mCachedSpriteDict.Remove(resName);
                }
            }
#endif

            var res = ResMgr.Instance.GetRes(resName);
            if (res == null)
            {
                return;
            }

            if (mWaitLoadList.Remove(res))
            {
                --mLoadingCount;
                if (mLoadingCount == 0)
                {
                    mListener = null;
                }
            }

            if (mResList.Remove(res))
            {
                res.UnRegisteResListener(OnResLoadFinish);
                res.Release();
                ResMgr.Instance.ClearOnUpdate();
            }
        }

        public void ReleaseRes(string[] names)
        {
            if (names == null || names.Length == 0)
            {
                return;
            }

            for (var i = names.Length - 1; i >= 0; --i)
            {
                ReleaseRes(names[i]);
            }
        }

        public void ReleaseAllRes()
        {
#if UNITY_EDITOR
            if (AbstractRes.SimulateAssetBundleInEditor)
            {
                foreach (var spritePair in mCachedSpriteDict)
                {
                    GameObject.Destroy(spritePair.Value);
                }

                mCachedSpriteDict.Clear();
            }
#endif

            mListener = null;
            mLoadingCount = 0;
            mWaitLoadList.Clear();

            if (mResList.Count > 0)
            {
                //确保首先删除的是AB，这样能对Asset的卸载做优化
                mResList.Reverse();

                for (var i = mResList.Count - 1; i >= 0; --i)
                {
                    mResList[i].UnRegisteResListener(OnResLoadFinish);
                    mResList[i].Release();
                }

                mResList.Clear();
                ResMgr.Instance.ClearOnUpdate();
            }

            RemoveAllCallbacks(true);
        }

        public void UnloadAllInstantialteRes(bool flag)
        {
            if (mResList.Count > 0)
            {
                for (var i = mResList.Count - 1; i >= 0; --i)
                {
                    if (mResList[i].UnloadImage(flag))
                    {
                        if (mWaitLoadList.Remove(mResList[i]))
                        {
                            --mLoadingCount;
                        }

                        RemoveCallback(mResList[i], true);

                        mResList[i].UnRegisteResListener(OnResLoadFinish);
                        mResList[i].Release();
                        mResList.RemoveAt(i);
                    }
                }

                ResMgr.Instance.ClearOnUpdate();
            }
        }

        public override void Dispose()
        {
            ReleaseAllRes();
            base.Dispose();
        }

        public void Dump()
        {
            foreach (var res in mResList)
            {
                Log.I(res.AssetName);
            }
        }

        private void SetStrategy(IResLoaderStrategy strategy)
        {
            mStrategy = strategy ?? defaultStrategy;
        }

        private void DoLoadAsync()
        {
            if (mLoadingCount == 0)
            {
                if (mListener != null)
                {
                    var callback = mListener;
                    mListener = null;
                    callback();
                }

                return;
            }

            var nextNode = mWaitLoadList.First;
            LinkedListNode<IRes> currentNode = null;
            while (nextNode != null)
            {
                currentNode = nextNode;
                var res = currentNode.Value;
                nextNode = currentNode.Next;
                if (res.IsDependResLoadFinish())
                {
                    mWaitLoadList.Remove(currentNode);

                    if (res.State != ResState.Ready)
                    {
                        res.RegisteResListener(OnResLoadFinish);
                        res.LoadAsync();
                    }
                    else
                    {
                        --mLoadingCount;
                    }
                }
            }
        }

        private void RemoveCallback(IRes res, bool release)
        {
            if (mCallbackRecardList != null)
            {
                var current = mCallbackRecardList.First;
                LinkedListNode<CallBackWrap> next = null;
                while (current != null)
                {
                    next = current.Next;
                    if (current.Value.IsRes(res))
                    {
                        if (release)
                        {
                            current.Value.Release();
                        }

                        mCallbackRecardList.Remove(current);
                    }

                    current = next;
                }
            }
        }

        private void RemoveAllCallbacks(bool release)
        {
            if (mCallbackRecardList != null)
            {
                var count = mCallbackRecardList.Count;
                while (count > 0)
                {
                    --count;
                    if (release)
                    {
                        mCallbackRecardList.Last.Value.Release();
                    }

                    mCallbackRecardList.RemoveLast();
                }
            }
        }

        private void OnResLoadFinish(bool result, IRes res)
        {
            --mLoadingCount;

            res.AcceptLoaderStrategyAsync(this, mStrategy);
            DoLoadAsync();
            if (mLoadingCount == 0)
            {
                RemoveAllCallbacks(false);

                //ResMgr.Instance.timeDebugger.End();
                //ResMgr.Instance.timeDebugger.Dump(-1);
                if (mListener != null)
                {
                    mListener();
                    mListener = null;
                }

                mStrategy.OnAllTaskFinish(this);
            }
        }

        private void AddRes2Array(IRes res, bool lastOrder)
        {
            //再次确保队列中没有它
            var oldRes = FindResInArray(mResList, res.AssetName);

            if (oldRes != null)
            {
                return;
            }

            res.Retain();
            mResList.Add(res);

            if (res.State != ResState.Ready)
            {
                ++mLoadingCount;
                if (lastOrder)
                {
                    mWaitLoadList.AddLast(res);
                }
                else
                {
                    mWaitLoadList.AddFirst(res);
                }
            }
        }

        private static IRes FindResInArray(List<IRes> list, string ownerBundleName, string assetName)
        {
            if (list == null)
            {
                return null;
            }

            for (var i = list.Count - 1; i >= 0; --i)
            {
                if (list[i].AssetName.Equals(assetName) && list[i].OwnerBundleName == ownerBundleName)
                {
                    return list[i];
                }
            }

            return null;
        }


        private static IRes FindResInArray(List<IRes> list, string name)
        {
            if (list == null)
            {
                return null;
            }

            for (var i = list.Count - 1; i >= 0; --i)
            {
                if (list[i].AssetName.Equals(name))
                {
                    return list[i];
                }
            }

            return null;
        }

        private void AddResListenerReward(IRes res, Action<bool, IRes> listener)
        {
            if (mCallbackRecardList == null)
            {
                mCallbackRecardList = new LinkedList<CallBackWrap>();
            }

            mCallbackRecardList.AddLast(new CallBackWrap(res, listener));
        }

        void IPoolable.OnRecycled()
        {
            ReleaseAllRes();
        }
    }
}