using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace TeamSuneat
{
    public class GameAddressableAssetManager
    {
        private static readonly Dictionary<string, object> _resourcesCache = new();
        private static readonly Dictionary<string, AsyncOperationHandle> _asyncOperationHandles = new();

        #region 리소스 불러오기

        public T LoadResource<T>(string key) where T : UnityEngine.Object
        {
            if (string.IsNullOrEmpty(key))
            {
                Log.Warning("리소스 키가 비어 있습니다.");
                return null;
            }

            // 이미 캐시된 리소스가 있는지 확인
            if (_resourcesCache.TryGetValue(key, out object cachedResource))
            {
                return cachedResource as T;
            }

            return null;
        }

        public async Task<T> LoadResourceAsync<T>(string path) where T : UnityEngine.Object
        {
            if (string.IsNullOrEmpty(path))
            {
                Log.Warning("리소스 키가 비어 있습니다.");
                return null;
            }

            // 이미 캐시된 리소스가 있는지 확인
            if (_resourcesCache.TryGetValue(path, out object cachedResource))
            {
                return cachedResource as T;
            }

            try
            {
                // 비동기 불러오기 시작
                AsyncOperationHandle<T> asyncOperation = Addressables.LoadAssetAsync<T>(path);
                _asyncOperationHandles[path] = asyncOperation;

                T resource = await asyncOperation.Task;
                if (resource != null)
                {
                    Log.Info(LogTags.Resource, "비동기로 불러온 AssetReference 리소스를 캐시합니다: {0}", path);
                    _resourcesCache[path] = resource;
                    return resource;
                }
                else
                {
                    Log.Error("Addressable 리소스 불러오기 실패: {0}", path);
                    return null;
                }
            }
            catch (System.Exception ex)
            {
                Log.Error("Addressable 리소스 불러오기 중 오류 발생: {0}, 오류: {1}", path, ex.Message);
                return null;
            }
        }

        public async Task<T> LoadResourceAsync<T>(AssetReference assetReference) where T : UnityEngine.Object
        {
            if (assetReference == null)
            {
                Log.Warning(LogTags.Resource, "AssetReference가 null입니다.");
                return null;
            }

            string key = assetReference.AssetGUID;

            // 이미 캐시된 리소스가 있는지 확인
            if (_resourcesCache.TryGetValue(key, out object cachedResource))
            {
                Log.Info(LogTags.Resource, "캐시된 리소스를 사용합니다: {0}", key);
                return cachedResource as T;
            }

            try
            {
                // 비동기 불러오기 시작
                AsyncOperationHandle<T> asyncOperation = assetReference.LoadAssetAsync<T>();
                _asyncOperationHandles[key] = asyncOperation;

                T resource = await asyncOperation.Task;

                if (resource != null)
                {
                    _resourcesCache[key] = resource;
                    Log.Info(LogTags.Resource, "AssetReference 리소스를 비동기로 불러오기했습니다: {0}", key);
                    return resource;
                }
                else
                {
                    Log.Error(LogTags.Resource, "AssetReference 리소스 불러오기 실패: {0}", key);
                    return null;
                }
            }
            catch (System.Exception ex)
            {
                Log.Error(LogTags.Resource, "AssetReference 리소스 불러오기 중 오류 발생: {0}, 오류: {1}", key, ex.Message);
                return null;
            }
        }

        public async Task<IList<T>> LoadResourcesByLabelAsync<T>(string label) where T : UnityEngine.Object
        {
            try
            {
                AsyncOperationHandle<IList<T>> handle = Addressables.LoadAssetsAsync<T>(label, null);
                _asyncOperationHandles[label] = handle;

                IList<T> assets = await handle.Task;
                if (assets != null && assets.Count > 0)
                {
                    Log.Info(LogTags.Resource, "{0} 라벨로 {1} 타입의 리소스를 {2}개 불러왔습니다.", label, typeof(T), assets.Count);
                    for (int i = 0; i < assets.Count; i++)
                    {
                        T asset = assets[i];
                        string path = PathManager.FindPathByType<T>(asset.name);
                        if (!string.IsNullOrEmpty(path))
                        {
                            _resourcesCache.Add(path, asset);
                            Log.Progress(LogTags.Resource, "불러온 {0} 리소스를 캐시합니다. Path: {1}", asset.name, path);
                        }
                    }
                    return assets;
                }
                else
                {
                    Log.Warning(LogTags.Resource, "{0} 라벨로 리소스를 찾을 수 없습니다.", label);
                    return new List<T>();
                }
            }
            catch (System.Exception ex)
            {
                Log.Error(LogTags.Resource, "{0} 라벨 리소스 불러오기 중 오류 발생: {1}", label, ex.Message);
                return new List<T>();
            }
        }

        #endregion 리소스 불러오기

        #region 리소스 해제

        public void ReleaseResource<T>(T resource) where T : UnityEngine.Object
        {
            if (resource != null)
            {
                Addressables.Release(resource);
                Log.Info(LogTags.Resource, "리소스를 해제했습니다: {0}", resource.name);
            }
        }

        public void ReleaseResource(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                return;
            }

            if (_asyncOperationHandles.TryGetValue(key, out AsyncOperationHandle handle))
            {
                Addressables.Release(handle);
                _ = _asyncOperationHandles.Remove(key);
                Log.Info(LogTags.Resource, "리소스를 해제했습니다: {0}", key);
            }

            if (_resourcesCache.ContainsKey(key))
            {
                _ = _resourcesCache.Remove(key);
            }
        }

        public void ReleaseAllResources()
        {
            foreach (AsyncOperationHandle handle in _asyncOperationHandles.Values)
            {
                Addressables.Release(handle);
            }

            _asyncOperationHandles.Clear();
            _resourcesCache.Clear();

            Log.Info(LogTags.Resource, "모든 Addressable 리소스를 해제했습니다.");
        }

        #endregion 리소스 해제

        #region 유틸리티

        public bool IsResourceLoaded(string key)
        {
            return _resourcesCache.ContainsKey(key);
        }

        #endregion 유틸리티
    }
}