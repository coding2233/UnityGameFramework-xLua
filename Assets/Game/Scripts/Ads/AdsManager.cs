using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Advertisements;

namespace Wanderer.GameFramework
{
    public class AdsManager : GameFrameworkModule,IUnityAdsListener
    {
        private string _appleId = "3982834";
        private string _androidId = "3982835";

        HashSet<string> _readyIds = new HashSet<string>();

        private Dictionary<string, List<Action>> _onAdsStart = new Dictionary<string, List<Action>>();
        private Dictionary<string, List<Action<ShowResult>>> _onAdsFinish = new Dictionary<string, List<Action<ShowResult>>>();

        private bool _testMode = false;

        public AdsManager()
        {
        }

        public override void OnInit()
        {
            base.OnInit();

            Advertisement.AddListener(this);

#if UNITY_EDITOR || TEST
            _testMode = true;
#endif

#if UNITY_ANDROID
            Advertisement.Initialize(_androidId, _testMode,true);
#elif UNITY_IOS
            Advertisement.Initialize(_appleId, _testMode,true);
#endif
        }


        public override void OnClose()
        {
            Advertisement.RemoveListener(this);
        }

        public async void Show(string id, Action onStart=null, Action<ShowResult> OnFinish=null,bool isBanner=false,BannerPosition bannerPosition= BannerPosition.BOTTOM_CENTER)
        {
            if (_testMode)
                return;

            await PlayAds(id, onStart, OnFinish, isBanner, bannerPosition);
        }

        IEnumerator PlayAds(string id, Action onStart,Action<ShowResult> onFinish, bool isBanner , BannerPosition bannerPosition)
        {
            while (!Advertisement.isInitialized)
            {
                yield return null;
            }

           

            if (isBanner)
            {
                Advertisement.Banner.Load(id);
                while(!Advertisement.IsReady(id))
                {
                    yield return null;
                }

                while (!_readyIds.Contains(id))
                {
                    yield return null;
                }

                if (onStart != null)
                {
                    _onAdsStart[id].Add(onStart);
                }
                if (onFinish != null)
                {
                    _onAdsFinish[id].Add(onFinish);
                }

                Advertisement.Banner.SetPosition(bannerPosition);
                Advertisement.Banner.Show(id);
            }
            else
            {
                Advertisement.Load(id);
                while (!Advertisement.IsReady(id))
                {
                    yield return null;
                }

                while (!_readyIds.Contains(id))
                {
                    yield return null;
                }

                if (onStart != null)
                {
                    _onAdsStart[id].Add(onStart);
                }
                if (onFinish != null)
                {
                    _onAdsFinish[id].Add(onFinish);
                }
                Advertisement.Show(id);
            }
        }

        //public void ShowGameStart()
        //{
        //    Show("GameStart");
        //}

        //public void ShowLoading()
        //{
        //    Show("Loading");
        //}

        //public void ShowGameSelect()
        //{
        //    Show("GameSelect");
        //}

        public void OnUnityAdsDidError(string message)
        {
            Debug.LogWarning($"ads OnUnityAdsDidError: {message}");
        }

        public void OnUnityAdsDidFinish(string placementId, ShowResult showResult)
        {
            Debug.Log($"ads OnUnityAdsDidFinish: {placementId}");

            if (!_onAdsFinish.ContainsKey(placementId))
                return;

            var onFinishCallback = _onAdsFinish[placementId];


            if (onFinishCallback.Count > 0)
            {
                for (int i = 0; i < onFinishCallback.Count; i++)
                {
                    onFinishCallback[i].Invoke(showResult);
                }
                onFinishCallback.Clear();
            }
        }

        public void OnUnityAdsDidStart(string placementId)
        {
            Debug.Log($"ads OnUnityAdsDidStart: {placementId}");

            if (!_onAdsStart.ContainsKey(placementId))
                return;

            var onStartCallback = _onAdsStart[placementId];


            if (onStartCallback.Count > 0)
            {
                for (int i = 0; i < onStartCallback.Count; i++)
                {
                    onStartCallback[i].Invoke();
                }
                onStartCallback.Clear();
            }
        }

        public void OnUnityAdsReady(string id)
        {
            if (!_onAdsStart.ContainsKey(id))
            {
                _onAdsStart.Add(id, new List<Action>());
            }
            if (!_onAdsFinish.ContainsKey(id))
            {
                _onAdsFinish.Add(id, new List<Action<ShowResult>>());
            }
            _readyIds.Add(id);

            Debug.Log($"ads OnUnityAdsReady: {id}");
        }
    }
}
