using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;

namespace TeamSuneat
{
    public class GameAddressableAssetManager
    {
        private static readonly Dictionary<string, object> _resourcesCache = new();
        private static readonly Dictionary<string, AsyncOperationHandle> _asyncOperationHandles = new();

        #region 리소스 불러오기

        public T LoadResource<T>(string assetGuid) where T : UnityEngine.Object
        {
            if (string.IsNullOrEmpty(assetGuid))
            {
                Log.Warning("리소스 키가 비어 있습니다.");
                return null;
            }

            // 이미 캐시된 리소스가 있는지 확인
            if (_resourcesCache.TryGetValue(assetGuid, out object cachedResource))
            {
                return cachedResource as T;
            }

            return null;
        }

        public async Task<T> LoadResourceAsync<T>(string assetGuidOrKey) where T : UnityEngine.Object
        {
            if (string.IsNullOrEmpty(assetGuidOrKey))
            {
                Log.Warning("리소스 키가 비어 있습니다.");
                return null;
            }

            AsyncOperationHandle<IList<IResourceLocation>> locationsHandle = Addressables.LoadResourceLocationsAsync(assetGuidOrKey, typeof(T));

            try
            {
                IList<IResourceLocation> locations = await locationsHandle.Task;
                if (locations == null || locations.Count == 0)
                {
                    Log.Warning(LogTags.Resource, "Addressable 리소스 위치를 찾을 수 없습니다: {0}", assetGuidOrKey);
                    return null;
                }

                IResourceLocation location = locations[0];
                string cacheKey = location.PrimaryKey;

                if (_resourcesCache.TryGetValue(cacheKey, out object cachedResource))
                {
                    return cachedResource as T;
                }

                AsyncOperationHandle<T> asyncOperation = Addressables.LoadAssetAsync<T>(location);
                _asyncOperationHandles[cacheKey] = asyncOperation;

                T resource = await asyncOperation.Task;
                if (resource != null)
                {
                    Log.Info(LogTags.Resource, "AssetGUID를 키로 리소스를 캐시합니다: {0}", cacheKey);
                    _resourcesCache[cacheKey] = resource;
                    return resource;
                }

                Log.Error("Addressable 리소스 불러오기 실패: {0}", cacheKey);
                return null;
            }
            catch (System.Exception ex)
            {
                Log.Error("Addressable 리소스 불러오기 중 오류 발생: {0}, 오류: {1}", assetGuidOrKey, ex.Message);
                return null;
            }
            finally
            {
                Addressables.Release(locationsHandle);
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
                    await CacheResourcesByLocations(label, assets);
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

        public bool IsResourceLoaded(AssetReference assetReference)
        {
            if (assetReference == null)
            {
                return false;
            }

            return IsResourceLoaded(assetReference.AssetGUID);
        }

        private async Task CacheResourcesByLocations<T>(string label, IList<T> assets) where T : UnityEngine.Object
        {
            AsyncOperationHandle<IList<IResourceLocation>> locationsHandle = Addressables.LoadResourceLocationsAsync(label, typeof(T));

            try
            {
                IList<IResourceLocation> locations = await locationsHandle.Task;
                if (locations == null || locations.Count == 0)
                {
                    Log.Warning(LogTags.Resource, "{0} 라벨로 리소스 위치를 찾을 수 없습니다.", label);
                    return;
                }

                int count = Mathf.Min(assets.Count, locations.Count);
                for (int i = 0; i < count; i++)
                {
                    string cacheKey = locations[i].PrimaryKey;
                    T asset = assets[i];

                    if (_resourcesCache.ContainsKey(cacheKey))
                    {
                        continue;
                    }

                    _resourcesCache.Add(cacheKey, asset);
                    Log.Progress(LogTags.Resource, "{0} 라벨로 불러온 리소스를 AssetGUID로 캐시합니다. Key: {1}, Asset: {2}", label, cacheKey, asset.name);
                }
            }
            catch (System.Exception ex)
            {
                Log.Error(LogTags.Resource, "{0} 라벨 리소스 캐싱 중 오류 발생: {1}", label, ex.Message);
            }
            finally
            {
                Addressables.Release(locationsHandle);
            }
        }

        #endregion 유틸리티
    }
}