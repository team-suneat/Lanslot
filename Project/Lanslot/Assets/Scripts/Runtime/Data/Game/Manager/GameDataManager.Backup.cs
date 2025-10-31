using System;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using UnityEngine;

namespace TeamSuneat.Data.Game
{
    /// <summary>
    /// GameDataManager의 백업 시스템 전체를 담당하는 partial 클래스
    /// </summary>
    public partial class GameDataManager
    {
        #region 백업 시스템 상수

        private const int MAX_BACKUP_COUNT = 10;
        private const int BACKUP_RETENTION_DAYS = 30;
        private const string BACKUP_FILE_PREFIX = "AutoBattle_Backup_";
        private const string BACKUP_FILE_NAME = "AutoBattle_Backup";

        #endregion 백업 시스템 상수

        #region 백업 생성

        /// <summary>
        /// 타임스탬프가 포함된 비상 백업을 생성합니다.
        /// </summary>
        /// <param name="chunk">백업할 데이터 chunk</param>
        /// <param name="originalFilePath">원본 파일 경로 (로그용)</param>
        private void SaveBackupWithTimestamp(string chunk, string originalFilePath)
        {
            try
            {
                string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
                string backupFilePath = GetBackupFilePathWithTimestamp(timestamp);
                string chunkAES = Encrypt(chunk);

                if (Write(backupFilePath, chunkAES))
                {
                    Debug.Log($"비상 백업 생성: {backupFilePath} (원본: {Path.GetFileName(originalFilePath)})");
                    CleanupOldBackups();
                }
                else
                {
                    Debug.LogError($"비상 백업 생성 실패: {backupFilePath}");
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"비상 백업 생성 중 오류: {ex.Message}");
            }
        }

        /// <summary>
        /// 타임스탬프가 포함된 백업 파일 경로를 반환합니다.
        /// </summary>
        /// <param name="timestamp">타임스탬프</param>
        /// <returns>백업 파일 경로</returns>
        private string GetBackupFilePathWithTimestamp(string timestamp)
        {
            return string.Format("{0}/{1}{2}.dat", Application.persistentDataPath, BACKUP_FILE_PREFIX, timestamp);
        }

        #endregion 백업 생성

        #region 백업 파일 검색

        /// <summary>
        /// 모든 백업 파일 경로를 반환합니다. (문자열 배열)
        /// </summary>
        /// <returns>백업 파일 경로 배열</returns>
        protected string[] GetAllBackupFilePaths()
        {
            try
            {
                string saveDirectory = Application.persistentDataPath;
                if (!Directory.Exists(saveDirectory))
                    return new string[0];

                // 기존 백업 파일과 새로운 타임스탬프 백업 파일 모두 검색
                var timestampedBackups = Directory.GetFiles(saveDirectory, $"{BACKUP_FILE_PREFIX}*.dat")
                    .Where(f => Path.GetFileName(f).Contains("_"))
                    .OrderByDescending(f => f)
                    .ToArray();

                var legacyBackups = Directory.GetFiles(saveDirectory, $"{BACKUP_FILE_NAME}.dat")
                    .OrderByDescending(f => f)
                    .ToArray();

                // 기존 백업을 먼저, 그 다음 타임스탬프 백업 순서로 합치기
                return legacyBackups.Concat(timestampedBackups).ToArray();
            }
            catch (Exception ex)
            {
                Debug.LogError($"백업 파일 검색 중 오류: {ex.Message}");
                return new string[0];
            }
        }

        /// <summary>
        /// 모든 백업 파일 정보를 FileInfo 배열로 반환합니다.
        /// </summary>
        /// <returns>백업 파일 정보 배열</returns>
        protected FileInfo[] GetAllBackupFileInfos()
        {
            try
            {
                string saveDirectory = Application.persistentDataPath;
                if (!Directory.Exists(saveDirectory))
                    return new FileInfo[0];

                // 기존 백업 파일과 새로운 타임스탬프 백업 파일 모두 검색
                var timestampedBackups = Directory.GetFiles(saveDirectory, $"{BACKUP_FILE_PREFIX}*.dat")
                    .Where(f => Path.GetFileName(f).Contains("_"))
                    .Select(f => new FileInfo(f))
                    .OrderByDescending(f => f.CreationTime)
                    .ToArray();

                var legacyBackups = Directory.GetFiles(saveDirectory, $"{BACKUP_FILE_NAME}.dat")
                    .Select(f => new FileInfo(f))
                    .OrderByDescending(f => f.CreationTime)
                    .ToArray();

                // 기존 백업을 먼저, 그 다음 타임스탬프 백업 순서로 합치기
                return legacyBackups.Concat(timestampedBackups).ToArray();
            }
            catch (Exception ex)
            {
                Debug.LogError($"백업 파일 정보 검색 중 오류: {ex.Message}");
                return new FileInfo[0];
            }
        }

        #endregion 백업 파일 검색

        #region 백업 복구

