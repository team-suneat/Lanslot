using UnityEngine;

namespace TeamSuneat.Data.Game
{
    /// <summary>
    /// GameDataManager의 저장 로직을 담당하는 partial 클래스
    /// </summary>
    public partial class GameDataManager
    {
        /// <summary>
        /// 게임 데이터를 저장합니다.
        /// </summary>
        public void Save()
        {
            float currentTime = Time.time;

            // 쿨다운 체크
            if (currentTime - _lastSaveTime < SAVE_COOLDOWN_TIME)
            {
                _pendingSave = true;
                return;
            }

            if (USE_ASYNC_SAVE) { SaveAsync(); }    // 비동기 저장 모드
            else { SaveSync(); }// 동기 저장 모드

            GlobalEvent.Send(GlobalEventType.SAVE_GAME_DATA);
        }

        /// <summary>
        /// 쿨다운이 지난 후 대기 중인 저장을 처리합니다.
        /// </summary>
        public void ProcessPendingSave()
        {
            if (_pendingSave && Time.time - _lastSaveTime >= SAVE_COOLDOWN_TIME)
            {
                _pendingSave = false;
                Save();
            }
        }

        /// <summary>
        /// 에디터 또는 개발 빌드용 저장을 수행합니다.
        /// </summary>
        /// <returns>저장된 chunk</returns>
        public string SaveForEditorOrDevelopment()
        {
            string chunk = SaveForEditor(0);

            if (!string.IsNullOrEmpty(chunk))
            {
                if (_saveCount >= GAME_DATA_SAVE_INTERVAL_COUNT)
                {
                    _ = SaveForEditor(1);

                    _saveCount = 0;
                }
            }

            return chunk;
        }

        /// <summary>
        /// 빌드용 저장을 수행합니다.
        /// </summary>
        /// <returns>저장된 chunk</returns>
        public string SaveForBuild()
        {
            string chunk = SaveForBuild(0);
            if (!string.IsNullOrEmpty(chunk))
            {
                if (_saveCount >= GAME_DATA_SAVE_INTERVAL_COUNT)
                {
                    _ = SaveForBuild(1);
                    _saveCount = 0;
                }
            }

            return chunk;
        }

        /// <summary>
        /// 에디터용 저장을 수행합니다.
        /// </summary>
        /// <param name="index">저장할 인덱스</param>
        /// <returns>저장된 chunk</returns>
        public string SaveForEditor(int index)
        {
            return PerformSave(index, false);
        }

        /// <summary>
        /// 빌드용 저장을 수행합니다.
        /// </summary>
        /// <param name="index">저장할 인덱스</param>
        /// <returns>저장된 chunk</returns>
        private string SaveForBuild(int index)
        {
            return PerformSave(index, false);
        }

        /// <summary>
        /// 게임 데이터를 저장합니다 (통합된 저장 메서드).
        /// </summary>
        /// <param name="index">저장할 인덱스</param>
        /// <param name="isBackground">백그라운드 스레드에서 호출되는지 여부</param>
        /// <returns>저장된 chunk</returns>
        private string PerformSave(int index, bool isBackground = false, float? customTimestamp = null)
        {
            string chunk = ConvertToChunk(index, isBackground);
            if (string.IsNullOrEmpty(chunk))
            {
                return null; // 변경사항이 없음
            }

            string saveFilePath = GetSaveFilePath(index);
            string chunkAES = TryApplyAES() ? Encrypt(chunk) : chunk;

            if (Write(saveFilePath, chunkAES))
            {
                // 백그라운드에서는 customTimestamp 사용, 동기에서는 Time.time 사용
                if (isBackground)
                {
                    // 백그라운드에서는 미리 준비된 timestamp 사용
                    UpdateSaveState(chunk, index, customTimestamp ?? _lastSaveTime, isBackground);
                }
                else
                {
                    // 동기에서는 Time.time 사용
                    UpdateSaveState(chunk, index, Time.time, isBackground);
                }
                return chunk;
            }

            return null;
        }

        /// <summary>
        /// GameData를 chunk로 변환합니다.
        /// </summary>
        /// <param name="index">인덱스</param>
        /// <param name="isBackground">백그라운드 스레드에서 호출되는지 여부</param>
        /// <returns>변환된 chunk</returns>
        private string ConvertToChunk(int index, bool isBackground = false)
        {
            string chunk = null;

            if (Data != null)
            {
                chunk = SerializeObject(Data);
            }

            if (string.IsNullOrEmpty(chunk))
            {
                return null;
            }

            // 스레드 안전성 처리
            if (isBackground)
            {
                lock (_asyncSaveLock)
                {
                    if (_storedChunks[index] == chunk)
                    {
                        return null;
                    }
                }
            }
            else
            {
                if (_storedChunks[index] == chunk)
                {
                    return null;
                }
            }

            return chunk;
        }

