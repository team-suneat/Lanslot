# 고급 한글 깨짐 자동 감지 및 수정 스크립트
# 사용법: .\FixKoreanEncodingAdvanced.ps1

# C# 파일들 찾기
$csFiles = Get-ChildItem -Path "." -Recurse -Filter "*.cs"
$garbledFiles = @()
$modifiedFiles = @()

Write-Host "깨진 한글 검색 중..." -ForegroundColor Yellow

foreach ($file in $csFiles) {
    $content = Get-Content $file.FullName -Encoding UTF8 -Raw
    $garbledLines = FindAllGarbledKorean $content
    
    if ($garbledLines.Count -gt 0) {
        $garbledFiles += @{
            File = $file.FullName
            Lines = $garbledLines
        }
        Write-Host "깨진 한글 발견: $($file.FullName)" -ForegroundColor Red
        $garbledLines | ForEach-Object { Write-Host "  $_" -ForegroundColor DarkRed }
    }
}

if ($garbledFiles.Count -eq 0) {
    Write-Host "깨진 한글이 발견되지 않았습니다." -ForegroundColor Green
} else {
    Write-Host "`n총 $($garbledFiles.Count)개 파일에서 깨진 한글을 발견했습니다." -ForegroundColor Yellow
    Write-Host "수정을 시작합니다..." -ForegroundColor Yellow
    
    foreach ($garbledFile in $garbledFiles) {
        $filePath = $garbledFile.File
        $content = Get-Content $filePath -Encoding UTF8 -Raw
        $originalContent = $content
        
        # 각 패턴에 대해 수정
        foreach ($key in $garbledPatterns.Keys) {
            $content = $content -replace [regex]::Escape($key), $garbledPatterns[$key]
        }
        
        # 내용이 변경되었으면 파일 저장
        if ($content -ne $originalContent) {
            Set-Content $filePath -Value $content -Encoding UTF8
            $modifiedFiles += $filePath
            Write-Host "수정 완료: $filePath" -ForegroundColor Green
        }
    }
    
    Write-Host "`n총 $($modifiedFiles.Count)개 파일이 수정되었습니다." -ForegroundColor Green
    Write-Host "수정된 파일 목록:" -ForegroundColor Cyan
    $modifiedFiles | ForEach-Object { Write-Host "  - $_" -ForegroundColor White }
}