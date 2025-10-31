using Lean.Pool;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.U2D;

namespace TeamSuneat
{
    public partial class ResourcesManager
    {
        #region Fields

        private static readonly GameAddressableAssetManager _addressableManager = new();

        #endregion Fields

        #region Initialization

        /// <summary>
        /// Addressable 시스템을 초기화합니다.
        /// </summary>
        public static void InitializeAddressables()
        {
            Log.Warning("비동기 모드에서는 InitializeAddressablesAsync()를 사용하세요.");
        }

        #endregion Initialization

        #region Resource Management

        /// <summary>
        /// 리소스가 로드되었는지 확인합니다.
        /// </summary>
        public static bool IsResourceLoaded<T>(string path) where T : UnityEngine.Object
        {
            return _addressableManager.IsResourceLoaded(path);
        }

        /// <summary>
        /// 리소스를 해제합니다.
        /// </summary>
        public static void ReleaseResource<T>(T resource) where T : UnityEngine.Object
        {
            if (resource == null)
            {
                return;
            }

            _addressableManager.ReleaseResource(resource);
        }

        /// <summary>
        /// Addressable 리소스를 로드합니다. (내부용)
        /// </summary>
        internal static T LoadAddressableResource<T>(string targetAddressablePath)
        {
            return default;
        }

        #endregion Resource Management

        #region Object Instantiation

        /// <summary>
        /// 게임오브젝트를 인스턴스화합니다.
        /// </summary>
        public static GameObject Instantiate(GameObject prefab, Transform parent = null, bool usePool = true)
        {
            return InstantiateInternal(prefab, null, parent, usePool);
        }

        /// <summary>
        /// 게임오브젝트를 지정된 위치에 인스턴스화합니다.
        /// </summary>
        public static GameObject Instantiate(GameObject prefab, Vector3 position, bool usePool = true)
        {
            return InstantiateInternal(prefab, position, null, usePool);
        }

        /// <summary>
        /// 게임오브젝트를 인스턴스화하고 지정된 컴포넌트를 반환합니다.
        /// </summary>
        public static T Instantiate<T>(GameObject prefab, Transform parent = null, bool usePool = true)
        {
            return InstantiateInternal<T>(prefab, null, parent, usePool);
        }

        /// <summary>
        /// 게임오브젝트를 지정된 위치에 인스턴스화하고 지정된 컴포넌트를 반환합니다.
        /// </summary>
        public static T Instantiate<T>(GameObject prefab, Vector3 position, bool usePool = true)
        {
            return InstantiateInternal<T>(prefab, position, null, usePool);
        }

        #endregion Object Instantiation

        #region Prefab Spawning

        /// <summary>
        /// 프리팹을 지정된 위치에 스폰합니다.
        /// </summary>
        public static GameObject SpawnPrefab(string prefabName, Vector3 position)
        {
            return SpawnPrefabInternal(prefabName, position);
        }

        /// <summary>
        /// 프리팹을 지정된 부모 하위에 스폰합니다.
        /// </summary>
        public static GameObject SpawnPrefab(string prefabName, Transform parent)
        {
            return SpawnPrefabInternal(prefabName, null, parent);
        }

        /// <summary>
        /// 프리팹을 지정된 위치에 스폰하고 지정된 컴포넌트를 반환합니다.
        /// </summary>
        public static T SpawnPrefab<T>(string prefabName, Vector3 position) where T : Component
        {
            return SpawnPrefabInternal<T>(prefabName, position);
        }

        /// <summary>
        /// 프리팹을 지정된 부모 하위에 스폰하고 지정된 컴포넌트를 반환합니다.
        /// </summary>
        public static T SpawnPrefab<T>(string prefabName, Transform parent) where T : Component
        {
            return SpawnPrefabInternal<T>(prefabName, null, parent);
        }

        #endregion Prefab Spawning

        #region Async Prefab Spawning

        /// <summary>
        /// 프리팹을 비동기로 로드하고 지정된 위치에 스폰합니다.
        /// </summary>
        public static async Task<T> SpawnPrefabAsync<T>(string prefabName, Vector3 position) where T : Component
        {
            return await SpawnPrefabInternalAsync<T>(prefabName, position);
        }

