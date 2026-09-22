$targetDir = $PSScriptRoot
if ([string]::IsNullOrEmpty($targetDir)) {
    $targetDir = Get-Location
}

$drivers = @(
    @{ Name = "01_Chipset.zip"; Url = "https://www.sparq.co.kr/down/Clevo/h58/win10/01_Chipset.zip"; Desc = "칩셋 드라이버" },
    @{ Name = "02_VGA.zip"; Url = "https://www.sparq.co.kr/down/Clevo/h58/win10/02_VGA.zip"; Desc = "인텔 내장 VGA 드라이버" },
    @{ Name = "03_LAN.zip"; Url = "https://www.sparq.co.kr/down/Clevo/h58/win10/03_LAN.zip"; Desc = "Realtek 유선랜 드라이버" },
    @{ Name = "04_Cardreader.zip"; Url = "https://www.sparq.co.kr/down/Clevo/h58/win10/04_Cardreader.zip"; Desc = "Realtek 카드리더 드라이버" },
    @{ Name = "05_Touchpad.zip"; Url = "https://www.sparq.co.kr/down/Clevo/h58/win10/05_Touchpad.zip"; Desc = "Synaptics 터치패드 드라이버" },
    @{ Name = "06_HID.zip"; Url = "https://www.sparq.co.kr/down/Clevo/h58/win10/06_HID.zip"; Desc = "Intel HID 단축키 드라이버" },
    @{ Name = "07_Hotkey.zip"; Url = "https://www.sparq.co.kr/down/Clevo/h58/win10/07_Hotkey.zip"; Desc = "Clevo 컨트롤 센터 (Hotkey)" },
    @{ Name = "08_MEI.zip"; Url = "https://www.sparq.co.kr/down/Clevo/h58/win10/08_MEI.zip"; Desc = "Intel Management Engine 드라이버" },
    @{ Name = "09_Audio.zip"; Url = "https://www.sparq.co.kr/down/Clevo/h58/win10/09_Audio.zip"; Desc = "Realtek 오디오 드라이버" },
    @{ Name = "10_WLAN.zip"; Url = "https://www.sparq.co.kr/down/Clevo/h58/win10/10_WLAN.zip"; Desc = "Intel 무선랜(Wi-Fi) 드라이버" },
    @{ Name = "11_BT.zip"; Url = "https://www.sparq.co.kr/down/Clevo/h58/win10/11_BT.zip"; Desc = "Intel 블루투스 드라이버" },
    @{ Name = "12_IRST.zip"; Url = "https://www.sparq.co.kr/down/Clevo/h58/win10/12_IRST.zip"; Desc = "Intel 빠른 스토리지(IRST) 드라이버" }
)

Write-Host "============================================================" -ForegroundColor Cyan
Write-Host "   한성컴퓨터 H58 공식 드라이버 12종 다운로더" -ForegroundColor Cyan
Write-Host "============================================================" -ForegroundColor Cyan
Write-Host "저장 위치: $targetDir`n"

foreach ($item in $drivers) {
    $dest = Join-Path $targetDir $item.Name
    if (Test-Path $dest) {
        $sizeMB = [math]::Round(((Get-Item $dest).Length / 1MB), 2)
        Write-Host ("[이미 존재함] {0} ({1} MB) -> 다운로드 건너뜀" -f $item.Name, $sizeMB) -ForegroundColor Yellow
        continue
    }
    Write-Host ("[{0}] {1} 다운로드 중... -> {2}" -f $item.Desc, $item.Name, $item.Url)
    & curl.exe -fSL --retry 3 --retry-delay 2 -o $dest $item.Url
    if (Test-Path $dest) {
        $sizeMB = [math]::Round(((Get-Item $dest).Length / 1MB), 2)
        Write-Host ("  -> 완료! 파일 크기: {0} MB" -f $sizeMB) -ForegroundColor Green
    } else {
        Write-Host ("  -> 실패!" ) -ForegroundColor Red
    }
}

Write-Host "`n=== 드라이버 다운로드 상태 요약 ===" -ForegroundColor Cyan
Get-ChildItem $targetDir -Filter "*.zip" | Select-Object Name, @{Name="크기 (MB)"; Expression={[math]::Round($_.Length/1MB, 2)}}, LastWriteTime | Format-Table -AutoSize