        /// <summary>
        /// 모든 백업 파일에서 데이터 복구를 시도합니다.
        /// </summary>
        /// <param name="originalFilePath">원본 파일 경로 (로그용)</param>
        /// <returns>복구된 GameData 객체</returns>
        public GameData TryLoadFromBackupFiles(string originalFilePath)
        {
            try
            {
                var backupFiles = GetAllBackupFilePaths();
                Debug.Log($"백업 파일 {backupFiles.Length}개 발견");

                foreach (string backupFile in backupFiles)
                {
                    Debug.Log($"백업 파일에서 복구 시도: {Path.GetFileName(backupFile)}");

                    GameData recoveredData = LoadGameData(backupFile);
                    if (recoveredData != null)
                    {
                        Debug.Log($"백업 파일에서 복구 성공: {Path.GetFileName(backupFile)}");
                        return recoveredData;
                    }
                }

                Debug.LogWarning("모든 백업 파일에서 복구에 실패했습니다.");
                return null;
            }
            catch (Exception ex)
            {
                Debug.LogError($"백업 파일 복구 중 오류: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// 백업 파일 정보를 로그로 출력합니다.
        /// </summary>
        public void LogBackupFileInfo()
        {
            try
            {
                var backupFiles = GetAllBackupFileInfos();
                Debug.Log($"백업 파일 정보 (총 {backupFiles.Length}개):");

                foreach (var file in backupFiles)
                {
                    Debug.Log($"  - {file.Name}");
                    Debug.Log($"    크기: {file.Length:N0} bytes");
                    Debug.Log($"    생성일: {file.CreationTime:yyyy-MM-dd HH:mm:ss}");
                    Debug.Log($"    수정일: {file.LastWriteTime:yyyy-MM-dd HH:mm:ss}");
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"백업 파일 정보 출력 중 오류: {ex.Message}");
            }
        }

        /// <summary>
        /// 특정 백업 파일에서 데이터를 복구합니다.
        /// </summary>
        /// <param name="backupFileName">백업 파일명</param>
        /// <returns>복구 성공 여부</returns>
        public bool RestoreFromBackup(string backupFileName)
        {
            try
            {
                string saveDirectory = Application.persistentDataPath;
                string backupFilePath = Path.Combine(saveDirectory, backupFileName);

                if (!File.Exists(backupFilePath))
                {
                    Debug.LogError($"백업 파일이 존재하지 않습니다: {backupFileName}");
                    return false;
                }

                // 백업 파일에서 GameData 직접 로드
                string chunk = File.ReadAllText(backupFilePath);
                if (string.IsNullOrEmpty(chunk))
                {
                    Debug.LogError($"백업 파일이 비어있습니다: {backupFileName}");
                    return false;
                }

                // 복호화 시도
                string symmetricKey = AES.Encrypt(GameSymmetricIdentifier(), "pub");
                string decryptedChunk = AES.Decrypt(chunk, symmetricKey);

                if (string.IsNullOrEmpty(decryptedChunk))
                {
                    Debug.LogError($"백업 파일 복호화 실패: {backupFileName}");
                    return false;
                }

                // 마이그레이션을 통한 GameData 로드
                GameData recoveredData = MigrateAndLoad(decryptedChunk);
                if (recoveredData == null)
                {
                    Debug.LogError($"백업 파일에서 GameData 로드 실패: {backupFileName}");
                    return false;
                }

                // 복구된 데이터를 메인 세이브 파일에 저장
                string mainSavePath = GetSaveFilePath(0);
                string serializedData = JsonConvert.SerializeObject(recoveredData, Formatting.Indented,
                    new JsonSerializerSettings { ContractResolver = new OrderedContractResolver() });
                string encryptedData = AES.Encrypt(serializedData, symmetricKey);

                File.WriteAllText(mainSavePath, encryptedData);

                Debug.Log($"백업에서 복구 성공: {backupFileName}");
                return true;
            }
            catch (Exception ex)
            {
                Debug.LogError($"백업 복구 중 오류: {ex.Message}");
                return false;
            }
        }

        #endregion 백업 복구

        #region 백업 정리

        /// <summary>
        /// 오래된 백업 파일들을 정리합니다.
        /// </summary>
        private void CleanupOldBackups()
        {
            try
            {
                var backupFiles = GetAllBackupFileInfos();

                // 최대 개수 초과 시 오래된 파일 삭제
                if (backupFiles.Length > MAX_BACKUP_COUNT)
                {
                    var filesToDelete = backupFiles.Skip(MAX_BACKUP_COUNT);
                    foreach (var file in filesToDelete)
                    {
                        try
                        {
                            File.Delete(file.FullName);
                            Debug.Log($"오래된 백업 파일 삭제: {file.Name}");
                        }
                        catch (Exception ex)
                        {
                            Debug.LogWarning($"백업 파일 삭제 실패: {file.Name}, 오류: {ex.Message}");
                        }
                    }
                }

                // 보관 기간 초과 파일 삭제
                DateTime cutoffDate = DateTime.Now.AddDays(-BACKUP_RETENTION_DAYS);
                var expiredFiles = backupFiles.Where(f => f.CreationTime < cutoffDate);

                foreach (var file in expiredFiles)
                {
                    try
                    {
                        File.Delete(file.FullName);
                        Debug.Log($"만료된 백업 파일 삭제: {file.Name} (생성일: {file.CreationTime:yyyy-MM-dd})");
                    }
                    catch (Exception ex)
                    {
                        Debug.LogWarning($"백업 파일 삭제 실패: {file.Name}, 오류: {ex.Message}");
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"백업 파일 정리 중 오류: {ex.Message}");
            }
        }

        #endregion 백업 정리
    }
}