        /// <summary>
        /// 프리팹을 비동기로 로드하고 지정된 부모 하위에 스폰합니다.
        /// </summary>
        public static async Task<T> SpawnPrefabAsync<T>(string prefabName, Transform parent) where T : Component
        {
            return await SpawnPrefabInternalAsync<T>(prefabName, null, parent);
        }

        /// <summary>
        /// 프리팹을 비동기로 로드하고 지정된 위치에 스폰합니다.
        /// </summary>
        public static async Task<GameObject> SpawnPrefabAsync(string prefabName, Vector3 position)
        {
            return await SpawnPrefabInternalAsync(prefabName, position);
        }

        /// <summary>
        /// 프리팹을 비동기로 로드하고 지정된 부모 하위에 스폰합니다.
        /// </summary>
        public static async Task<GameObject> SpawnPrefabAsync(string prefabName, Transform parent)
        {
            return await SpawnPrefabInternalAsync(prefabName, null, parent);
        }

        #endregion Async Prefab Spawning

        #region Object Pooling

        /// <summary>
        /// 게임오브젝트를 풀에서 제거합니다.
        /// </summary>
        public static void Despawn(GameObject clone)
        {
            if (clone.activeInHierarchy && clone.activeSelf)
            {
                if (false == XScene.IsChangeScene)
                {
                    LeanPool.Despawn(clone);
                }
                else
                {
                    Log.Info(LogTags.Resource, "씬 전환 중에는 LeadPool을 사용하여 제거할 수 없습니다. {0}", clone.GetHierarchyName());
                }
            }
        }

        /// <summary>
        /// 게임오브젝트를 지정된 지연 시간 후 풀에서 제거합니다.
        /// </summary>
        public static void Despawn(GameObject clone, float delay)
        {
            if (clone.activeInHierarchy && clone.activeSelf)
            {
                if (false == XScene.IsChangeScene)
                {
                    LeanPool.Despawn(clone, delay);
                }
                else
                {
                    GameObject.Destroy(clone);
                    Log.Warning("씬 전환 중에는 LeadPool을 사용하여 제거할 수 없습니다. 게임오브젝트를 삭제합니다. {0}", clone.GetHierarchyName());
                }
            }
        }

        #endregion Object Pooling

        #region Addressable System

        /// <summary>
        /// Addressable 시스템이 초기화되었는지 확인합니다.
        /// </summary>
        public static bool IsAddressableSystemInitialized()
        {
            try
            {
                return UnityEngine.AddressableAssets.Addressables.ResourceManager != null;
            }
            catch
            {
                return false;
            }
        }

        #endregion Addressable System

        #region Resource Loading

        /// <summary>
        /// 리소스를 동기적으로 로드합니다.
        /// </summary>
        public static T LoadResource<T>(string path) where T : UnityEngine.Object
        {
            if (!IsValidPath(path, "리소스 경로"))
            {
                return null;
            }

            return _addressableManager.LoadResource<T>(path);
        }

        /// <summary>
        /// 리소스를 비동기적으로 로드합니다.
        /// </summary>
        public static async Task<T> LoadResourceAsync<T>(string path) where T : UnityEngine.Object
        {
            if (!IsValidPath(path, "리소스 경로"))
            {
                return null;
            }

            try
            {
                T resource = await LoadAddressableResourceAsync<T>(path);
                if (resource != null)
                {
                    return resource;
                }
            }
            catch (Exception ex)
            {
                Log.Warning(LogTags.Resource, $"Addressable 로드 실패. 경로:{path}, 메세지:{ex.Message}");
            }

            return null;
        }

        /// <summary>
        /// Addressable 리소스를 비동기적으로 로드합니다.
        /// </summary>
        public static async Task<T> LoadAddressableResourceAsync<T>(string path) where T : UnityEngine.Object
        {
            if (!IsValidPath(path, "리소스 경로"))
            {
                return null;
            }

            return await _addressableManager.LoadResourceAsync<T>(path);
        }

        /// <summary>
        /// 라벨로 여러 리소스를 비동기적으로 로드합니다.
        /// </summary>
        public static async Task<IList<T>> LoadResourcesByLabelAsync<T>(string label) where T : UnityEngine.Object
        {
            if (!IsValidPath(label, "라벨"))
            {
                return new List<T>();
            }

            try
            {
                return await _addressableManager.LoadResourcesByLabelAsync<T>(label);
            }
            catch (Exception ex)
            {
                Log.Error(LogTags.Resource, "라벨 리소스 로드 실패: {0}, 오류: {1}", label, ex.Message);
                return new List<T>();
            }
        }

