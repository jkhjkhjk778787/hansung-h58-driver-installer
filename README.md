# 한성컴퓨터 H58 (Clevo NB50TZ) 원클릭 드라이버 자동 설치 도구

한성컴퓨터 **H58** 노트북(클레보 NB50TZ 베어본 기반, 인텔 8세대 데스크탑 CPU 탑재 모델)의 공식 윈도우 10 드라이버 12종을 자동으로 다운로드하고, PnP 드라이버 스토어에 일괄 등록 및 설치해 주는 원클릭 자동화 유틸리티입니다.

---

## 💻 지원 사양 (Target Specifications)

* **제조사/모델**: 한성컴퓨터 H58 (H Series / SKU: WN01-0596)
* **메인보드/섀시**: Clevo NB50TZ / NB50TK1
* **프로세서**: Intel 8th Gen Desktop Core Processors (i5-8500 등 LGA1151)
* **그래픽**: Intel UHD Graphics 630
* **지원 운영체제**: Windows 10 64-bit

---

## 📦 구성 파일 및 폴더 구조

```text
H58_Drivers/
├── src/
│   ├── Program.cs          # 원클릭 통합 설치기 C# 소스 코드
│   └── app.manifest        # UAC 관리자 권한 자동 획득 매니페스트
├── build.bat               # 윈도우 기본 csc.exe 컴파일러를 이용한 빌드 스크립트
├── install.bat             # 설치기 원클릭 실행 배치 파일
├── download_drivers.ps1    # 한성 공식 서버에서 드라이버 12종을 받는 다운로더
├── .gitignore              # 1.3GB 대용량 드라이버 ZIP 및 임시 파일 제외
├── README.md               # 프로젝트 안내 문서
└── LICENSE                 # MIT 라이선스
```

---

## 🚀 사용 방법 (Quick Start)

### 1. 드라이버 다운로드
PowerShell을 실행하여 아래 스크립트를 실행합니다. (공식 서버로부터 12개 드라이버 자동 다운로드)
```powershell
powershell -ExecutionPolicy Bypass -File download_drivers.ps1
```

### 2. 원클릭 드라이버 설치
`H58_원클릭_드라이버설치.exe` 또는 `install.bat`을 실행합니다.
* 관리자 권한(UAC) 승인 창이 뜨면 **[예]**를 누릅니다.
* 프롬프트에서 `Y`를 입력하면 자동으로 압축 해제 $\rightarrow$ PnP 드라이버 등록(`pnputil`) $\rightarrow$ 전용 소프트웨어(Control Center, MEI, IRST) 순으로 일괄 설치됩니다.
* 설치가 완료된 후 재부팅하면 모든 장치 인식이 완료됩니다.

---

## 🔨 소스 코드 빌드 (Build from Source)

별도의 Visual Studio 설치 없이 Windows 10에 기본 내장된 C# 컴파일러(`csc.exe`)로 즉시 빌드할 수 있습니다.

```cmd
build.bat
```
빌드 성공 시 `H58_원클릭_드라이버설치.exe` 및 `Install_All_Drivers.exe`가 생성됩니다.

---

## 📋 포함 드라이버 목록 (한성 공식 배포)

| 번호 | 패키지명 | 드라이버 설명 | 설치 방식 |
| :---: | :--- | :--- | :--- |
| **01** | `01_Chipset.zip` | Intel 300 Series 칩셋 드라이버 | INF PnP 자동 설치 |
| **02** | `02_VGA.zip` | Intel UHD Graphics 630 그래픽 드라이버 | INF PnP 자동 설치 |
| **03** | `03_LAN.zip` | Realtek PCIe GBE Family 유선랜 드라이버 | INF PnP 자동 설치 |
| **04** | `04_Cardreader.zip` | Realtek PCIE 카드리더 드라이버 | INF PnP 자동 설치 |
| **05** | `05_Touchpad.zip` | Synaptics 정밀 터치패드 드라이버 | INF PnP 자동 설치 |
| **06** | `06_HID.zip` | Intel HID Event Filter / Airplane Mode | INF PnP 자동 설치 |
| **07** | `07_Hotkey.zip` | Clevo Control Center (팬 제어, 단축키) | 무인 자동 설치 (`/s`) |
| **08** | `08_MEI.zip` | Intel Management Engine Interface | 무인 자동 설치 (`-s`) |
| **09** | `09_Audio.zip` | Realtek High Definition Audio 오디오 | INF PnP 자동 설치 |
| **10** | `10_WLAN.zip` | Intel Wireless-AC 무선랜 (Wi-Fi) | INF PnP 자동 설치 |
| **11** | `11_BT.zip` | Intel Wireless Bluetooth 블루투스 | INF PnP 자동 설치 |
| **12** | `12_IRST.zip` | Intel 빠른 스토리지 기술 (IRST) | 무인 자동 설치 (`-s`) |

---

## 📄 라이선스 (License)

본 자동화 설치 스크립트 및 도구는 [MIT License](LICENSE)에 따라 자유롭게 수정 및 배포할 수 있습니다. (드라이버 바이너리 파일은 각 제조사 Intel/Realtek/Clevo의 라이선스를 따릅니다.)