        /// <summary>
        /// 저장 상태를 업데이트합니다.
        /// </summary>
        /// <param name="chunk">저장된 chunk</param>
        /// <param name="index">저장 인덱스</param>
        /// <param name="timestamp">저장 시간 (메인 스레드에서 미리 계산된 값)</param>
        /// <param name="isBackground">백그라운드 스레드에서 호출되는지 여부</param>
        private void UpdateSaveState(string chunk, int index, float timestamp, bool isBackground = false)
        {
            _storedChunks[index] = chunk;

            if (index == 0)
            {
                _saveCount++;
                _lastSaveTime = timestamp;

                if (CheckUnityEditor())
                {
                    Debug.Log($"유니티 에디터용 게임 데이터를 저장합니다. Index:{index}, SaveCount:{_saveCount}/{GAME_DATA_SAVE_INTERVAL_COUNT}");
                }

                // 백업 저장이 필요한 경우
                if (_saveCount >= GAME_DATA_SAVE_INTERVAL_COUNT)
                {
                    _ = PerformSave(1, isBackground, timestamp); // 백업 저장
                    _saveCount = 0;

                    if (CheckUnityEditor())
                    {
                        Debug.Log($"유니티 에디터용 게임 백업 데이터를 저장합니다. Index:1");
                    }
                }
            }
        }

        #region 비동기 저장

        /// <summary>
        /// 비동기적으로 게임 데이터를 저장합니다.
        /// </summary>
        private void SaveAsync()
        {
            lock (_asyncSaveLock)
            {
                if (_isAsyncSaving)
                {
                    Debug.LogWarning("이미 비동기 저장 중입니다. 저장 요청을 무시합니다.");
                    return;
                }
                _isAsyncSaving = true;
            }

            // 메인 스레드에서 최소한의 정보만 준비
            bool needsBackup = _saveCount >= GAME_DATA_SAVE_INTERVAL_COUNT;
            bool useEncryption = TryApplyAES();
            float currentTimestamp = Time.time; // 메인 스레드에서 미리 계산

            TeamSuneat.Log.Progress("게임 데이터를 저장합니다: {0}/{1}", _saveCount, GAME_DATA_SAVE_INTERVAL_COUNT);

            // 모든 무거운 작업을 백그라운드 스레드로 이동
            _ = System.Threading.Tasks.Task.Run(() => PerformFullAsyncSave(needsBackup, useEncryption, currentTimestamp));
        }

        /// <summary>
        /// 완전한 비동기 저장 작업을 수행합니다 (직렬화, 암호화, 파일 쓰기 모두 백그라운드에서).
        /// </summary>
        private void PerformFullAsyncSave(bool needsBackup, bool useEncryption, float timestamp)
        {
            bool saveSuccess = false;
            bool backupSuccess = true;
            string errorMessage = null;
            string originalChunk = null;

            try
            {
                // 백그라운드에서 저장 수행
                originalChunk = PerformSave(0, true, timestamp);
                if (string.IsNullOrEmpty(originalChunk))
                {
                    return; // 변경사항이 없으면 저장하지 않음
                }

                saveSuccess = true;

                // 백업 저장도 필요하면 처리
                if (needsBackup)
                {
                    string backupChunk = PerformSave(1, true, timestamp);
                    backupSuccess = !string.IsNullOrEmpty(backupChunk);
                }
            }
            catch (System.Exception ex)
            {
                errorMessage = ex.Message;
                Debug.LogError(errorMessage);
            }
            finally
            {
                // 스레드 안전한 상태 리셋
                lock (_asyncSaveLock)
                {
                    _isAsyncSaving = false;
                }

                // 메인 스레드로 결과 전달 (Unity API 호출)
                MonoBehaviour monoBehaviour = UnityEngine.Object.FindAnyObjectByType<MonoBehaviour>();
                if (monoBehaviour != null)
                {
                    _ = monoBehaviour.StartCoroutine(
                        OnAsyncSaveComplete(saveSuccess, backupSuccess, originalChunk, needsBackup, errorMessage));
                }
            }
        }

        /// <summary>
        /// 비동기 저장 완료 시 메인 스레드에서 호출됩니다.
        /// </summary>
        private System.Collections.IEnumerator OnAsyncSaveComplete(bool saveSuccess, bool backupSuccess, string originalChunk, bool needsBackup, string errorMessage)
        {
            yield return null; // 한 프레임 대기

            if (saveSuccess)
            {
                if (backupSuccess)
                {
                    Debug.Log("비동기 저장이 완료되었습니다.");
                }
                else
                {
                    Debug.LogWarning("메인 저장은 성공했지만 백업 저장에 실패했습니다.");
                }
            }
            else
            {
                Debug.LogError("비동기 저장 중 파일 쓰기 실패");
            }

            if (!string.IsNullOrEmpty(errorMessage))
            {
                Debug.LogError($"비동기 저장 중 오류 발생: {errorMessage}");
            }
        }

        #endregion 비동기 저장

        #region 동기 저장

        /// <summary>
        /// 동기적으로 게임 데이터를 저장합니다.
        /// </summary>
        private void SaveSync()
        {
            if (CheckUnityEditor())
            {
                _ = SaveForEditorOrDevelopment();
            }
            else
            {
                _ = SaveForBuild();
            }
        }

        #endregion 동기 저장

        #region 유틸리티

        /// <summary>
        /// Unity 에디터인지 확인합니다.
        /// </summary>
        /// <returns>에디터 여부</returns>
        private bool CheckUnityEditor()
        {
#if UNITY_EDITOR
            return true;
#else
            return false;
#endif
        }

        #endregion 유틸리티
    }
}