        #endregion Resource Loading

        #region Specialized Resource Loading

        /// <summary>
        /// 스프라이트 아틀라스를 로드합니다.
        /// </summary>
        public static SpriteAtlas LoadSpriteAtlas(string atlasPath)
        {
            return LoadResource<SpriteAtlas>(atlasPath);
        }

        /// <summary>
        /// 스프라이트를 로드합니다.
        /// </summary>
        public static Sprite LoadSprite(string spritePath)
        {
            return LoadResource<Sprite>(spritePath);
        }

        /// <summary>
        /// 텍스처를 로드합니다.
        /// </summary>
        public static Texture2D LoadTexture(string texturePath)
        {
            return LoadResource<Texture2D>(texturePath);
        }

        /// <summary>
        /// 오디오 클립을 로드합니다.
        /// </summary>
        public static AudioClip LoadAudioClip(string audioPath)
        {
            return LoadResource<AudioClip>(audioPath);
        }

        /// <summary>
        /// 머티리얼을 로드합니다.
        /// </summary>
        public static Material LoadMaterial(string materialPath)
        {
            return LoadResource<Material>(materialPath);
        }

        /// <summary>
        /// 폰트를 로드합니다.
        /// </summary>
        public static Font LoadFont(string fontPath)
        {
            return LoadResource<Font>(fontPath);
        }

        /// <summary>
        /// 스프라이트 아틀라스를 비동기적으로 로드합니다.
        /// </summary>
        public static async Task<SpriteAtlas> LoadSpriteAtlasAsync(string atlasPath)
        {
            return await LoadResourceAsync<SpriteAtlas>(atlasPath);
        }

        /// <summary>
        /// 스프라이트를 비동기적으로 로드합니다.
        /// </summary>
        public static async Task<Sprite> LoadSpriteAsync(string spritePath)
        {
            return await LoadResourceAsync<Sprite>(spritePath);
        }

        /// <summary>
        /// 텍스처를 비동기적으로 로드합니다.
        /// </summary>
        public static async Task<Texture2D> LoadTextureAsync(string texturePath)
        {
            return await LoadResourceAsync<Texture2D>(texturePath);
        }

        /// <summary>
        /// 오디오 클립을 비동기적으로 로드합니다.
        /// </summary>
        public static async Task<AudioClip> LoadAudioClipAsync(string audioPath)
        {
            return await LoadResourceAsync<AudioClip>(audioPath);
        }

        /// <summary>
        /// 머티리얼을 비동기적으로 로드합니다.
        /// </summary>
        public static async Task<Material> LoadMaterialAsync(string materialPath)
        {
            return await LoadResourceAsync<Material>(materialPath);
        }

        /// <summary>
        /// 폰트를 비동기적으로 로드합니다.
        /// </summary>
        public static async Task<Font> LoadFontAsync(string fontPath)
        {
            return await LoadResourceAsync<Font>(fontPath);
        }

        #endregion Specialized Resource Loading

        #region Advanced Resource Loading

        /// <summary>
        /// 스프라이트 이름에 따라 적절한 아틀라스를 비동기로 로드합니다.
        /// </summary>
        private static async Task<SpriteAtlas> LoadSpriteAtlasBySpriteNameAsync(string spriteName)
        {
            if (string.IsNullOrEmpty(spriteName))
            {
                return null;
            }

            string atlasName = GetAtlasNameBySpriteName(spriteName);
            string atlasPath = PathManager.FindAtlasPath(atlasName);
            return await LoadResourceAsync<SpriteAtlas>(atlasPath);
        }

        /// <summary>
        /// 스프라이트 이름에 따라 적절한 아틀라스 이름을 반환하는 헬퍼 메서드
        /// </summary>
        private static string GetAtlasNameBySpriteName(string spriteName)
        {
            if (spriteName.Contains("ui_item_inven_") || spriteName.Contains("ui_item_runewords_") ||
                spriteName.Contains("ui_item_mythic_") || spriteName.Contains("ui_item_unlock_") ||
                spriteName.Contains("ui_item_book_"))
            {
                return "atlas_resources_item";
            }
            else if (spriteName.Contains("ui_skill_icon_"))
            {
                return "atlas_resources_skill";
            }
            else if (spriteName.Contains("portrait"))
            {
                return "atlas_resources_npc";
            }
            else
            {
                return "atlas_resources";
            }
        }

        #endregion Advanced Resource Loading

        #region Private Helper Methods

        private static GameObject SpawnPrefabInternal(string prefabName, Vector3? position = null, Transform parent = null)
        {
            string prefabPath = PathManager.FindPrefabPath(prefabName);
            GameObject prefab = LoadResource<GameObject>(prefabPath);

            if (prefab == null)
            {
                LogPrefabSpawnFailure(prefabName, "캐시에서 찾기");
                return null;
            }

            return position.HasValue
                ? Instantiate(prefab, position.Value)
                : Instantiate(prefab, parent);
        }

        private static async Task<GameObject> SpawnPrefabInternalAsync(string prefabName, Vector3? position = null, Transform parent = null)
        {
            string prefabPath = PathManager.FindPrefabPath(prefabName);
            GameObject prefab = await LoadResourceAsync<GameObject>(prefabPath);

            if (prefab == null)
            {
                LogPrefabSpawnFailure(prefabName, "비동기 로드");
                return null;
            }

            return position.HasValue
                ? Instantiate(prefab, position.Value)
                : Instantiate(prefab, parent);
        }

        /// 제네릭 타입으로 프리팹을 동기적으로 스폰하는 내부 메서드
        /// </summary>
        private static T SpawnPrefabInternal<T>(string prefabName, Vector3? position = null, Transform parent = null) where T : Component
        {
            GameObject instance = SpawnPrefabInternal(prefabName, position, parent);
            return instance?.GetComponent<T>();
        }

        private static async Task<T> SpawnPrefabInternalAsync<T>(string prefabName, Vector3? position = null, Transform parent = null) where T : Component
        {
            GameObject instance = await SpawnPrefabInternalAsync(prefabName, position, parent);
            return instance?.GetComponent<T>();
        }

        private static GameObject InstantiateInternal(GameObject prefab, Vector3? position = null, Transform parent = null, bool usePool = true)
        {
            if (prefab == null)
            {
                return null;
            }

            Vector3 spawnPosition = position ?? prefab.transform.position;
            Quaternion spawnRotation = prefab.transform.rotation;

            return usePool
                ? LeanPool.Spawn(prefab, spawnPosition, spawnRotation, parent)
                : UnityEngine.Object.Instantiate(prefab, spawnPosition, spawnRotation, parent);
        }

        private static T InstantiateInternal<T>(GameObject prefab, Vector3? position = null, Transform parent = null, bool usePool = true)
        {
            if (prefab == null)
            {
                return default;
            }

            GameObject clone = InstantiateInternal(prefab, position, parent, usePool);
            if (clone == null)
            {
                LogPrefabInstantiateFailure(prefab.name.ToDisableString());
                return default;
            }

            return clone.GetComponent<T>();
        }

        private static void LogResourceLoadFailure(string resourceName, string operation = "로드")
        {
            if (Log.LevelWarning)
            {
                Log.Warning(LogTags.Resource, "리소스를 {0}할 수 없습니다: {1}", operation, resourceName);
            }
        }

        private static void LogPrefabSpawnFailure(string prefabName, string operation = "스폰")
        {
            if (Log.LevelWarning)
            {
                Log.Warning(LogTags.Resource, "프리팹을 {0}할 수 없습니다: {1}", operation, prefabName);
            }
        }

        private static void LogPrefabInstantiateFailure(string prefabName)
        {
            if (Log.LevelWarning)
            {
                Log.Warning(LogTags.Resource, "프리팹을 인스턴스화할 수 없습니다: {0}", prefabName);
            }
        }

        private static bool IsValidPath(string path, string pathType = "경로")
        {
            if (string.IsNullOrEmpty(path))
            {
                if (Log.LevelWarning)
                {
                    Log.Warning(LogTags.Resource, "{0}이 비어 있습니다.", pathType);
                }
                return false;
            }
            return true;
        }

        #endregion Private Helper Methods
    }
}